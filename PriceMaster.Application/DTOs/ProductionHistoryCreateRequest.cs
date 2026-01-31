namespace PriceMaster.Application.DTOs {
    public class ProductionHistoryCreateRequest {
        public required string ProductCode { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string? Notes { get; set; }
    }
}
