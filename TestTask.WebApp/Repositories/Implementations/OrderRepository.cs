using TestTask.Repositories.Interfaces;
using TestTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestTask.Database;

namespace TestTask.Repositories.Implementations
{
    public class OrderRepository : IRepository<Order>
    {
        private AppDbContext dbContext;

        DateTime dateTimeDefault = new DateTime(01, 01, 01, 00, 00, 00);
        public OrderRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateAsync(Order order)
        {
            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid orderId)
        {
            var order = await dbContext.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                dbContext.Orders.Remove(order);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Order>> GetOrdersByStatus(OrderStatus orderStatus) =>
            await dbContext.Orders.Where(o => o.OrderStatus == orderStatus && o.OrderDate != dateTimeDefault).ToListAsync()
             ?? throw new NullReferenceException();
        public async Task<List<Order>> GetOrdersByCustomer(Guid customerId) =>
            await dbContext.Orders.Where(o=>o.CustomerId == customerId && o.OrderDate!= dateTimeDefault).ToListAsync()
                ?? throw new NullReferenceException();

        public async Task<List<Order>> GetAllAsync() => 
            await dbContext.Orders.ToListAsync()
             ?? throw new NullReferenceException();

        public async Task<Order> GetAsync(Guid orderId) =>
            await dbContext.Orders.FindAsync(orderId)
                ?? throw new NullReferenceException();

        public async Task Update(Order order)
        {
            dbContext.Entry<Order>(order).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task<Order> GetShopCart(Guid customerId) =>
            (await dbContext.Orders
                .Where(o => o.CustomerId == customerId && o.OrderDate == dateTimeDefault)
                    .ToListAsync()).FirstOrDefault();
        public void Dispose()
        {

        }
    }
}
