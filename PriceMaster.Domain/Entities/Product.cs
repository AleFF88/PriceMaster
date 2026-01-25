namespace PriceMaster.Domain.Entities {
    public class Product {
        public int ProductId { get; set; }
        public required string ProductCode { get; set; }

        public int SeriesId { get; set; }
        public Series? Series { get; set; }

        public decimal SizeWidth { get; set; }
        public decimal SizeHeight { get; set; }
        public decimal RecommendedPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }

        public ICollection<BomItem> BomItems { get; set; } = [];
        public ICollection<ProductionHistory> ProductionHistories { get; set; } = [];
    }
}
