
namespace AicaDocsApi.Responses;

public class ApiResponse
{
    public required ProblemDetails? ProblemDetails { get; set; }
}

public class ApiResponse<T>
{
    public required T? Data { get; set; }
}