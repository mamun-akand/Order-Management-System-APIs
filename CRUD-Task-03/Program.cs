using CRUD.IRepository;
using CRUD.Repository;
using CRUD_Task_03.DBContext;
using Microsoft.EntityFrameworkCore;
using System;

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
                FetchAndPrintOrders(dbContext);
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
        private static void FetchAndPrintOrders(AppDbContext _dbContext)
        {
            var orders = _dbContext.OrderHeaders.ToList();

            Console.WriteLine("Orders Retrieved from Database:");
            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}, Customer: {order.CustomerName}");
            }
        }
    }
}
