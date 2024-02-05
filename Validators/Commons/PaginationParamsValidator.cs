using AicaDocsApi.Dto.FilterCommons;
using FluentValidation;

namespace AicaDocsApi.Validators.Commons;

public class PaginationParamsValidator : AbstractValidator<PaginationParams>
{
    public PaginationParamsValidator()
    {
        RuleFor(e => e.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page Number of Pagination Params must be greater than 0")
            .NotNull()
            .WithMessage("Page Number of Pagination Params cannot be null");
        
        RuleFor(e => e.PageSize)
            .GreaterThan(0)
            .WithMessage("Page Size of Pagination Params must be greater than 0")
            .NotNull()
            .WithMessage("Page Size of Pagination Params cannot be null");
    }
}