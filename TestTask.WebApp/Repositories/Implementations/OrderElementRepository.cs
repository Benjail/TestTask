using Microsoft.EntityFrameworkCore;
using TestTask.Repositories.Interfaces;
using TestTask.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestTask.Database;
using System.Linq;

namespace TestTask.Repositories.Implementations
{
    public class OrderElementRepository : IRepository<OrderElement>
    {
        private AppDbContext dbContext;

        public OrderElementRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateAsync(OrderElement orderElement)
        {
            await dbContext.OrderElements.AddAsync(orderElement);
            await dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid orderElementId)
        {
            var orderElement = await dbContext.OrderElements.FindAsync(orderElementId);
            if (orderElement == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                dbContext.OrderElements.Remove(orderElement);
                await dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<OrderElement>> GetAllByOrder(Guid orderId) =>
             await dbContext.OrderElements
                .Where(o => o.OrderId == orderId)
                .ToListAsync();
       public async Task<List<OrderElement>> GetShopCartElements(Guid orderId) =>
            await dbContext.OrderElements
                .Where(o => o.OrderId == orderId)
                    .ToListAsync(); 
        public async Task<OrderElement> GetAsync(Item item) =>
          (await dbContext.OrderElements
            .Where(o => o.ItemId == item.ItemId)
                .ToListAsync()).FirstOrDefault();  
        public async Task<List<OrderElement>> GetAllAsync() => await dbContext.OrderElements.ToListAsync();
        public async Task<OrderElement> GetAsync(Guid orderElementId) =>
            await dbContext.OrderElements.FindAsync(orderElementId)
                ?? throw new NullReferenceException();
        public async Task<List<OrderElement>> GetAsync(Order order) =>
            (await dbContext.OrderElements.Where(o => o.Order == order)
                 .ToListAsync())
             ?? throw new NotImplementedException();
        
        public async Task Update(OrderElement orderElement)
        {
            dbContext.Entry(orderElement).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }



















        public Task<List<OrderElement>> GetAsync(OrderStatus orderStatus)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderElement>> GetAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
        
        public Task<Order> GetShopCart(Guid customerId)
        {
            throw new NotImplementedException();
        }
       
        public void Dispose()
        {
        }
    }
}