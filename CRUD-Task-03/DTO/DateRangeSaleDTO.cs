namespace CRUD_Task_03.DTO
{
    public class DateRangeSaleDTO
    {
        public decimal TotalAmountWithinDateRange { get; set; }
        public List<DailySale> DailySaleList { get; set; }

    }

    public class DailySale
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalSale { get; set; }
    }
}
