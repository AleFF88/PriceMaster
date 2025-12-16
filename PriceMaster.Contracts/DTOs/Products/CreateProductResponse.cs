namespace PriceMaster.Contracts.DTOs.Products {
    public class CreateProductResponse {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ProductCode { get; set; }
    }
}
