using FluentValidation;
using PriceMaster.Application.DTOs;

namespace PriceMaster.Application.Validators {
    public class ProductionHistoryCreateRequestValidator : AbstractValidator<ProductionHistoryCreateRequest> {
        public ProductionHistoryCreateRequestValidator() {
            RuleFor(x => x.ProductCode).NotEmpty();

            RuleFor(x => x.ProductionDate)
                .LessThanOrEqualTo(p => DateTime.UtcNow).WithMessage("Production date cannot be in the future.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
