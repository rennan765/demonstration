using _4oito6.Demonstration.Domain.Model.Entities;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Domain.Model.Validators
{
    [ExcludeFromCodeCoverage]
    public class EmailDocumentPersonValidator : AbstractValidator<Person>
    {
        public EmailDocumentPersonValidator()
        {
            RuleFor(p => p).Must(_ => false).WithMessage("E-mail e/ou CPF já cadastrados.");
        }
    }
}