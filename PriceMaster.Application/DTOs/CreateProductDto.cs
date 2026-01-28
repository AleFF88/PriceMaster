namespace PriceMaster.Application.DTOs {
    public record CreateProductDto(string ProductCode, int SeriesId, decimal RecommendedPrice, List<BomItemDto> BomItems);

    public record BomItemDto(int ComponentId, decimal Quantity);
}
