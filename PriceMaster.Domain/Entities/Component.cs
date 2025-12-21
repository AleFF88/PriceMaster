namespace PriceMaster.Domain.Entities {
    public class Component {
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }

        public int UnitId { get; set; }
        public Unit? Unit { get; set; }

        public decimal PricePerUnit { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<BomItem> BomItems { get; set; } = [];
    }
}
