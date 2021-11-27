using _4oito6.Demonstration.Commons.Extensions;
using FluentValidation;

namespace _4oito6.Demonstration.Person.Domain.Model.Validators
{
    using _4oito6.Demonstration.Domain.Model.Entities;

    public class CreatePersonValidator : AbstractValidator<Person>
    {
        public CreatePersonValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Nome deve ser preenchido.")
                .NotNull().WithMessage("Nome deve ser preenchido.");

            RuleFor(p => p.Email)
                .NotEmpty().WithMessage("E-mail deve ser preenchido.")
                .NotNull().WithMessage("E-mail deve ser preenchido.")
                .EmailAddress().WithMessage("E-mail inválido.");

            RuleFor(p => p.Document)
                .NotEmpty().WithMessage("CPF deve ser preenchido.")
                .NotNull().WithMessage("CPF deve ser preenchido.")
                .Must(d => d.IsDocumentValid()).WithMessage("CPF inválido.");
        }
    }
}