using _4oito6.Demonstration.Domain.Model.Entities;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Domain.Model.Validators
{
    [ExcludeFromCodeCoverage]
    public class NotFoundPersonValidator : AbstractValidator<Person>
    {
        public NotFoundPersonValidator()
        {
            RuleFor(p => p.Id).Must(_ => false).WithMessage("Pessoa não encontrada.");
        }
    }
}