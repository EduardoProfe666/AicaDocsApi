using AicaDocsApi.Dto.Documents;
using FluentValidation;

namespace AicaDocsApi.Validators.Document;

public class DocumentCreatedDtoValidator : AbstractValidator<DocumentCreatedDto>
{
    public DocumentCreatedDtoValidator()
    {
        RuleFor(e => e.Title)
            .MaximumLength(64)
            .WithMessage("Title must have a maximum length of 64 caracters")
            .NotEmpty()
            .WithMessage("Title must not be empty");
        
        RuleFor(e => e.Code)
            .MaximumLength(64)
            .WithMessage("Code must have a maximum length of 64 caracters")
            .NotEmpty()
            .WithMessage("Code must not be empty");

        RuleFor(e => e.Edition)
            .GreaterThan(e => 0)
            .WithMessage("Edition must be greater than 0");
        
        RuleFor(e => e.Pages)
            .GreaterThan(e => 0)
            .WithMessage("Pages must be greater than 0");

        RuleFor(e => e.DateOfValidity)
            .LessThanOrEqualTo(e => DateTimeOffset.UtcNow)
            .WithMessage("Date of validity must be less or equal than today`s day");















    }
}