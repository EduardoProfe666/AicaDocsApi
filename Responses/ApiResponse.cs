using Microsoft.AspNetCore.Mvc;

namespace AicaDocsApi.Responses;

public class ApiResponse
{

    public ProblemDetails? ProblemDetails { get; set; }
}

public class ApiResponse<T>
{
    public T? Data { get; set; }
}