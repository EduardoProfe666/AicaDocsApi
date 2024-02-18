using AicaDocsApi.Database;
using AicaDocsApi.Dto.PaginationCommons;
using AicaDocsApi.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Endpoints;

public static class PaginationEndpoints
{
    public static void MapPaginationEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/pagination/")
            .WithTags(["Pagination"])
            .WithOpenApi();

        // ---------------- Endpoint Declarations --------------------//
        group.MapGet("/pages/{type}/{pageSize:int}", GetTotalPages)
            .WithSummary("Get the total pages of specific Data Set by the page size")
            .WithDescription("""
                             This endpoint allows you to get the total pages of an specific Data Set **type**
                             with the given **pageSize**.

                             The **pageSize** must be greater than 0.
                             
                             The valid types of Data Sets (**type**) are:
                             - **0** -> Documents
                             - **1** -> Downloads
                             """);

        // -------------------------- Endpoints Functions ---------------------------------- //

        // --------- Get Total Pages --------//
        static async Task<Results<Ok<ApiResponse<int>>, BadRequest<ApiResponse>>> GetTotalPages(short type,
            int pageSize, AicaDocsDb db, CancellationToken ct)
        {
            var errorMessages = new List<string>();
            if (!Enum.IsDefined(typeof(TypeOfDataSet), type))
                errorMessages.Add("Type of Model must be valid");
            if (pageSize < 1)
                errorMessages.Add("Page Size must be greater than 0");

            if (errorMessages.Count > 0)
                return TypedResults.BadRequest(new ApiResponse()
                {
                    ProblemDetails = new()
                    {
                        Status = 400, Errors = new Dictionary<string, string[]>
                        {
                            { "Pages Total Errors", errorMessages.ToArray() }
                        }
                    }
                });
            var total = 0;
            switch ((TypeOfDataSet)type)
            {
                case TypeOfDataSet.Documents:
                    total = await db.Documents.AsNoTracking().CountAsync(cancellationToken: ct);
                    break;
                case TypeOfDataSet.Downloads:
                    total = await db.Downloads.AsNoTracking().CountAsync(cancellationToken: ct);
                    break;
            }

            return TypedResults.Ok(new ApiResponse<int> { Data = (int)Math.Ceiling((double)total / pageSize) });
        }
    }
}