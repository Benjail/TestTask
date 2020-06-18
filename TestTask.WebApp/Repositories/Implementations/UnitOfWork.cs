using Microsoft.AspNetCore.Identity;
using TestTask.Models;
using System;
using System.Threading.Tasks;
using TestTask.Database;
using TestTask.WebApp.Repositories.Interfaces;
using TestTask.Repositories.Interfaces;

namespace TestTask.Repositories.Implementations
{
    public class UnitOfWork : IDisposable ,IUnitOfWork
    {
        private AppDbContext dbContext;
        private UserManager<Customer> userManager;
        private IRepository<Item> itemRepository;
        private IRepository<Order> orderRepository;
        private IRepository<OrderElement> orderElementRepository;
        private IUserRepository<Customer> customerRepository;

        public UnitOfWork(AppDbContext dbContext, UserManager<Customer> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        
        public IRepository<Item> Items
        {
            get
            {
                if (itemRepository == null)
                {
                    itemRepository = new ItemRepository(dbContext);
                }
                return itemRepository;
            }
        }
        public IRepository<Order> Orders
        {
            get
            {
                if (orderRepository == null)
                {
                    orderRepository = new OrderRepository(dbContext);
                }
                return orderRepository;
            }
        }
        public IRepository<OrderElement> OrderElements
        {
            get
            {
                if (orderElementRepository == null)
                {
                    orderElementRepository = new OrderElementRepository(dbContext);
                }
                return orderElementRepository;
            }
        }
        public IUserRepository<Customer> Customers
        {
            get
            {
                if (customerRepository == null)
                {
                    customerRepository = new CustomerRepository(dbContext, userManager);
                }
                return customerRepository;
            }
        }
        
       
        public async Task SaveAsync() =>
            await dbContext.SaveChangesAsync();
        public void Dispose()
        {
        }
    }
}
