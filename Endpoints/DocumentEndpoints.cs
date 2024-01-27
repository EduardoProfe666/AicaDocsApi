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
            .WithSummary("Get all documents");

        group.MapPost("", PostDocument)
            .WithSummary("Create a new document");

        group.MapPut("/{id:int}", PutDocument)
            .WithSummary("Update an specific document");

        static async Task<Ok<ApiResponse<IEnumerable<Document>>>> GetDocuments(DocumentDb db, CancellationToken ct)
        {
            var data = await db.Documents.ToListAsync(ct);
            return TypedResults.Ok(new ApiResponse<IEnumerable<Document>>
            {
                Data = data
            });   
        }

        static async Task<Results<Created,BadRequest<ApiResponse>>> PostDocument(DocumentCreatedDto doc, DocumentDb db,
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

        static async Task<Results<NotFound<ApiResponse>,Ok>> PutDocument(int id, DocumentUpdateDto docData, DocumentDb db, CancellationToken ct)
        {
            var doc = await db.Documents.FirstOrDefaultAsync(x => x.Id == id, ct);

            if (doc is null)
                return TypedResults.NotFound(new ApiResponse
                    { ProblemDetails = new() { Detail = "No existe documentación con el id pasado" } });
            
            if (!string.IsNullOrWhiteSpace(docData.Title))
                doc.Title = docData.Title;
            if (DateOnly.FromDateTime(docData.DateOfValidity.DateTime) > DateOnly.FromDateTime(DateTime.UtcNow))
                doc.DateOfValidity = DateOnly.FromDateTime(docData.DateOfValidity.DateTime);
            if (!string.IsNullOrWhiteSpace(docData.Code))
                doc.Code = docData.Code;
            if (docData.Pages > 0)
                doc.Pages = docData.Pages;
            if (docData.Edition > 0)
                doc.Edition = docData.Edition;
            await db.SaveChangesAsync(ct);

            return TypedResults.Ok();
        }
    }
}