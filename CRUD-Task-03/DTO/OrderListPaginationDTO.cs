namespace CRUD_Task_03.DTO
{
    public class OrderListPaginationDTO
    {
        public List<OrderListDataDTO> data { get; set; }
        public long currentPage { get; set; }
        public long pageSize { get; set; }
        public long totalCount { get; set; }
    }
}
