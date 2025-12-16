namespace PriceMaster.Contracts.DTOs {
    public class ProductResponse {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ProductCode { get; set; }
    }
}
