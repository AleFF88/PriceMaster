namespace PriceMaster.Domain.Entities {
    public class BomItem {          // Bill of Materials
        public int BomItemId { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int ComponentId { get; set; }
        public Component? Component { get; set; }

        public decimal Quantity { get; set; }
    }
}
