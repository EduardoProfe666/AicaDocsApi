using AicaDocsApi.Database;
using AicaDocsApi.Dto;
using AicaDocsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Endpoints;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/document")
            .WithOpenApi();

        group.MapGet("", async (DocumentDb db, CancellationToken ct) =>
        {
            var data = await db.Documents.ToListAsync(ct);
            return Results.Ok(new ApiResponse<IEnumerable<Document>>
            {
                Data = data
            });
        });

        group.MapPost("", async (DocumentCreatedDto doc, DocumentDb db, CancellationToken cancellationToken) =>
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
                return Results.BadRequest(new ApiResponse
                {
                    ProblemDetails = new()
                    {
                        Detail = message
                    }
                });


            db.Documents.Add(doc.ToNewDocument());
            await db.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        });

        group.MapPut("/{id:int}", async (int id, DocumentPutDto docData, DocumentDb db, CancellationToken ct) =>
        {
            var doc = await db.Documents.FirstOrDefaultAsync(x => x.Id == id, ct);

            if (doc is null)
                return Results.NotFound(new ApiResponse
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

            return Results.NoContent();
        });
        
    }
}