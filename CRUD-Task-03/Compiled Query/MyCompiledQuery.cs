using CRUD_Task_03.DBContext;
using CRUD_Task_03.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Task_03.Compiled_Query
{
    public class MyCompiledQuery
    {
        public static readonly Func<AppDbContext, IEnumerable<OrderHeader>> GetAllOrdersCompiled =
                EF.CompileQuery((AppDbContext context) =>
                       context.OrderHeaders.Where(ord => ord.IsActive == true));

        public static readonly Func<AppDbContext, long, IEnumerable<OrderRow>> GetOrderRowsByOrderIdCompiled =
                EF.CompileQuery((AppDbContext context, long orderId) =>
                       context.OrderRows.Where(or => or.OrderId == orderId));
    }
}
