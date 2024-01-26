using Microsoft.AspNetCore.Mvc;

namespace AicaDocsApi.Dto;

public class ApiResponse
{
    public ProblemDetails? ProblemDetails { get; set; }
}

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public ProblemDetails? Error { get; set; }
}