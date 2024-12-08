
using CRUD.DTO;
using CRUD.Helper;
using CRUD.IRepository;
using CRUD_Task_03.DBContext;
using CRUD_Task_03.DTO;
using CRUD_Task_03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<List<GetOrderDetailsDTO>> SearchByCustormerName(string name)
        {
            try
            {
                var allDetails = await (from H in _context.OrderHeaders.Where(x => x.CustomerName.Contains(name) && x.IsActive == true)
                                        join R in _context.OrderRows on H.OrderId equals R.OrderId
                                        group R by new{H.OrderId, H.CustomerName,H.OrderDate } into grouped
                                        select new GetOrderDetailsDTO
                                        {
                                            getOrderDetailsHeader = new GetOrderDetailsHeaderDTO
                                            {
                                                OrderId = grouped.Key.OrderId,
                                                CustomerName = grouped.Key.CustomerName,
                                                OrderDate = grouped.Key.OrderDate,
                                            },
                                            Rows = grouped.Select(r => new GetOrderDetailsRowDTO
                                            {
                                                OrderItemId = r.OrderItemId,
                                                ProductName = r.ProductName,
                                                Quantity = r.Quantity,
                                                UnitPrice = r.UnitPrice,
                                            }).ToList()
                                        }).ToListAsync();

                return allDetails;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<GetOrderDetailsDTO>> GetByDateRang(DateTime fromDate, DateTime ToDate)
        {
            try
            {
                var allDetails = await (from H in _context.OrderHeaders.Where(x => x.OrderDate >= fromDate.Date && x.OrderDate <= ToDate.Date && x.IsActive == true)
                                       join R in _context.OrderRows on H.OrderId equals R.OrderId
                                       group R by new { H.OrderId, H.CustomerName, H.OrderDate } into grouped
                                       select new GetOrderDetailsDTO
                                       {
                                           getOrderDetailsHeader = new GetOrderDetailsHeaderDTO
                                           {
                                               OrderId = grouped.Key.OrderId,
                                               CustomerName = grouped.Key.CustomerName,
                                               OrderDate = grouped.Key.OrderDate,
                                           },
                                           Rows = grouped.Select(r => new GetOrderDetailsRowDTO
                                           {
                                               OrderItemId = r.OrderItemId,
                                               ProductName = r.ProductName,
                                               Quantity = r.Quantity,
                                               UnitPrice = r.UnitPrice,
                                           }).ToList()
                                       }).ToListAsync();

                return allDetails;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<DateRangTotalAmount> GetDateRangTotalAmount(DateTime fromDate, DateTime ToDate)
        {
            try
            {
                var TotalSum = await _context.OrderHeaders.Where(x => x.OrderDate >= fromDate.Date && x.OrderDate <= ToDate.Date
                                                   && x.IsActive == true).SumAsync(p => p.TotalAmount);

                return new DateRangTotalAmount
                {
                    TotalAmount = TotalSum,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<List<GetOrderDetailsTest>> GetOrdersByFilters(OrderFilterDTO filters)
        {
            try
            {
                //var allDetails = await (from H in _context.OrderHeaders
                //                        join R in _context.OrderRows on H.OrderId equals R.OrderId
                //                        where (filters.StartDate == null || filters.StartDate >= H.OrderDate)
                //                        && (filters.EndDate == null || filters.EndDate <= H.OrderDate)
                //                        && (filters.CustomerName == null || H.CustomerName.ToLower().Contains(filters.CustomerName.ToLower()))
                //                        && (filters.MinTotalAmount == null || filters.MinTotalAmount == 0 || filters.MinTotalAmount >= H.TotalAmount)
                //                        && (filters.MaxTotalAmount == null || filters.MaxTotalAmount == 0 || filters.MaxTotalAmount <= H.TotalAmount)
                //                        group R by new { H.OrderId, H.CustomerName, H.OrderDate } into grouped
                //                        select new GetOrderDetailsDTO2
                //                        {
                //                            getOrderDetailsHeader = new GetOrderDetailsHeaderDTO2
                //                            {
                //                                OrderId = grouped.Key.OrderId,
                //                                CustomerName = grouped.Key.CustomerName,
                //                                OrderDate = grouped.Key.OrderDate,
                //                                Rows = grouped.Select(r => new GetOrderDetailsRowDTO
                //                                {
                //                                    OrderItemId = r.OrderItemId,
                //                                    ProductName = r.ProductName,
                //                                    Quantity = r.Quantity,
                //                                    UnitPrice = r.UnitPrice,
                //                                }).ToList()
                //                            }

                //                        }).ToListAsync();


                var Headers = await (from H in _context.OrderHeaders
                                                 where (filters.StartDate == null || filters.StartDate >= H.OrderDate)
                                                       && (filters.EndDate == null || filters.EndDate <= H.OrderDate)
                                                       && (filters.CustomerName == null || H.CustomerName.ToLower().Contains(filters.CustomerName.ToLower()))
                                                       && (filters.MinTotalAmount == null || filters.MinTotalAmount == 0 || filters.MinTotalAmount >= H.TotalAmount)
                                                       && (filters.MaxTotalAmount == null || filters.MaxTotalAmount == 0 || filters.MaxTotalAmount <= H.TotalAmount)
                                                 select new GetOrderDetailsTest
                                                 {
                                                     OrderId = H.OrderId,
                                                     CustomerName = H.CustomerName,
                                                     OrderDate = H.OrderDate,
                                                     Rows = new List<GetOrderDetailsRowDTO>()
                                                 }).ToListAsync();

                var orderRows = await _context.OrderRows
                                              .Where(r => Headers.Select(h => h.OrderId).Contains(r.OrderId))
                                              .ToListAsync();

                var updateHeaders = Headers.Select(H =>
                {
                    H.Rows = orderRows.Where(r => r.OrderId == H.OrderId)
                                           .Select(r => new GetOrderDetailsRowDTO
                                           {
                                                OrderItemId = r.OrderItemId,
                                                ProductName = r.ProductName,
                                                Quantity = r.Quantity,
                                                UnitPrice = r.UnitPrice
                                           })
                                           .ToList();
                    return H;
                }).ToList();

                return updateHeaders;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<OrderListPaginationDTO> GetOrderListPagination(long PageNo, long PageSize)
        {
            try
            {
                if (PageNo <= 0) PageNo = 1;

                var currentData = await _context.OrderHeaders.Where(o => o.IsActive == true)
                                                        .Skip((int)((PageNo - 1) * PageSize))
                                                        .Take((int)PageSize)
                                                        .Select(ord => new OrderListDataDTO
                                                        {
                                                            OrderId = ord.OrderId,
                                                            OrderDate = ord.OrderDate,
                                                            CustomerName = ord.CustomerName,
                                                            TotalAmount = ord.TotalAmount,
                                                        }).ToListAsync();
                return new OrderListPaginationDTO
                {
                    data = currentData,
                    currentPage = PageNo,
                    pageSize = PageSize,
                    totalCount = currentData.Count(),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}
