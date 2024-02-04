using AicaDocsApi.Dto.Nomenclators.Filter;
using FluentValidation;

namespace AicaDocsApi.Validators.Filter;

public class FilterNomenclatorDtoValidator: AbstractValidator<FilterNomenclatorDto>
{
    public FilterNomenclatorDtoValidator()
    {
        RuleFor(a => a.Type)
            .IsInEnum()
            .WithMessage("Type of nomenclator must be valid");
        
        RuleFor(e => e.SortBy)
            .IsInEnum()
            .WithMessage("SortBy of nomenclator must be valid")
            .NotNull()
            .WithMessage("SortBy cannot be null");
        
        RuleFor(e => e.SortOrder)
            .IsInEnum()
            .WithMessage("SortOrder of nomenclator must be valid")
            .NotNull()
            .WithMessage("SortOrder cannot be null");
        
        RuleFor(e => e.PaginationParams)
            .NotNull()
            .WithMessage("Pagination Params cannot be null")
            .NotNull()
            .WithMessage("Pagination Params cannot be null");
        
        RuleFor(e => e.PaginationParams.PageNumber)
            .GreaterThan((short)0)
            .WithMessage("Page Number of Pagination Params must be greater than 0")
            .NotNull()
            .WithMessage("Page Number of Pagination Params cannot be null");
        
        RuleFor(e => e.PaginationParams.PageSize)
            .GreaterThan((short)0)
            .WithMessage("Page Size of Pagination Params must be greater than 0")
            .NotNull()
            .WithMessage("Page Size of Pagination Params cannot be null");
    }
}