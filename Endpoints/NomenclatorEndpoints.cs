using AicaDocsApi.Database;
using AicaDocsApi.Dto.FilterCommons;
using AicaDocsApi.Dto.Nomenclators;
using AicaDocsApi.Dto.Nomenclators.Filter;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Validators;
using AicaDocsApi.Validators.Commons;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Endpoints;

public static class NomenclatorEndpoints
{
    public static void MapNomenclatorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/nomenclator")
            .WithOpenApi()
            .WithTags(["Nomenclators"]);

        // ToDo: Quitar en despliegue
        group.MapGet("", async (AicaDocsDb db, CancellationToken ct) => await db.Nomenclators.ToListAsync(ct))
            .WithSummary("ONLY FOR TESTING. Get all nomenclators");

        group.MapPost("/filter", FilterNomenclator)
            .WithSummary("Get nomenclators with specific filters, sorts and pagination")
            .AddEndpointFilter<ValidationFilter<FilterNomenclatorDto>>();

        group.MapGet("/{id:int}", GetNomenclatorById)
            .WithSummary("Get the nomenclator with the given id");

        group.MapPost("", PostNomenclator)
            .WithSummary("Create a new Nomenclator")
            .AddEndpointFilter<ValidationFilter<NomenclatorCreatedDto>>();

        group.MapPatch("/{id:int}", PatchNomenclator)
            .WithSummary("Updates the name of the nomenclator with the given id")
            .AddEndpointFilter<ValidationFilter<NomenclatorUpdateDto>>();


        static async Task<Results<Ok<ApiResponse<IEnumerable<Nomenclator>>>, BadRequest<ApiResponse>>>
            FilterNomenclator(
                FilterNomenclatorDto filter,
                AicaDocsDb db, CancellationToken ct)
        {

            var data = db.Nomenclators.AsNoTracking();
            if (filter.Type is not null)
                data = data.Where(a => a.Type == filter.Type);
            
            switch (filter.SortBy)
            {
                case SortByNomenclator.Id:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Id)
                        : data.OrderByDescending(t => t.Id);
                    break;
                case SortByNomenclator.Name:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Name)
                        : data.OrderByDescending(t => t.Name);
                    break;
                case SortByNomenclator.Type:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Type)
                        : data.OrderByDescending(t => t.Type);
                    break;
            }

            data = data
                .Skip((filter.PaginationParams.PageNumber - 1) * filter.PaginationParams.PageSize)
                .Take(filter.PaginationParams.PageSize);

            return TypedResults.Ok(new ApiResponse<IEnumerable<Nomenclator>>
            {
                Data = await data.ToListAsync(cancellationToken: ct)
            });
        }

        static async Task<Results<Ok<ApiResponse<Nomenclator>>, NotFound<ApiResponse>>> GetNomenclatorById(
            int id, AicaDocsDb db, CancellationToken ct)
        {
            var data = await db.Nomenclators.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);
            if (data is null)
                return TypedResults.NotFound(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 404,
                        Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclator Id", ["Doesn't exist a nomenclator with the given Id"] }
                        }
                    }
                });
            return TypedResults.Ok(new ApiResponse<Nomenclator>()
            {
                Data = data
            });
        }

        static async Task<Results<Created, BadRequest<ApiResponse>>> PostNomenclator(NomenclatorCreatedDto nomenclator,
            AicaDocsDb db, CancellationToken ct)
        {
            await db.Nomenclators.AddAsync(new Nomenclator() { Type = nomenclator.Type, Name = nomenclator.Name },
                ct);
            await db.SaveChangesAsync(ct);
            return TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound<ApiResponse>, BadRequest<ApiResponse>>> PatchNomenclator(int id,
            NomenclatorUpdateDto nomenclator,
            AicaDocsDb db, CancellationToken ct)
        {
            var data = await db.Nomenclators.AsQueryable().FirstOrDefaultAsync(a => a.Id == id, cancellationToken: ct);
            if (data is null)
                return TypedResults.NotFound(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 404,
                        Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclator Id", ["Doesn't exist a nomenclator with the given Id"] }
                        }
                    }
                });

            data.Name = nomenclator.Name;
            await db.SaveChangesAsync(ct);

            return TypedResults.Ok();
        }
    }
}