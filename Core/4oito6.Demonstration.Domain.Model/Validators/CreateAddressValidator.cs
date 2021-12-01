using _4oito6.Demonstration.Domain.Model.Entities;
using FluentValidation;

namespace _4oito6.Demonstration.Domain.Model.Validators
{
    public class CreateAddressValidator : AbstractValidator<Address>
    {
        public CreateAddressValidator()
        {
            RuleFor(a => a.Street)
                .NotEmpty().WithMessage("Logradouro é obrigatório.")
                .NotNull().WithMessage("Logradouro é obrigatório.");

            RuleFor(a => a.District)
                .NotEmpty().WithMessage("Bairro é obrigatório.")
                .NotNull().WithMessage("Bairro é obrigatório.");

            RuleFor(a => a.City)
                .NotEmpty().WithMessage("Cidade é obrigatório.")
                .NotNull().WithMessage("Cidade é obrigatório.");

            RuleFor(a => a.State)
                .NotEmpty().WithMessage("Estado é obrigatório.")
                .NotNull().WithMessage("Estado é obrigatório.")
                .MaximumLength(2).WithMessage("Estado inválido.");

            RuleFor(a => a.PostalCode)
                .NotEmpty().WithMessage("CEP é obrigatório.")
                .NotNull().WithMessage("CEP é obrigatório.")
                .Length(8).WithMessage("CEP inválido")
                .Must(cep => long.TryParse(cep, out var result)).WithMessage("CEP deve conter somente números.");
        }
    }
}