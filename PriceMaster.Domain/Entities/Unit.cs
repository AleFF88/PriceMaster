namespace PriceMaster.Domain.Entities {
    public class Unit {
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public ICollection<Component> Components { get; set; } = [];
    }
}
