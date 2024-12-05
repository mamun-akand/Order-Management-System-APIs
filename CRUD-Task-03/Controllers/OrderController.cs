using CRUD.DTO;
using CRUD.Helper;
using CRUD.IRepository;
using CRUD_Task_03.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<GetOrderDetailsDTO> GetOrderDetails(int Id)
        {
            var result = await _IOrderRepo.GetOrderDetails(Id);
            return result;
        }

        [HttpGet]
        [Route("GetMinMax")]
        public async Task<MinMaxDTO> GetMinMax()
        {
            var result = await _IOrderRepo.GetMinMax();
            return result;
        }



    }
}
