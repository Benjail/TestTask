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
            dbContext.Orders.Remove(order);
            await dbContext.SaveChangesAsync();            
        }

        public async Task<Order> GetAsync(Guid orderId) => await dbContext.Orders
            .Include(o => o.OrderElements)
            .Where(o => o.OrderId == orderId) 
            .FirstOrDefaultAsync();

        public async Task<List<Order>> GetAsync(OrderStatus orderStatus) =>
            await dbContext.Orders
            .Where(o => o.OrderStatus == orderStatus)
            .ToListAsync();

        public async Task<List<Order>> GetAsync(Customer customer) =>
            await dbContext.Orders
            .Where(o => o.CustomerId == customer.Id)
            .ToListAsync(); 
        
        public async Task<List<Order>> GetAllAsync() =>                     
            await dbContext.Orders
            .ToListAsync();            
        
        public async Task Update(Order order)
        {
            dbContext.Entry(order).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }                       
               
        public void Dispose()
        {

        }














        public Task<Order> GetAsync(Item item)
        {
            throw new NotImplementedException();
        }
        public Task<List<Order>> GetAsync(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
