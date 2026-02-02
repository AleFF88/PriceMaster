using FluentValidation;
using PriceMaster.Application.Requests;

namespace PriceMaster.Application.Validators {
    public class CreateProductionHistoryRequestValidator : AbstractValidator<CreateProductionHistoryRequest> {
        public CreateProductionHistoryRequestValidator() {
            RuleFor(x => x.ProductCode).EnsureNotEmpty();

            RuleFor(x => x.ProductionDate)
                .LessThanOrEqualTo(p => DateTime.UtcNow).WithMessage("Production date cannot be in the future.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
