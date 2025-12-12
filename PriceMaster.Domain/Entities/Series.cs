namespace PriceMaster.Domain.Entities {
    public class Series {
        public int SeriesId { get; set; }
        public string? SeriesName { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
