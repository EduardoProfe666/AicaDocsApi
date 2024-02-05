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
            .GreaterThan((short)0)
            .WithMessage("Edition must be greater than 0")
            .NotNull()
            .WithMessage("Edition cannot be null");
        
        RuleFor(e => e.Pages)
            .GreaterThan((short)0)
            .WithMessage("Pages must be greater than 0")
            .NotNull()
            .WithMessage("Pages cannot be null");

        RuleFor(e => e.DateOfValidity)
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("Date of validity must be less or equal than today`s day")
            .NotNull()
            .WithMessage("Date of validity cannot be null");
        
        RuleFor(e => e.TypeId)
            .GreaterThan(0)
            .WithMessage("Type id must be greater than 0")
            .NotNull()
            .WithMessage("Type id cannot be null");
        
        RuleFor(e => e.ProcessId)
            .GreaterThan(0)
            .WithMessage("Process id must be greater than 0")
            .NotNull()
            .WithMessage("Process id cannot be null");
        
        RuleFor(e => e.ScopeId)
            .GreaterThan(0)
            .WithMessage("Scope id must be greater than 0")
            .NotNull()
            .WithMessage("Scope id cannot be null");














    }
}