using _4oito6.Demonstration.Domain.Model.Entities;
using FluentValidation;

namespace _4oito6.Demonstration.Domain.Model.Validators
{
    public class NotFoundPersonValidator : AbstractValidator<Person>
    {
        public NotFoundPersonValidator()
        {
            RuleFor(p => p.Id).Must(_ => false).WithMessage("Pessoa não encontrada.");
        }
    }
}