using CRUD.DTO;
using CRUD.Helper;
using CRUD.IRepository;
using CRUD_Task_03.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _IOrderRepo; 
        public OrderController(IOrder IOrderRepo)
        {
            _IOrderRepo = IOrderRepo;
        }

        [HttpPost]
        [Route("CreateOrderWithItems")]
        public async Task<IActionResult> CreateOrderWithItems(CreateOrderDTO create)
        {
            var result = await _IOrderRepo.CreateOrderWithItems(create);
            return Ok(result);
        }

        [HttpPut]
        [Route("DeleteOrderWithCorrespondingItems")]
        public async Task<IActionResult> DeleteOrderWithCorrespondingItems(long Id)
        {
            var result = await _IOrderRepo.DeleteOrderWithCorrespondingItems(Id);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetOrderDatails")]
        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            var result = await _IOrderRepo.GetOrderDetails(Id);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetMinMax")]
        public async Task<IActionResult> GetMinMax()
        {
            var result = await _IOrderRepo.GetMinMax();
            return Ok(result);
        }

        [HttpGet]
        [Route("SearchByCustormerName")]
        public async Task<IActionResult> SearchByCustomerName(string name)
        {
            var result = await _IOrderRepo.SearchByCustormerName(name);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetByDateRang")]
        public async Task<IActionResult> GetByDateRang(DateTime fromDate, DateTime ToDate)
        {
            var result = await _IOrderRepo.GetByDateRang(fromDate, ToDate);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetDateRangTotalAmount")]
        public async Task<IActionResult> GetDateRangTotalAmount(DateTime fromDate, DateTime ToDate)
        {
            var result = await _IOrderRepo.GetDateRangTotalAmount(fromDate, ToDate);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetOrdersByFilters")]
        public async Task<IActionResult> GetOrdersByFilters([FromQuery] OrderFilterDTO filters)
        {
            var result = await _IOrderRepo.GetOrdersByFilters(filters);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetOrderListPagination")]
        public async Task<IActionResult> GetOrderListPagination(long PageNo, long PageSize)
        {
            var result = await _IOrderRepo.GetOrderListPagination(PageNo, PageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("DateRangeSale")]
        public async Task<IActionResult> DateRangeSale(DateTime fromDate, DateTime toDate)
        {
            var result = await _IOrderRepo.DateRangeSale(fromDate, toDate);
            return Ok(result);
        }

        [HttpGet]
        [Route("Practice")]
        public async Task<IActionResult> Practice()
        {
            var result = await _IOrderRepo.Practice();
            return Ok(result);
        } 
    }
}
