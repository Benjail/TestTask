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
           // var item = await dbContext.Items.FindAsync(itemId);
            if (await dbContext.Items.FindAsync(itemId)!=null)
            {
                dbContext.Items.Remove(await dbContext.Items.FindAsync(itemId));
                await dbContext.SaveChangesAsync();  
            }
            else
            {
                throw new NullReferenceException();
            }
        }
        public async Task<List<Item>> GetAllAsync() => await dbContext.Items.ToListAsync();
        public async Task<Item> GetAsync(Guid itemId) =>
            await dbContext.Items.FindAsync(itemId) 
                ?? throw new NullReferenceException();
        public async Task Update(Item item)
        {
           dbContext.Entry(item).State = EntityState.Modified;
          await dbContext.SaveChangesAsync();
        }
        public void Dispose()
        { 
        }
    }
}
