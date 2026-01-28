namespace PriceMaster.Application.DTOs {
    public record CreateProductDto {
        public string ProductCode { get; init; }
        public int SeriesId { get; init; }
        public decimal SizeWidth { get; init; }
        public decimal SizeHeight { get; init; }
        public decimal RecommendedPrice { get; init; }
        public List<BomItemDto> BomItems { get; init; }
    }

    public record BomItemDto {
        public int ComponentId { get; init; }
        public decimal Quantity { get; init; }
    };
}
