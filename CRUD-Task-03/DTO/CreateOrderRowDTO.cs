namespace CRUD.DTO
{
    public class CreateOrderRowDTO
    {
        public string ProductName { get; set; } = null!;
        public long Quantity { get; set; }
        public decimal UnitPrice { get; set; } 
    }
}
