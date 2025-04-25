using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Intaker.TaskManager.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                // If no validators are registered for this request, skip validation
                return await next();
            }

            // Create validation context
            var context = new ValidationContext<TRequest>(request);

            // Run all validators
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Combine all validation failures
            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToList();

            if (failures.Count > 0)
            {
                // If any validations failed, throw a ValidationException
                throw new ValidationException(failures);
            }

            // All validations passed, continue to the next behavior or handler
            return await next();
        }
    }
} 