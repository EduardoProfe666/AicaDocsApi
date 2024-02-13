using AicaDocsApi.Database;
using AicaDocsApi.Dto.Documents;
using AicaDocsApi.Dto.Documents.Filter;
using AicaDocsApi.Dto.FilterCommons;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Utils.BlobServices;
using AicaDocsApi.Validators.Commons;
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
            .WithTags(["Documents"]);

        // ToDo: Quitar este endpoint en despliegue
        group.MapGet("", async (AicaDocsDb db, CancellationToken ct) => await db.Documents.ToListAsync(ct))
            .WithSummary("ONLY FOR TESTING. Get all documents");

        group.MapPost("/filter", FilterDocument)
            .WithSummary("Get documents with specific filters, sorts and pagination")
            .AddEndpointFilter<ValidationFilter<FilterDocumentDto>>();


        group.MapGet("/{id:int}", GetDocumentById)
            .WithSummary("Get the document with the given id");

        group.MapPost("", PostDocument)
            .WithSummary("Create a new document")
            .AddEndpointFilter<ValidationFilter<DocumentCreatedDto>>()
            .DisableAntiforgery();

        static async Task<Results<Ok<ApiResponse<Document>>, NotFound<ApiResponse>>> GetDocumentById(int id,
            AicaDocsDb db,
            CancellationToken ct)
        {
            var doc = await db.Documents.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);

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

            return TypedResults.Ok(new ApiResponse<Document>()
            {
                Data = doc
            });
        }

        static async Task<Results<Created, BadRequest<ApiResponse>>> PostDocument(
            [FromForm] DocumentCreatedDto doc,
            AicaDocsDb db,
            ValidateUtils vu,
            IBlobService bs,
            CancellationToken ct)
        {
            var validation = await db.Documents.AsNoTracking().FirstOrDefaultAsync(
                a => a.Code + a.Edition == doc.Code + doc.Edition,
                cancellationToken: ct) is not null;
            if (validation)
                return TypedResults.BadRequest(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 400, Errors = new Dictionary<string, string[]>
                        {
                            { "Code-Edition", ["Code+Edition must be unique"] }
                        }
                    }
                });

            var errorMessages = new List<string>();
            validation = !await vu.ValidateNomenclatorId(doc.TypeId, TypeOfNomenclator.TypeOfDocument, ct);
            if (validation)
                errorMessages.Add("Type of document must be valid");

            validation = !await vu.ValidateNomenclatorId(doc.ProcessId, TypeOfNomenclator.ProcessOfDocument, ct);
            if (validation)
                errorMessages.Add("Process of document must be valid");

            validation = !await vu.ValidateNomenclatorId(doc.ScopeId, TypeOfNomenclator.ScopeOfDocument, ct);
            if (validation)
                errorMessages.Add("Scope of document must be valid");

            if (errorMessages.Count > 0)
                return TypedResults.BadRequest(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 400, Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclators", errorMessages.ToArray() }
                        }
                    }
                });

            var fileName = doc.Code + doc.Edition;


            await bs.UploadObject(doc.Pdf, fileName, ct);
            await bs.UploadObject(doc.Word, fileName, ct);

            db.Documents.Add(doc.ToNewDocument());
            await db.SaveChangesAsync(ct);

            return TypedResults.Created();
        }


        static async Task<Results<Ok<ApiResponse<IEnumerable<Document>>>, BadRequest<ApiResponse>>>
            FilterDocument(
                FilterDocumentDto filter,
                ValidateUtils vu,
                AicaDocsDb db, CancellationToken ct)
        {
            // Prev Validations
            var errorMessages = new List<string>();
            var validation = filter.TypeId is not null &&
                             !await vu.ValidateNomenclatorId(filter.TypeId, TypeOfNomenclator.TypeOfDocument, ct);
            if (validation)
                errorMessages.Add("Type of document must be valid");

            validation = filter.ProcessId is not null &&
                         !await vu.ValidateNomenclatorId(filter.ProcessId, TypeOfNomenclator.ProcessOfDocument, ct);
            if (validation)
                errorMessages.Add("Process of document must be valid");

            validation = filter.ScopeId is not null &&
                         !await vu.ValidateNomenclatorId(filter.ScopeId, TypeOfNomenclator.ScopeOfDocument, ct);
            if (validation)
                errorMessages.Add("Scope of document must be valid");

            if (errorMessages.Count > 0)
                return TypedResults.BadRequest(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 400, Errors = new Dictionary<string, string[]>
                        {
                            { "Nomenclators", errorMessages.ToArray() }
                        }
                    }
                });

            // Filter
            var data = db.Documents.AsNoTracking();
            if (filter.Code is not null)
                data = data.Where(t => t.Code.Contains(filter.Code.Trim()));
            if (filter.Title is not null)
                data = data.Where(t => t.Title.Contains(filter.Title.Trim()));
            if (filter.Edition is not null)
                data = data.Where(t => t.Edition == filter.Edition);
            if (filter.Pages is not null)
                data = data.Where(t => t.Pages == filter.Pages);
            if (filter.TypeId is not null)
                data = data.Where(t => t.TypeId == filter.TypeId);
            if (filter.ProcessId is not null)
                data = data.Where(t => t.ProcessId == filter.ProcessId);
            if (filter.ScopeId is not null)
                data = data.Where(t => t.ScopeId == filter.ScopeId);
            if (filter.DateOfValidity is not null)
                switch (filter.DateComparator)
                {
                    case DateComparator.Equal:
                        data = data.Where(t => t.DateOfValidity == filter.DateOfValidity);
                        break;
                    case DateComparator.Greater:
                        data = data.Where(t => t.DateOfValidity > filter.DateOfValidity);
                        break;
                    case DateComparator.Less:
                        data = data.Where(t => t.DateOfValidity < filter.DateOfValidity);
                        break;
                    case DateComparator.EqualGreater:
                        data = data.Where(t => t.DateOfValidity >= filter.DateOfValidity);
                        break;
                    case DateComparator.EqualLess:
                        data = data.Where(t => t.DateOfValidity <= filter.DateOfValidity);
                        break;
                }

            // Sort 
            switch (filter.SortBy)
            {
                case SortByDocument.Code:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Code)
                        : data.OrderByDescending(t => t.Code);
                    break;
                case SortByDocument.Title:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Title)
                        : data.OrderByDescending(t => t.Title);
                    break;
                case SortByDocument.Edition:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Edition)
                        : data.OrderByDescending(t => t.Edition);
                    break;
                case SortByDocument.Id:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Id)
                        : data.OrderBy(t => t.Id);
                    break;
                case SortByDocument.Pages:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.Pages)
                        : data.OrderBy(t => t.Pages);
                    break;
                case SortByDocument.DateOfValidity:
                    data = filter.SortOrder == SortOrder.Asc
                        ? data.OrderBy(t => t.DateOfValidity)
                        : data.OrderBy(t => t.DateOfValidity);
                    break;
            }

            // Pagination
            data = data
                .Skip((filter.PaginationParams.PageNumber - 1) * filter.PaginationParams.PageSize)
                .Take(filter.PaginationParams.PageSize);

            return TypedResults.Ok(new ApiResponse<IEnumerable<Document>>
            {
                Data = await data.ToListAsync(cancellationToken: ct)
            });
        }
    }
}
