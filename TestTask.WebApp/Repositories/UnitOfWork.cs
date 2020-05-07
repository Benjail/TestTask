using Microsoft.AspNetCore.Identity;
using TestTask.Repositories.Implementations;
using TestTask.Models;
using System;
using System.Threading.Tasks;
using TestTask.Database;

namespace TestTask.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private AppDbContext dbContext;
        private UserManager<Customer> userManager;
        private ItemRepository itemRepository;
        private OrderRepository orderRepository;
        private OrderElementRepository orderElementRepository;
        private CustomerRepository customerRepository;

        public UnitOfWork(AppDbContext dbContext, UserManager<Customer> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public ItemRepository Items
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
        public OrderRepository Orders
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
        public OrderElementRepository OrderElements
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
        public CustomerRepository Customers
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
