using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _4oito6.Demonstration.Domain.Model.Entities.Base
{
    public abstract class EntityBase
    {
        private readonly List<ValidationResult> _validationResults = new List<ValidationResult>();

        public bool IsValid => !_validationResults.Any(x => !x.IsValid);

        public bool Validate<TEntity>(TEntity entity, AbstractValidator<TEntity> validator)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (validator is null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            var result = validator.Validate(entity);
            _validationResults.Add(result);
            return result.IsValid;
        }
    }
}