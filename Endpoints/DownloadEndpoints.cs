using System.Reactive.Linq;
using AicaDocsApi.Database;
using AicaDocsApi.Dto.Downloads;
using AicaDocsApi.Dto.Downloads.Filter;
using AicaDocsApi.Dto.FilterCommons;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Utils;
using AicaDocsApi.Validators.Commons;
using AicaDocsApi.Validators.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;

namespace AicaDocsApi.Endpoints;

public static class DownloadEndpoints
{
    public static void MapDownloadEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/download")
            .WithOpenApi()
            .WithTags(["Downloads"]);

        // ToDo: Quitar este endpoint en despliegue
        group.MapGet("", async (AicaDocsDb db, CancellationToken ct) => await db.Downloads.ToListAsync(ct))
            .WithSummary("ONLY FOR TESTING. Get all downloads");

        group.MapPost("/filter", FilterDownload)
            .WithSummary("Get downloads with specific filters, sorts and pagination")
            .AddEndpointFilter<ValidationFilter<FilterDownloadDto>>();

        group.MapGet("/{id:int}", GetDownloadById)
            .WithSummary("Get the download with the given id");

        group.MapPost("", PostDownloadDocument)
            .WithSummary("Download a document in the specified format")
            .AddEndpointFilter<ValidationFilter<DownloadCreatedDto>>();

        static async Task<Results<Ok<ApiResponse<string>>, NotFound<ApiResponse>>> PostDownloadDocument(
            DownloadCreatedDto dto,
            BucketNameProvider bucketNameProvider, IMinioClient minioClient, AicaDocsDb db, CancellationToken ct)
        {
            var doc = await db.Documents.FirstOrDefaultAsync(e => e.Id == dto.DocumentId, cancellationToken: ct);

            if (doc is null)
            {
                return TypedResults.NotFound(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 404, Errors = new Dictionary<string, string[]>
                        {
                            { "Document Id", ["Doesn`t exist a document with the given id"] }
                        }
                    }
                });
            }
            
            var format = dto.Format == Format.Pdf ? "pdf" : "word";
            var ext = dto.Format == Format.Pdf ? "pdf" : "docx";

            var valid = true;
            try
            {
                var argsLo = new ListObjectsArgs()
                    .WithBucket(bucketNameProvider.BucketName)
                    .WithPrefix($"{doc.Code + doc.Edition}.{ext}");
                await minioClient.ListObjectsAsync(argsLo, ct);
            }
            catch
            {
                valid = format != "pdf";
                if (!valid)
                {
                    ext = "doc";
                    try
                    {
                        var argsLo = new ListObjectsArgs()
                            .WithBucket(bucketNameProvider.BucketName)
                            .WithPrefix($"{doc.Code + doc.Edition}.{ext}");
                        await minioClient.ListObjectsAsync(argsLo, ct);
                    }
                    catch
                    {
                        valid = false;
                    }
                }
            }

            if (!valid)
            {
                return TypedResults.NotFound(new ApiResponse
                {
                    ProblemDetails = new()
                    {
                        Status = 404, Errors = new Dictionary<string, string[]>
                        {
                            { "Document Existance", ["Doesn`t exist a file document with the given specifications."] }
                        }
                    }
                });
            }

            var args = new PresignedGetObjectArgs()
                .WithBucket(bucketNameProvider.BucketName)
                .WithObject($"/{format}/{doc.Code + doc.Edition}.{ext}")
                .WithExpiry(60 * 5);
            var url = await minioClient.PresignedGetObjectAsync(args);

            db.Downloads.Add(dto.ToDownload());
            await db.SaveChangesAsync(ct);

            return TypedResults.Ok(new ApiResponse<string> { Data = url });
        }

        static async Task<Results<Ok<ApiResponse<Download>>, NotFound<ApiResponse>>> GetDownloadById(int id,
            AicaDocsDb db,
            CancellationToken ct)
        {
            var dl = await db.Downloads.FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);

            if (dl is null)
            {
                return TypedResults.NotFound(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 404, Errors = new Dictionary<string, string[]>
                        {
                            { "Download Id", ["Doesn`t exist a download with the given id"] }
                        }
                    }
                });
            }

            return TypedResults.Ok(new ApiResponse<Download>()
            {
                Data = dl
            });
        }

        static async Task<Results<Ok<ApiResponse<IEnumerable<Download>>>, BadRequest<ApiResponse>>>
            FilterDownload(
                FilterDownloadDto filter,
                ValidateUtils vu,
                AicaDocsDb db, CancellationToken ct)
        {
            if (filter.ReasonId is not null &&
                !await vu.ValidateNomenclatorId(filter.ReasonId, TypeOfNomenclator.ReasonOfDownload, ct))
                return TypedResults.BadRequest(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 400,
                        Errors = new Dictionary<string, string[]>
                        {
                            { "ReasonId", ["Download reason must be valid"] }
                        }
                    }
                });

            var data = db.Downloads.Where(a => true);
            if (filter.Format is not null)
                data = data.Where(t => t.Format == filter.Format);
            if (filter.Username is not null)
                data = data.Where(t => t.Username.Contains(filter.Username.Trim()));
            if (filter.DocumentId is not null)
                data = data.Where(t => t.DocumentId == filter.DocumentId);
            if (filter.ReasonId is not null)
                data = data.Where(t => t.ReasonId == filter.ReasonId);
            if (filter.DateDownload is not null)
                switch (filter.DateComparator)
                {
                    case DateComparator.Equal:
                        data = data.Where(t => t.DateOfDownload == filter.DateDownload);
                        break;
                    case DateComparator.Greater:
                        data = data.Where(t => t.DateOfDownload > filter.DateDownload);
                        break;
                    case DateComparator.Less:
                        data = data.Where(t => t.DateOfDownload < filter.DateDownload);
                        break;
                    case DateComparator.EqualGreater:
                        data = data.Where(t => t.DateOfDownload >= filter.DateDownload);
                        break;
                    case DateComparator.EqualLess:
                        data = data.Where(t => t.DateOfDownload <= filter.DateDownload);
                        break;
                }


            switch (filter.SortBy)
            {
                case SortByDownload.Id:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Id)
                        : data.OrderByDescending(t => t.Id);
                    break;
                case SortByDownload.Username:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Username)
                        : data.OrderByDescending(t => t.Username);
                    break;
                case SortByDownload.DateDownload:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.DateOfDownload)
                        : data.OrderByDescending(t => t.DateOfDownload);
                    break;
                case SortByDownload.Format:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Format)
                        : data.OrderBy(t => t.Format);
                    break;
            }

            data = data
                .Skip((filter.PaginationParams.PageNumber - 1) * filter.PaginationParams.PageSize)
                .Take(filter.PaginationParams.PageSize);

            return TypedResults.Ok(new ApiResponse<IEnumerable<Download>>
            {
                Data = await data.ToListAsync(cancellationToken: ct)
            });
        }
    }
}