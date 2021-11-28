using _4oito6.Demonstration.Domain.Model.Entities;
using FluentValidation;

namespace _4oito6.Demonstration.Domain.Model.Validators
{
    public class EmailDocumentPersonValidator : AbstractValidator<Person>
    {
        public EmailDocumentPersonValidator()
        {
            RuleFor(p => p).Must(_ => false).WithMessage("E-mail e/ou CPF já cadastrados.");
        }
    }
}