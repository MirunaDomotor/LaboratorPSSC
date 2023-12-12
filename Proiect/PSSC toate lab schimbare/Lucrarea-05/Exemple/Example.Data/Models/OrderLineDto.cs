namespace Example.Data.Models
{
    public class OrderLineDto
    {
        public int OrderLineId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
    }
}
