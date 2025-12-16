namespace PriceMaster.Contracts.DTOs.ProductionHistory {
    public class AddProductionHistoryResponse {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
        public decimal RecommendedPrice { get; set; }
        public decimal WorkCost { get; set; }
    }
}
