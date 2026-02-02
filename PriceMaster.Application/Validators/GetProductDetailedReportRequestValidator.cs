using FluentValidation;
using PriceMaster.Application.Requests;

namespace PriceMaster.Application.Validators {
    public class GetProductDetailedReportRequestValidator : AbstractValidator<GetProductDetailedReportRequest> {
        public GetProductDetailedReportRequestValidator() {
            RuleFor(x => x.ProductCode).EnsureNotEmpty();

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("Start date cannot be later than end date.");
        }
    }
}
