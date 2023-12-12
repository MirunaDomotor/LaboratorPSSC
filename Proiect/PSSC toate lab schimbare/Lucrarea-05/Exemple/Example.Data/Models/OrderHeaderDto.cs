namespace Example.Data.Models
{
    public class OrderHeaderDto
    {
        public OrderHeaderDto(string name, string address, double total)
        {
            Address = address;
            Total = total;
            Name = name;
        }

        public OrderHeaderDto()
        {

        }

        public int OrderId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public double? Total { get; set; }
    }
}
