namespace PriceMaster.Application.Requests {
    public record CreateProductionHistoryRequest {
        public required string ProductCode { get; init; }
        public DateTime? ProductionDate { get; init; }
        public string? Notes { get; init; }
    }
}
