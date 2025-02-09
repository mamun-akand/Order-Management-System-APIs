using CRUD.IRepository;
using CRUD.Repository;
using CRUD_Task_03.Compiled_Query;
using CRUD_Task_03.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace CRUD_Task_03
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            /* Added Start */

            var connectionString = builder.Configuration.GetConnectionString("CRUDConnectionString"); //added
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString)); //added 
            builder.Services.AddScoped<IOrder, Order>();  //added

            /* Added End */

            var app = builder.Build();


            /* For Test Database Call Start */

            // **Fetch and print orders from database**
            using (var scope = app.Services.CreateScope()) // Create a scope for DI
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Get DbContext
                GetDataUsingCompiledQuery(dbContext);
            }

            /* For Test Database Call End */


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }


        // **Method to fetch and print orders**
        private static void GetDataUsingCompiledQuery(AppDbContext _dbContext)
        {
            // Compiled Query Call
            var orders = MyCompiledQuery.GetAllOrdersCompiled(_dbContext).ToList();
            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Customer: {order.CustomerName}, Date: {order.OrderDate}");
                var orderRows = MyCompiledQuery.GetOrderRowsByOrderIdCompiled(_dbContext, order.OrderId).ToList();
                foreach (var detail in orderRows)
                {
                    Console.WriteLine($"\tProduct: {detail.ProductName}, Quantity: {detail.Quantity}, Price: {detail.UnitPrice}");
                }
            }
        }
    }
}
