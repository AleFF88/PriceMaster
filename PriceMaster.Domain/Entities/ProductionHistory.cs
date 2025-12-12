namespace PriceMaster.Domain.Entities {
    public class ProductionHistory {
        public int ProductionHistoryId { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
        public decimal RecommendedPrice { get; set; }
        public decimal WorkCost { get; set; }
        public string? Notes { get; set; }
    }
}
