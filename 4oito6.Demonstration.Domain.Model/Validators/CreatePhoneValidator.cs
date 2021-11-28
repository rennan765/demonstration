using _4oito6.Demonstration.Domain.Model.Entities;
using FluentValidation;

namespace _4oito6.Demonstration.Domain.Model.Validators
{
    public class CreatePhoneValidator : AbstractValidator<Phone>
    {
        public CreatePhoneValidator()
        {
            RuleFor(p => p.Code)
                .NotNull().WithMessage("DDD obrigatório.")
                .NotEmpty().WithMessage("DDD obrigatório.")
                .MaximumLength(2).WithMessage("DDD inválido.");

            RuleFor(p => p.Number)
                .NotNull().WithMessage("Número de telefone obrigatório.")
                .NotEmpty().WithMessage("Número de telefone obrigatório.")
                .MaximumLength(9).WithMessage("Número de telefone inválido.");
        }
    }
}