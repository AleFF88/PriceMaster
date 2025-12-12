namespace PriceMaster.Domain.Entities {
    public class Category {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public ICollection<Component> Components { get; set; } = [];
    }
}
