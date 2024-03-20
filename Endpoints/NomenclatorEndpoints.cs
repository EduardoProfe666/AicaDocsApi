using AicaDocsApi.Database;
using AicaDocsApi.Dto.Nomenclators;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
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
        
        // ---------------- Endpoint Declarations --------------------//
        
        group.MapGet("/{type}", GetAllNomenclatorByType)
            .WithSummary("Get all the nomenclators with the given type")
            .WithDescription("""
                             This endpoint allows you to get all the nomenclators with the given **type**.

                             The valid types of nomenclators (**type**) are:
                             - **0** -> Process of Document
                             - **1** -> Reason of Download
                             - **2** -> Scope of Document
                             - **3** -> Type of Document
                             """);
        
        group.MapGet("/{type}/{id:int}", GetAllNomenclatorByTypeAndId)
            .WithSummary("Get the nomenclator with the given type and id")
            .WithDescription("""
                             This endpoint allows you to get the nomenclator with the given **type** and **id**.

                             The valid types of nomenclators (**type**) are:
                             - **0** -> Process of Document
                             - **1** -> Reason of Download
                             - **2** -> Scope of Document
                             - **3** -> Type of Document
                             """);

        group.MapPost("", PostNomenclator)
            .WithSummary("Create a new Nomenclator")
            .AddEndpointFilter<ValidationFilter<NomenclatorCreatedDto>>()
            .WithDescription("""
                             This endpoint allows you to create a new nomenclator with
                             the given **type** and **name**.

                             The valid types of nomenclators (**type**) are:
                             - **0** -> Process of Document
                             - **1** -> Reason of Download
                             - **2** -> Scope of Document
                             - **3** -> Type of Document
                             """);
        
        group.MapDelete("/{type}/{id:int}", DeleteNomenclator)
            .WithSummary("Delete a nomenclator with the given type and id")
            .WithDescription("""
                             This endpoint allows you to delete a nomenclator with the given **type** and **id**
                             
                             The valid types of nomenclators (**type**) are:
                             - **0** -> Process of Document
                             - **1** -> Reason of Download
                             - **2** -> Scope of Document
                             - **3** -> Type of Document
                             """);
        
        group.MapPut("/{id:int}", PutNomenclator)
            .WithSummary("Edit the name of the nomenclator with the given id")
            .AddEndpointFilter<ValidationFilter<NomenclatorPutDto>>()
            .WithDescription("""
                             This endpoint allows you to edit the **name** of the nomenclator with
                             the given **id**.
                             """);

        // -------------------------- Endpoints Functions ---------------------------------- //
        
        // --------- Get Nomenclators by type --------//
        static async Task<Results<Ok<ApiResponse<IEnumerable<Nomenclator>>>, BadRequest<ApiResponse>>>
            GetAllNomenclatorByType(short type, AicaDocsDb db, CancellationToken ct)
        {
            if (!Enum.IsDefined(typeof(TypeOfNomenclator), type))
                return TypedResults.BadRequest(new ApiResponse
                {
                    ProblemDetails = new ProblemDetails
                    {
                        Status = 400,
                        Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclator Type", ["Type of Nomenclator must be valid"] }
                        }
                    }
                });
            return TypedResults.Ok(new ApiResponse<IEnumerable<Nomenclator>>
            {
                Data = await db.Nomenclators.AsNoTracking().Where(o => o.Type == (TypeOfNomenclator)type)
                    .ToListAsync(ct)
            });
        }
        
        // --------- Get nomenclator by type and id --------//
        static async Task<Results<Ok<ApiResponse<Nomenclator>>, BadRequest<ApiResponse>,
                NotFound<ApiResponse>>>
            GetAllNomenclatorByTypeAndId(short type, int id, AicaDocsDb db, CancellationToken ct)
        {
            if (!Enum.IsDefined(typeof(TypeOfNomenclator), type))

                return TypedResults.BadRequest(new ApiResponse
                {
                    ProblemDetails = new ProblemDetails
                    {
                        Status = 400,
                        Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclator Type", ["Type of Nomenclator must be valid"] }
                        }
                    }
                });

            var data = await db.Nomenclators.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.Type == (TypeOfNomenclator)type, ct);
            if (data is null)
                return TypedResults.NotFound(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 404,
                        Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclator Id | Type", ["Doesn't exist a nomenclator with the given Id and Type"] }
                        }
                    }
                });

            return TypedResults.Ok(new ApiResponse<Nomenclator>()
            {
                Data = data
            });
        }

        // --------- Create a new nomenclator --------- //
        static async Task<Results<Created, BadRequest<ApiResponse>>> PostNomenclator(NomenclatorCreatedDto nomenclator,
            AicaDocsDb db, CancellationToken ct)
        {
            await db.Nomenclators.AddAsync(new Nomenclator() { Type = nomenclator.Type, Name = nomenclator.Name.Trim() },
                ct);
            await db.SaveChangesAsync(ct);
            return TypedResults.Created();
        }
        
        // -------- Delete a nomenclator --------- //
        static async Task<Results<Ok, BadRequest<ApiResponse>, NotFound<ApiResponse>>> DeleteNomenclator(short type, int id, AicaDocsDb db,
            CancellationToken ct)
        {
            if (!Enum.IsDefined(typeof(TypeOfNomenclator), type))

                return TypedResults.BadRequest(new ApiResponse
                {
                    ProblemDetails = new ProblemDetails
                    {
                        Status = 400,
                        Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclator Type", ["Type of Nomenclator must be valid"] }
                        }
                    }
                });

            var data = await db.Nomenclators.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.Type == (TypeOfNomenclator)type, ct);
            if (data is null)
                return TypedResults.NotFound(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 404,
                        Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclator Id | Type", ["Doesn't exist a nomenclator with the given Id and Type"] }
                        }
                    }
                });

            var error = false;
            switch((TypeOfNomenclator)type)
            {
                case TypeOfNomenclator.ProcessOfDocument:
                    error = (await db.Documents.AsNoTracking().FirstOrDefaultAsync(a=>a.ProcessId==id,cancellationToken: ct)) is not null ;
                    break;
                case TypeOfNomenclator.ReasonOfDownload:
                    error = (await db.Downloads.AsNoTracking().FirstOrDefaultAsync(a=>a.ReasonId==id,cancellationToken: ct)) is not null ;
                    break;
                
                case TypeOfNomenclator.ScopeOfDocument:
                    error = (await db.Documents.AsNoTracking().FirstOrDefaultAsync(a=>a.ScopeId==id,cancellationToken: ct)) is not null ;
                    break;
                
                case TypeOfNomenclator.TypeOfDocument:
                    error = (await db.Documents.AsNoTracking().FirstOrDefaultAsync(a=>a.TypeId==id,cancellationToken: ct)) is not null ;
                    break;
            }

            if (error)
            {
                return TypedResults.BadRequest(new ApiResponse
                {
                    ProblemDetails = new ProblemDetails
                    {
                        Status = 400,
                        Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclator", ["Nomenclator selected is being used"] }
                        }
                    }
                });
            }

            db.Nomenclators.Remove(data);
            await db.SaveChangesAsync(ct);
            return TypedResults.Ok();
        }

        // ---------- Put name of nomenclator ---------- //
        static async Task<Results<Ok, NotFound<ApiResponse>, BadRequest<ApiResponse>>> PutNomenclator(int id,
            NomenclatorPutDto nomenclator,
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

            data.Name = nomenclator.Name.Trim();
            await db.SaveChangesAsync(ct);

            return TypedResults.Ok();
        }
    }
}