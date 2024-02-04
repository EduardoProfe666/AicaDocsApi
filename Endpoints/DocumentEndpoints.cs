using AicaDocsApi.Database;
using AicaDocsApi.Dto;
using AicaDocsApi.Dto.Documents;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Validators;
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
        group.MapGet("", GetDocuments)
            .WithSummary("ONLY FOR TESTING. Get all documents");

        group.MapPost("", PostDocument)
            .WithSummary("Create a new document")
            .AddEndpointFilter<ValidationFilter<DocumentCreatedDto>>();

        static async Task<Results<Ok<ApiResponse<DocumentViewDto>>, NotFound<ApiResponse>> GetDocumentById(int id, AicaDocsDb db,
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

            return TypedResults.Ok(new ApiResponse<DocumentViewDto>()
            {
                Data = new DocumentViewDto(doc)
            });
        }

        static async Task<Ok<ApiResponse<IEnumerable<DocumentViewDto>>>> GetDocuments(AicaDocsDb db, CancellationToken ct)
        {
            var dataReturn = new List<DocumentViewDto>();
            (await db.Documents.ToListAsync(ct))
                .ForEach(document => dataReturn.Add(new DocumentViewDto(document)));
            
            return TypedResults.Ok(new ApiResponse<IEnumerable<DocumentViewDto>>
            {
                Data = dataReturn
            });   
        }

        static async Task<Results<Created, BadRequest>> PostDocument(DocumentCreatedDto doc, AicaDocsDb db,
            CancellationToken cancellationToken)
        {
            db.Documents.Add(doc.ToNewDocument());
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created();
        }
        
    }
}