namespace PriceMaster.Contracts.DTOs {
    public class CreateProductRequest {
        public string ProductCode { get; set; } = string.Empty;
        public int SeriesId { get; set; }
        public decimal SizeWidth { get; set; }
        public decimal SizeHeight { get; set; }
        public decimal RecommendedPrice { get; set; }
        public string? Notes { get; set; }

        public List<BOMItemDto> BOMItems { get; set; } = [];
    }

    public class BOMItemDto {
        public int ComponentId { get; set; }
        public decimal Quantity { get; set; }
    }
}
