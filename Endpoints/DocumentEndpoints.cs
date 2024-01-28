using AicaDocsApi.Database;
using AicaDocsApi.Dto;
using AicaDocsApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Endpoints;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/document")
            .WithOpenApi()
            .WithTags(["Document"]);

        group.MapGet("", GetDocuments)
            .WithSummary("ONLY FOR TESTING. Get all documents");

        group.MapPost("", PostDocument)
            .WithSummary("Create a new document");

        static async Task<Ok<ApiResponse<IEnumerable<Document>>>> GetDocuments(AicaDocsDb db, CancellationToken ct)
        {
            var data = await db.Documents.ToListAsync(ct);
            return TypedResults.Ok(new ApiResponse<IEnumerable<Document>>
            {
                Data = data
            });   
        }

        static async Task<Results<Created,BadRequest<ApiResponse>>> PostDocument(DocumentCreatedDto doc, AicaDocsDb db,
            CancellationToken cancellationToken)
        {
            string? message = default;
            if (doc.Pages <= 0)
                message = "Las páginas deben ser mayor que 0";
            else if (doc.Edition <= 0)
                message = "La edición debe ser mayor que 0";
            else if (string.IsNullOrEmpty(doc.Title))
                message = "El título es requerido";
            else if (DateOnly.FromDateTime(doc.DateOfValidity.DateTime) > DateOnly.FromDateTime(DateTime.UtcNow))
                message = "La fecha no puede ser posterior al día actual";

            if (message is not null)
                return TypedResults.BadRequest(new ApiResponse
                {
                    ProblemDetails = new()
                    {
                        Detail = message
                    }
                });


            db.Documents.Add(doc.ToNewDocument());
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created();
        }
        
    }
}