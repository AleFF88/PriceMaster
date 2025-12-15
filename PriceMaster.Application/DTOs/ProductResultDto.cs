namespace PriceMaster.Application.DTOs {
    public class ProductResultDto {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ProductCode { get; set; }
    }
}
