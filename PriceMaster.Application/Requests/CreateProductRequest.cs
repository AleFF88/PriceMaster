namespace PriceMaster.Application.Requests {
    public record CreateProductRequest {
        public required string ProductCode { get; init; }
        public int SeriesId { get; init; }
        public decimal SizeWidth { get; init; }
        public decimal SizeHeight { get; init; }
        public decimal RecommendedPrice { get; init; }
        public required List<BomItemDto> BomItems { get; init; }
    }

    public record BomItemDto {
        public int ComponentId { get; init; }
        public decimal Quantity { get; init; }
    };
}
