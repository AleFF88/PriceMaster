namespace PriceMaster.Application.Requests {
    public class CreateProductionHistoryRequest {
        public required string ProductCode { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string? Notes { get; set; }
    }
}
