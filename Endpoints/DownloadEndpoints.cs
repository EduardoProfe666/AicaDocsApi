using AicaDocsApi.Database;
using AicaDocsApi.Dto.Downloads.Filter;
using AicaDocsApi.Dto.FilterCommons;
using AicaDocsApi.Dto.Nomenclators.Filter;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Validators;
using AicaDocsApi.Validators.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        
        
        static async Task<Results<Ok<ApiResponse<Download>>, NotFound<ApiResponse>>> GetDownloadById(int id,
            AicaDocsDb db,
            CancellationToken ct)
        {
            var dl = await db.Downloads.FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);

            if (dl is null)
            {
                return TypedResults.NotFound(new ApiResponse()
                {
                    ProblemDetails = new ProblemDetails()
                    {
                        Status = 404, Detail = "Doesn`t exist a download with the given id"
                    }
                });
            }

            return TypedResults.Ok(new ApiResponse<Download>()
            {
                Data = dl
            });
        }
        
        static async Task<Results<Ok<ApiResponse<IEnumerable<Download>>>, ValidationProblem, BadRequest<ApiResponse>>>
            FilterDownload(
                FilterDownloadDto filter,
                AicaDocsDb db, CancellationToken ct)
        {
            
            if (filter.ReasonId is not null && !await ValidateUtils.ValidateNomenclatorId(filter.ReasonId, TypeOfNomenclator.ReasonOfDownload, db, ct))
                return TypedResults.BadRequest(new ApiResponse()
                    { ProblemDetails = new() { Status = 400, Detail = "Download reason must be valid" } });
            
            var data = db.Downloads.Where(a => true);
            if (filter.Format is not null)
                data = data.Where(t => t.Format == filter.Format);
            if (filter.Username is not null)
                data = data.Where(t => t.Username.Contains(filter.Username.Trim()));
            if (filter.DocumentId is not null)
                data = data.Where(t => t.DocumentId == filter.DocumentId);
            if (filter.ReasonId is not null)
                data = data.Where(t => t.ReasonId == filter.ReasonId);
            if(filter.DateDownload is not null)
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