using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Domain.Model.Validators
{
    [ExcludeFromCodeCoverage]
    public class UpdatePersonValidator : CreatePersonValidator
    {
        public UpdatePersonValidator() : base()
        {
            RuleFor(p => p.Id)
                .Must(id => id > 0).WithMessage("Instância deve ter ID preenchido.");
        }
    }
}