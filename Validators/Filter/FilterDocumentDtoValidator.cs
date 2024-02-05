using AicaDocsApi.Dto.Documents.Filter;
using FluentValidation;

namespace AicaDocsApi.Validators.Filter;

public class FilterDocumentDtoValidator : AbstractValidator<FilterDocumentDto>
{
    public FilterDocumentDtoValidator()
    {
        RuleFor(e => e.Title)
            .MaximumLength(64)
            .WithMessage("Title must have a maximum length of 64 caracters");

        RuleFor(e => e.Code)
            .MaximumLength(64)
            .WithMessage("Code must have a maximum length of 64 caracters");

        RuleFor(e => e.Edition)
            .GreaterThan((short)0)
            .WithMessage("Edition must be greater than 0");

        RuleFor(e => e.Pages)
            .GreaterThan((short)0)
            .WithMessage("Pages must be greater than 0");

        RuleFor(e => e.DateOfValidity)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("Date of validity must be less or equal than today`s day");

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