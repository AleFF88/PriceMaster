namespace PriceMaster.Application.Requests {
    public record GetProductDetailedReportRequest {
        public required string ProductCode { get; init; }
        public DateTime? StartDate { get; init; } = null;
        public DateTime? EndDate { get; init; } = null;
    }
}
