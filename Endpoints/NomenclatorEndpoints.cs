using AicaDocsApi.Database;
using AicaDocsApi.Dto.Nomenclators;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Validators;
using AicaDocsApi.Validators.Nomenclator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Endpoints;

public static class NomenclatorEndpoints
{
    public static void MapNomenclatorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/nomenclator")
            .WithOpenApi()
            .WithTags(["Nomenclator"]);

        group.MapPost("/filter", GetNomenclatorByType)
            .WithSummary("Get nomenclators of an specific type")
            .AddEndpointFilter<ValidationFilter<FilterNomenclatorDto>>();

        group.MapGet("/{id:int}", GetNomenclatorById)
            .WithSummary("Get the nomenclator with the given id");

        group.MapPost("", PostNomenclator)
            .WithSummary("Create a new Nomenclator")
            .AddEndpointFilter<ValidationFilter<NomenclatorCreatedDto>>();

        group.MapPatch("/{id:int}", PatchNomenclator)
            .WithSummary("Updates the name of the nomenclator with the given id")
            .AddEndpointFilter<ValidationFilter<NomenclatorUpdateDto>>();


        static async Task<Results<Ok<ApiResponse<IEnumerable<NomenclatorViewDto>>>, BadRequest<ApiResponse>>>
            GetNomenclatorByType(
                FilterNomenclatorDto type,
                AicaDocsDb db, CancellationToken ct)
        {
            var dataReturn = new List<NomenclatorViewDto>();
            (await db.Nomenclators.Where(a => a.Type == type.Type).ToListAsync(cancellationToken: ct))
                .ForEach(n => dataReturn.Add(new NomenclatorViewDto(n)));
            return TypedResults.Ok(new ApiResponse<IEnumerable<NomenclatorViewDto>>
            {
                Data = dataReturn
            });
        }

        static async Task<Results<Ok<ApiResponse<NomenclatorViewDto>>, NotFound<ApiResponse>>> GetNomenclatorById(
            int id, AicaDocsDb db, CancellationToken ct)
        {
            var data = await db.Nomenclators.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (data is null)
                return TypedResults.NotFound(new ApiResponse()
                {
                    ProblemDetails = new() { Status = 404, Detail = "Doesn't exist a nomenclator with the given Id" }
                });
            return TypedResults.Ok(new ApiResponse<NomenclatorViewDto>()
            {
                Data = new NomenclatorViewDto(data)
            });
        }

        static async Task<Results<Created, BadRequest>> PostNomenclator(NomenclatorCreatedDto nomenclator,
            AicaDocsDb db, CancellationToken ct)
        {
            await db.Nomenclators.AddAsync(new Nomenclator() { Type = nomenclator.Type, Name = nomenclator.Name }, ct);
            await db.SaveChangesAsync(ct);
            return TypedResults.Created();
        }

        static async Task<Results<Ok, NotFound<ApiResponse>, BadRequest>> PatchNomenclator(int id, NomenclatorUpdateDto nomenclator,
            AicaDocsDb db, CancellationToken ct)
        {
            var data = await db.Nomenclators.FirstOrDefaultAsync(a => a.Id == id, cancellationToken: ct);
            if (data is null)
                return TypedResults.NotFound(new ApiResponse(){ProblemDetails = new()
                {
                    Status = 404,
                    Detail = "Doesn't exist a nomenclator with the given Id"
                }});

            data.Name = nomenclator.Name;
            await db.SaveChangesAsync(ct);
            
            return TypedResults.Ok();
        }
    }
}