using _4oito6.Demonstration.Domain.Model.Entities;
using FluentValidation;

namespace _4oito6.Demonstration.Contact.Domain.Services.Validators
{
    public class PersonNotFoundValidator : AbstractValidator<Person>
    {
        public PersonNotFoundValidator()
        {
            RuleFor(p => p.Id).Must(_ => false).WithMessage("Nenhuma Pessoa encontrada para o id informado.");
        }
    }
}