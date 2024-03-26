using System.Security.Claims;
using AicaDocsApi.Database;
using AicaDocsApi.Dto.Downloads;
using AicaDocsApi.Dto.Downloads.Filter;
using AicaDocsApi.Dto.FilterCommons;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Utils.BlobServices;
using AicaDocsApi.Validators.Commons;
using AicaDocsApi.Validators.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Endpoints;

public static class DownloadEndpoints
{
    public static void MapDownloadEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/download")
            .WithOpenApi()
            .WithTags(["Downloads"])
            .RequireAuthorization("api");

        // ---------------- Endpoint Declarations --------------------//

        group.MapGet("/{id:int}", GetDownloadById)
            .WithSummary("Get the download with the given id")
            .WithDescription("""
                             This endpoint allows you to get the download with the given **id**.
                             """);

        group.MapPost("/filter", FilterDownload)
            .WithSummary("Get downloads with specific filters, sorts and pagination")
            .AddEndpointFilter<ValidationFilter<FilterDownloadDto>>()
            .WithDescription("""
                             This endpoint allows you to get downloads with the given filters, sorts and
                             pagination.

                             **TotalPages** data returned is the total count of pages with the current pageSize

                             The valid formats of download (**format**) are:
                             - **0** -> Pdf
                             - **1** -> Word

                             The type of nomenclator asociated with the
                             reason of download (**reasonId**) must be **1**.

                             The valid sort by variants (**sortBy**) are:
                             - **0** -> Id
                             - **1** -> DateDownload
                             - **2** -> Format
                             - **3** -> Username

                             The valid sort order variants (**sortOrder**) are:
                             - **0** -> Asc
                             - **1** -> Desc

                             The valid date comparator variants (**dateComparator**)
                             that is used in the filter of dateDownload are:
                             - **0** -> Equal
                             - **1** -> Greater
                             - **2** -> Greater or Equal
                             - **3** -> Less
                             - **4** -> Less or Equal
                             """);

        group.MapPost("", PostDownloadDocument)
            .WithSummary("Download a document in the specified format")
            .AddEndpointFilter<ValidationFilter<DownloadCreatedDto>>()
            .WithDescription("""
                             This endpoint allows you to create a new download with the given body.

                             The valid formats of download (**format**) are:
                             - **0** -> Pdf
                             - **1** -> Word

                             The type of nomenclator asociated with the
                             reason of download (**reasonId**) must be **1**.
                             """);

        // -------------------------- Endpoints Functions ---------------------------------- //

        // ------------ Get download by id --------- //
        static async Task<Results<Ok<ApiResponse<Download>>, NotFound<ApiResponse>>> GetDownloadById(int id,
            AicaDocsDb db,
            CancellationToken ct)
        {
            var dl = await db.Downloads.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);

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

        // ------------ Filter, Sort and Paginate Downloads --------- //
        static async Task<Results<Ok<ApiResponse<FilterResponse<Download>>>, BadRequest<ApiResponse>>>
            FilterDownload(
                FilterDownloadDto filter,
                ValidateUtils vu,
                AicaDocsDb db, CancellationToken ct)
        {
            // Prev Validations
            var validation = filter.ReasonId is not null &&
                             !await vu.ValidateNomenclatorId(filter.ReasonId, TypeOfNomenclator.ReasonOfDownload, ct);
            if (validation)
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

            // Filter
            var data = db.Downloads.AsNoTracking();
            if (filter.Format is not null)
                data = data.Where(t => t.Format == filter.Format);
            if (filter.Username is not null)
                data = data.Where(t => t.Username.ToLower().Contains(filter.Username.Trim().ToLower()));
            if (filter.DocumentId is not null)
                data = data.Where(t => t.DocumentId == filter.DocumentId);
            if (filter.ReasonId is not null)
                data = data.Where(t => t.ReasonId == filter.ReasonId);
            if (filter.DateDownload is not null)
            {
                DateTimeOffset date = filter.DateDownload.Value.ToUniversalTime();
                switch (filter.DateComparator)
                {
                    case DateComparator.Equal:
                        data = data.Where(t => t.DateOfDownload == date);
                        break;
                    case DateComparator.Greater:
                        data = data.Where(t => t.DateOfDownload > date);
                        break;
                    case DateComparator.Less:
                        data = data.Where(t => t.DateOfDownload < date);
                        break;
                    case DateComparator.EqualGreater:
                        data = data.Where(t => t.DateOfDownload >= date);
                        break;
                    case DateComparator.EqualLess:
                        data = data.Where(t => t.DateOfDownload <= date);
                        break;
                }
            }

            // Sort
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

            var count = data.Count();
            // Pagination
            data = data
                .Skip((filter.PaginationParams.PageNumber - 1) * filter.PaginationParams.PageSize)
                .Take(filter.PaginationParams.PageSize);

            return TypedResults.Ok(new ApiResponse<FilterResponse<Download>>
            {
                Data = new()
                {
                    Data = await data.ToListAsync(cancellationToken: ct),
                    TotalPages = (int)Math.Ceiling((double)count / filter.PaginationParams.PageSize)
                }
            });
        }

        // ------------ Create a new download --------- //
        static async Task<Results<Ok<ApiResponse<string>>, NotFound<ApiResponse>, BadRequest<ApiResponse>>>
            PostDownloadDocument(
                ClaimsPrincipal user,
                DownloadCreatedDto dto, ValidateUtils vu,
                IBlobService bs, AicaDocsDb db, CancellationToken ct)
        {
            var validation = !await vu.ValidateNomenclatorId(dto.ReasonId, TypeOfNomenclator.ReasonOfDownload, ct);
            if (validation)
                return TypedResults.BadRequest(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 400, Errors = new Dictionary<string, string[]>
                        {
                            { "ReasonId", ["Reason Id must be valid"] }
                        }
                    }
                });

            var doc = await db.Documents.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == dto.DocumentId, cancellationToken: ct);

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

            var exists = await bs.ValidateExistanceObject($"/{format}/{doc.Code + doc.Edition}.{ext}", ct);
            if (!exists)
                return TypedResults.NotFound(new ApiResponse
                {
                    ProblemDetails = new()
                    {
                        Status = 404, Errors = new Dictionary<string, string[]>
                        {
                            { "Document Existance", [$"Doesn`t exist a file document with the given specifications."] }
                        }
                    }
                });

            var url = await bs.PresignedGetUrl($"/{format}/{doc.Code + doc.Edition}.{ext}", ct);

            db.Downloads.Add(new Download
            {
                Format = dto.Format,
                Username = user.Identity!.Name!,
                DocumentId = dto.DocumentId,
                ReasonId = dto.ReasonId
            });
            await db.SaveChangesAsync(ct);

            return TypedResults.Ok(new ApiResponse<string> { Data = url });
        }
    }
}