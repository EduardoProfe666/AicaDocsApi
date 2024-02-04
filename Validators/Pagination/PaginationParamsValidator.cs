using AicaDocsApi.Dto;
using FluentValidation;

namespace AicaDocsApi.Validators.Pagination;

public class PaginationParamsValidator: AbstractValidator<PaginationParams>
{
    public PaginationParamsValidator()
    {
        RuleFor(e => e.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page Number must be greater than 0");
        RuleFor(e => e.PageSize)
            .GreaterThan(0)
            .WithMessage("Page Size must be greater than 0");
    }
}