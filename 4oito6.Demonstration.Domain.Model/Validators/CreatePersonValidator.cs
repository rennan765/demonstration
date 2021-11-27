using _4oito6.Demonstration.Commons.Extensions;
using _4oito6.Demonstration.Domain.Model.Entities;
using FluentValidation;

namespace _4oito6.Demonstration.Domain.Model.Validators
{
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

            RuleFor(p => p.BirthDate)
                .Must(date => date.IsAgeRange(18, 150))
                .WithMessage("Necessário ser maior de 18 anos.");

            RuleFor(p => p.Phones)
                .NotEmpty().WithMessage("Telefone (s) obsigatório (s).");

            RuleFor(p => p.MainPhone)
                .NotNull().WithMessage("Necessário escolher um telefone principal.");
        }
    }
}