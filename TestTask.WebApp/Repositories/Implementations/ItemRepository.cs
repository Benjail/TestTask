using TestTask.Repositories.Interfaces;
using TestTask.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestTask.Database;

namespace TestTask.Repositories.Implementations
{
    public class ItemRepository : IRepository<Item>
    {
        private AppDbContext dbContext;

        public ItemRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task CreateAsync(Item item)
        {
            await dbContext.Items.AddAsync(item);
            dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid itemId)
        {
                dbContext.Items.Remove(await dbContext.Items.FindAsync(itemId));
                await dbContext.SaveChangesAsync();  
        }
        public async Task<List<Item>> GetAllAsync() => await dbContext.Items.ToListAsync();
        public async Task<Item> GetAsync(Guid itemId) =>
            await dbContext.Items.FindAsync(itemId);

        public async Task Update(Item item)
        {
           dbContext.Entry(item).State = EntityState.Modified;
          await dbContext.SaveChangesAsync();
        }
        public void Dispose()
        { 
        }
















        public Task<List<Item>> GetAsync(OrderStatus orderStatus)
        {
            throw new NotImplementedException();
        }

        public Task<List<Item>> GetAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public Task<Item> GetAsync(Item item)
        {
            throw new NotImplementedException();
        }
        public Task<List<Item>> GetAsync(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
