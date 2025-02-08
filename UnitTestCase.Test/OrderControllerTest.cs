/* //Example
[Fact]
public void OrderController_CreateOrderWithItems_ValidResult()
{
      //----AAA Format---
      //Arrange
      OrderController controller = new OrderController();
      string expectedResult = "It is okay";

      //Act
      string result = controller.index();

      //Assert
      Assert.Equal(expectedResult, result);
}
*/

using CRUD.Controllers;
using CRUD.DTO;
using CRUD.IRepository;
using CRUD.Helper;
using CRUD_Task_03.DTO;
using Microsoft.AspNetCore.Mvc;

public class OrderControllerTests
{
    [Fact]
    public async Task OrderController_CreateOrderWithItems_ValidOrder()
    {
        //Arrange -------------------------------------------------------------------
        var orderRepo = new FakeOrderRepository();
        var controller = new OrderController(orderRepo);
        var createOrderDto = new CreateOrderDTO
        {
            header = new CreateOrderHeaderDTO
            {
                CustomerName = "Test Customer"
            },
            rows = new List<CreateOrderRowDTO>
            {
                new CreateOrderRowDTO { ProductName = "Product 1", Quantity = 2, UnitPrice = 10.0m },
                new CreateOrderRowDTO { ProductName = "Product 2", Quantity = 1, UnitPrice = 20.0m }
            }
        };
        var expectedMessage = "Successfully Created Order";


        //Act ------------------------------------------------------------------------
        var result = await controller.CreateOrderWithItems(createOrderDto);


        //Assert ---------------------------------------------------------------------
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMessage = Assert.IsType<MessageHelper>(okResult.Value);
        Assert.Equal(expectedMessage, returnedMessage.Message);
        Assert.Equal(200, returnedMessage.StatusCode);
    }
}

public class FakeOrderRepository : IOrder
{
    public Task<MessageHelper> CreateOrderWithItems(CreateOrderDTO create)
    {
        // Simulate order creation and return a success message
        return Task.FromResult(new MessageHelper
        {
            Message = "Successfully Created Order",
            StatusCode = 200
        });
    }



    // for only interface purpose
    public Task<DateRangeSaleDTO> DateRangeSale(DateTime fromDate, DateTime toDate)
    {
        throw new NotImplementedException();
    }

    public Task<MessageHelper> DeleteOrderWithCorrespondingItems(long orderId)
    {
        throw new NotImplementedException();
    }

    public Task<List<GetOrderDetailsDTO>> GetByDateRang(DateTime fromDate, DateTime ToDate)
    {
        throw new NotImplementedException();
    }

    public Task<DateRangTotalAmount> GetDateRangTotalAmount(DateTime fromDate, DateTime ToDate)
    {
        throw new NotImplementedException();
    }

    public Task<MinMaxDTO> GetMinMax()
    {
        throw new NotImplementedException();
    }

    public Task<GetOrderDetailsDTO> GetOrderDetails(long Id)
    {
        throw new NotImplementedException();
    }

    public Task<OrderListPaginationDTO> GetOrderListPagination(long PageNo, long PageSize)
    {
        throw new NotImplementedException();
    }

    public Task<List<GetOrderDetailsTest>> GetOrdersByFilters(OrderFilterDTO filters)
    {
        throw new NotImplementedException();
    }

    public Task<List<practiceDTO>> Practice()
    {
        throw new NotImplementedException();
    }

    public Task<List<GetOrderDetailsDTO>> SearchByCustormerName(string name)
    {
        throw new NotImplementedException();
    }
}
