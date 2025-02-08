namespace CRUD_Task_03.DTO
{
    public class practiceDTO
    {
        public long OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool? IsActive { get; set; }
        public long OrderItemId { get; set; }
        public string ProductName { get; set; }
        public long Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
