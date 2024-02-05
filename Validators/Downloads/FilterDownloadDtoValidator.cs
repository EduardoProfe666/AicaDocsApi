using AicaDocsApi.Dto.Downloads.Filter;
using FluentValidation;

namespace AicaDocsApi.Validators.Downloads;

public class FilterDownloadDtoValidator : AbstractValidator<FilterDownloadDto>
{
    public FilterDownloadDtoValidator()
    {
        RuleFor(e => e.Format)
            .IsInEnum()
            .WithMessage("Format must be valid");

        RuleFor(e => e.Username)
            .MaximumLength(64)
            .WithMessage("Username must have a maximum length of 64 characters");

        RuleFor(e => e.DocumentId)
            .GreaterThan(0)
            .WithMessage("Document id must be greater than 0");
        
        RuleFor(e => e.ReasonId)
            .GreaterThan(0)
            .WithMessage("Reason id must be greater than 0");

        RuleFor(e => e.DateDownload)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("Date of download must be less or equal than today`s day");

        
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