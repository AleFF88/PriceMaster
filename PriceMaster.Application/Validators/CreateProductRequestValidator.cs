using FluentValidation;
using PriceMaster.Application.Requests;

namespace PriceMaster.Application.Validators {
    /// <summary>
    /// Validator for product creation data.
    /// Ensures data integrity before executing any business logic or database operations.
    /// </summary>
    /// <remarks>
    /// We do not perform database checks (e.g., uniqueness) here.
    /// Database-related validation is handled within the ProductService to keep 
    /// the validator fast and independent of infrastructure.
    /// </remarks>
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest> {
        public CreateProductRequestValidator() {
            RuleFor(x => x.ProductCode)
                .EnsureNotEmpty()
                .MaximumLength(10).WithMessage("Product code cannot exceed 10 characters.");

            RuleFor(x => x.SeriesId).IsPositive();
            RuleFor(x => x.SizeWidth).IsPositive();
            RuleFor(x => x.SizeHeight).IsPositive();
            RuleFor(x => x.RecommendedPrice).IsPositive();

            RuleFor(x => x.BomItems).EnsureNotEmpty("Product must have at least one BOM item.");

            RuleForEach(x => x.BomItems).ChildRules(item => {
                item.RuleFor(i => i.ComponentId).IsPositive();
                item.RuleFor(i => i.Quantity).IsPositive();
            });
        }
    }
}