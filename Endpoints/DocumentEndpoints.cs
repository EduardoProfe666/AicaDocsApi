using AicaDocsApi.Database;
using AicaDocsApi.Dto.Documents;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Validators;
using AicaDocsApi.Validators.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Endpoints;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/document")
            .WithOpenApi()
            .WithTags(["Document"]);

        // ToDo: Quitar este endpoint en despliegue
        group.MapGet("", async (AicaDocsDb db, CancellationToken ct) => await db.Documents.ToListAsync(ct))
            .WithSummary("ONLY FOR TESTING. Get all documents");

        group.MapPost("", PostDocument)
            .WithSummary("Create a new document")
            .AddEndpointFilter<ValidationFilter<DocumentCreatedDto>>();

        group.MapGet("/{id:int}", GetDocumentById)
            .WithSummary("Get the document with th given id");

        static async Task<Results<Ok<ApiResponse<Document>>, NotFound<ApiResponse>>> GetDocumentById(int id,
            AicaDocsDb db,
            CancellationToken ct)
        {
            var doc = await db.Documents.FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);

            if (doc is null)
            {
                return TypedResults.NotFound(new ApiResponse()
                {
                    ProblemDetails = new ProblemDetails()
                    {
                        Status = 404, Detail = "Doesn`t exist a document with the given id"
                    }
                });
            }

            return TypedResults.Ok(new ApiResponse<Document>()
            {
                Data = doc
            });
        }

        static async Task<Results<Created, BadRequest<ApiResponse>, ValidationProblem>> PostDocument(DocumentCreatedDto doc,
            AicaDocsDb db,
            CancellationToken ct)
        {
            if (!await ValidateUtils.ValidateNomenclatorId(doc.ProcessId, TypeOfNomenclator.ProcessOfDocument, db, ct))
                return TypedResults.BadRequest(new ApiResponse()
                    { ProblemDetails = new() { Status = 400, Detail = "Document Process must be valid" } });
            
            if (!await ValidateUtils.ValidateNomenclatorId(doc.ScopeId, TypeOfNomenclator.ScopeOfDocument, db, ct))
                return TypedResults.BadRequest(new ApiResponse()
                    { ProblemDetails = new() { Status = 400, Detail = "Document Scope must be valid" } });
            
            if (!await ValidateUtils.ValidateNomenclatorId(doc.TypeId, TypeOfNomenclator.TypeOfDocument, db, ct))
                return TypedResults.BadRequest(new ApiResponse()
                    { ProblemDetails = new() { Status = 400, Detail = "Document Type must be valid" } });
            
            db.Documents.Add(doc.ToNewDocument());
            await db.SaveChangesAsync(ct);

            return TypedResults.Created();
        }
    }
}