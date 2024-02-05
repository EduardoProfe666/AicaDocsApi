using AicaDocsApi.Responses;
using FluentValidation;

namespace AicaDocsApi.Validators.Commons;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var validator = ctx.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null) return await next(ctx);

        var entity = ctx.Arguments
            .OfType<T>()
            .FirstOrDefault(a => a?.GetType() == typeof(T));
        if (entity is null)
            return TypedResults.BadRequest(new ApiResponse
            {
                ProblemDetails = new()
                {
                    Status = 400, Errors = new Dictionary<string, string[]>
                    {
                        { "Validation Type Filters", ["Doesn't exist the type to validate"] }
                    }
                }
            });

        var validation = await validator.ValidateAsync(entity);
        if (validation.IsValid)
        {
            return await next(ctx);
        }

        return TypedResults.BadRequest(new ApiResponse
        {
            ProblemDetails = new()
            {
                Status = 400, Errors = validation.ToDictionary()
            }
        });
    }
}