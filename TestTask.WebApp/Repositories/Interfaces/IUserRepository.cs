using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestTask.Repositories.Interfaces
{
   public interface IUserRepository<T> where T : class
    {
        Task<IdentityResult> CreateAsync(T user, string password);
        Task DeleteAsync(T user);
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Guid userId);
        Task<T> GetAsync(string customeremail);
        Task Update(T user);
        Task<T> GetCurrentCustomer(HttpContext httpContext);
    }
}
