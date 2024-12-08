using CRUD.DTO;
using CRUD.Helper;
using CRUD.Repository;
using CRUD_Task_03.DTO;

namespace CRUD.IRepository
{
    public interface IOrder
    {
        Task<MessageHelper> CreateOrderWithItems(CreateOrderDTO create);
        Task<MessageHelper> DeleteOrderWithCorrespondingItems(long orderId);
        Task<GetOrderDetailsDTO> GetOrderDetails(long Id);
        Task<MinMaxDTO> GetMinMax();
        Task<List<GetOrderDetailsDTO>> SearchByCustormerName(string name);
        Task<List<GetOrderDetailsDTO>> GetByDateRang(DateTime fromDate, DateTime ToDate);
        Task<DateRangTotalAmount> GetDateRangTotalAmount(DateTime fromDate, DateTime ToDate);

        Task<List<GetOrderDetailsTest>> GetOrdersByFilters(OrderFilterDTO filters);
        Task<OrderListPaginationDTO> GetOrderListPagination(long PageNo, long PageSize);
    }
}
