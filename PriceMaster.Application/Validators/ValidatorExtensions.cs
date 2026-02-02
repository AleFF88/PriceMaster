using FluentValidation;

namespace PriceMaster.Application.Validators {
    /// <summary>
    /// Provides custom extension methods for FluentValidation rule builders.
    /// </summary>
    public static class ValidatorExtensions {
        /// <summary>
        /// Validates that a numeric property is greater than its default value (zero).
        /// </summary>
        /// <typeparam name="T">The type of the parent object (DTO) containing the property.</typeparam>
        /// <typeparam name="TProperty">The type of the property being validated (value type and comparable).</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined.</param>
        /// <returns>A continuous rule builder for further chaining.</returns>
        /// <remarks>
        /// This method uses the <c>CompareTo</c> logic to ensure the value is strictly positive.
        /// The error message automatically includes the property name using the '{PropertyName}' placeholder.
        /// <para><b>⚠ Works only with value types.</b></para>
        /// </remarks>
        public static IRuleBuilderOptions<T, TProperty> IsPositive<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : struct, IComparable<TProperty>, IComparable {
            return ruleBuilder
                .Must(value => value.CompareTo(default(TProperty)) > 0)
                .WithMessage("The field '{PropertyName}' must be greater than zero.");
        }

        /// <summary>
        /// Validates that a property is not null or empty. 
        /// <para><b>This method shadows the default FluentValidation NotEmpty to apply a project-wide error message.</b></para>
        /// </summary>
        /// <typeparam name="T">The type of the parent object.</typeparam>
        /// <param name="ruleBuilder">The rule builder for a string property.</param>
        /// <returns>A continuous rule builder.</returns>
        public static IRuleBuilderOptions<T, TProperty> NotEmpty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string? customMessage = null) {
            var message = customMessage ?? "{PropertyName} must be provided.";
            return ruleBuilder
                .NotEmpty()
                .WithMessage(message);
        }
    }
}