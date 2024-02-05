using AicaDocsApi.Dto.Documents.Filter;
using FluentValidation;

namespace AicaDocsApi.Validators.Documents;

public class FilterDocumentDtoValidator : AbstractValidator<FilterDocumentDto>
{
    public FilterDocumentDtoValidator()
    {
        RuleFor(e => e.Title)
            .MaximumLength(64)
            .WithMessage("Title must have a maximum length of 64 characters");

        RuleFor(e => e.Code)
            .MaximumLength(64)
            .WithMessage("Code must have a maximum length of 64 characters");

        RuleFor(e => e.Edition)
            .ExclusiveBetween(0,32001)
            .WithMessage("Edition must be greater than 0 and less than 32001");

        RuleFor(e => e.Pages)
            .ExclusiveBetween(0,32001)
            .WithMessage("Pages must be greater than 0 and less than 32001");

        RuleFor(e => e.TypeId)
            .GreaterThan(0)
            .WithMessage("Type id must be greater than 0");

        RuleFor(e => e.ProcessId)
            .GreaterThan(0)
            .WithMessage("Process id must be greater than 0");

        RuleFor(e => e.ScopeId)
            .GreaterThan(0)
            .WithMessage("Scope id must be greater than 0");
          
          
        RuleFor(e => e.SortBy)
            .IsInEnum()
            .WithMessage("SortBy of download must be valid")
            .NotNull()
            .WithMessage("SortBy cannot be null");
        
        RuleFor(e => e.SortOrder)
            .IsInEnum()
            .WithMessage("SortOrder of download must be valid")
            .NotNull()
            .WithMessage("SortOrder cannot be null");

        RuleFor(e => e.PaginationParams)
            .NotNull()
            .WithMessage("Pagination Params cannot be null");
        
        RuleFor(e => e.PaginationParams.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page Number of Pagination Params must be greater than 0")
            .NotNull()
            .WithMessage("Page Number of Pagination Params cannot be null");
        
        RuleFor(e => e.PaginationParams.PageSize)
            .GreaterThan(0)
            .WithMessage("Page Size of Pagination Params must be greater than 0")
            .NotNull()
            .WithMessage("Page Size of Pagination Params cannot be null");

    }
}