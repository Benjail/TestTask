using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestTask.Repositories.Interfaces;
using TestTask.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestTask.Database;
using Microsoft.AspNetCore.Http;

namespace TestTask.Repositories.Implementations
{
    public class CustomerRepository : IUserRepository<Customer> 
    {
        private AppDbContext dbContext;
        private UserManager<Customer> userManager;

        public CustomerRepository(AppDbContext dbContext, UserManager<Customer> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<IdentityResult> CreateAsync(Customer customer, string password) =>
            await userManager.CreateAsync(customer, password);

        public async Task DeleteAsync(Customer customer) => await userManager.DeleteAsync(customer);

        public async Task<List<Customer>> GetAllAsync() => await dbContext.Customers.ToListAsync();

        public async Task<Customer> GetAsync(Guid customerId) =>
            await userManager.FindByIdAsync(customerId.ToString());

        public async Task<Customer> GetAsync(string customerEmail) =>
            await userManager.FindByEmailAsync(customerEmail);
        public async Task<Customer> GetCurrentCustomer(HttpContext httpContext) =>
            await userManager.FindByEmailAsync(httpContext.User.Identity.Name);
        public async Task Update(Customer customer)
        {
          //  await userManager.UpdateAsync(customer);             
                dbContext.Entry(customer).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();           
         }
           public void Dispose()
           {
                
           }
        }
    }

