namespace PriceMaster.Contracts.DTOs.Reports {
    public class ProductDetailedReportResponse {
        public string ProductCode { get; set; } = string.Empty; 
        public int Count { get; set; }
        public decimal TotalValue { get; set; }
        public decimal WorkCost { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
    }
}
