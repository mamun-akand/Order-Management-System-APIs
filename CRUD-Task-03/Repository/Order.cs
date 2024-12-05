
using CRUD.DTO;
using CRUD.Helper;
using CRUD.IRepository;
using CRUD_Task_03.DBContext;
using CRUD_Task_03.DTO;
using CRUD_Task_03.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace CRUD.Repository
{
    public class Order : IOrder
    {
        private readonly AppDbContext _context;
        public Order(AppDbContext context)
        {
            _context = context;
        }


        public async Task<MessageHelper> CreateOrderWithItems(CreateOrderDTO create)
        {
            try
            {
                var newOrderHead = new OrderHeader
                {
                    CustomerName = create.header.CustomerName,
                    OrderDate = DateTime.Now,
                    TotalAmount = create.rows.Sum(item => item.Quantity * item.UnitPrice),
                    IsActive = true,
                };
                await _context.OrderHeaders.AddAsync(newOrderHead);
                await _context.SaveChangesAsync();

                var newOrderRows = create.rows.Select(item => new OrderRow
                {
                    OrderId = newOrderHead.OrderId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    IsActive = true,
                }).ToList();
                await _context.OrderRows.AddRangeAsync(newOrderRows);
                await _context.SaveChangesAsync();

                return new MessageHelper
                {
                    Message = "Successfully Created Order",
                    StatusCode = 200,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<MessageHelper> DeleteOrderWithCorrespondingItems(long Id)
        {
            try
            {
                var orderToDelete = await _context.OrderHeaders.FirstOrDefaultAsync(o => o.OrderId == Id);

                if (orderToDelete is null)
                {
                    throw new Exception("Order Not Found");
                }

                orderToDelete.IsActive = false;
                _context.OrderHeaders.Update(orderToDelete);
                _context.SaveChanges();

                var itemsToDelete = await _context.OrderRows.Where(item => item.OrderId == Id).ToListAsync();

                foreach (var item in itemsToDelete)
                {
                    item.IsActive = false;
                }
                _context.OrderRows.UpdateRange(itemsToDelete);
                await _context.SaveChangesAsync();

                return new MessageHelper
                {
                    Message = "Successful",
                    StatusCode = 200,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<GetOrderDetailsDTO> GetOrderDetails(long Id)
        {

            try
            {
                var orderheader = await (from h in _context.OrderHeaders
                                         where h.IsActive == true && h.OrderId == Id
                                         select new GetOrderDetailsHeaderDTO
                                         {
                                             OrderId = h.OrderId,
                                             CustomerName = h.CustomerName,
                                             OrderDate = h.OrderDate,
                                         }).FirstOrDefaultAsync();

                var orderRow = await (from r in _context.OrderRows
                                      where r.OrderId == Id && r.IsActive == true
                                      select new GetOrderDetailsRowDTO
                                      {
                                          OrderItemId = r.OrderItemId,
                                          ProductName = r.ProductName,
                                          Quantity = r.Quantity,
                                          UnitPrice = r.UnitPrice,
                                      }).ToListAsync();


                var OrderDetails = new GetOrderDetailsDTO
                {
                    getOrderDetailsHeader = orderheader,
                    Rows = orderRow,
                };
                return OrderDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<MinMaxDTO> GetMinMax()
        {
            try
            {
                var minOrder = await _context.OrderHeaders.Where(h => h.IsActive == true)
                                                          .OrderBy(h => h.TotalAmount)
                                                          .FirstOrDefaultAsync();

                var minOrderHeader =  new GetOrderDetailsHeaderDTO
                                      {
                                           OrderId = minOrder.OrderId,
                                           CustomerName = minOrder.CustomerName,
                                           OrderDate = minOrder.OrderDate,
                                      };

                var minOrderRows = await _context.OrderRows.Where(r => r.IsActive == true && r.OrderId == minOrder.OrderId)
                                                     .Select(row => new GetOrderDetailsRowDTO
                                                     {
                                                          OrderItemId = row.OrderItemId,
                                                          ProductName = row.ProductName,
                                                          Quantity = row.Quantity,
                                                          UnitPrice = row.UnitPrice,
                                                     }).ToListAsync();

                var maxOrder = await _context.OrderHeaders.Where(h => h.IsActive == true)
                                                    .OrderByDescending(h => h.TotalAmount)
                                                    .FirstOrDefaultAsync();

                var maxOrderHeader = new GetOrderDetailsHeaderDTO
                                        {
                                            OrderId = maxOrder.OrderId,
                                            CustomerName = maxOrder.CustomerName,
                                            OrderDate = maxOrder.OrderDate,
                                        };

                var maxOrderRows = await _context.OrderRows.Where(r => r.IsActive == true && r.OrderId == maxOrder.OrderId)
                                                     .Select(row => new GetOrderDetailsRowDTO
                                                     {
                                                         OrderItemId = row.OrderItemId,
                                                         ProductName = row.ProductName,
                                                         Quantity = row.Quantity,
                                                         UnitPrice = row.UnitPrice,
                                                     }).ToListAsync();
                return new MinMaxDTO
                {
                    MinOrderDetails = new GetOrderDetailsDTO
                    {
                        getOrderDetailsHeader = minOrderHeader,
                        Rows = minOrderRows,
                    },

                    MaxOrderDetails = new GetOrderDetailsDTO
                    {
                        getOrderDetailsHeader = maxOrderHeader,
                        Rows = maxOrderRows,
                    }
                };


            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

}
