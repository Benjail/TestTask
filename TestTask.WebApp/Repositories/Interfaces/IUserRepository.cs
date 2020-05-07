using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestTask.Repositories.Interfaces
{
    interface IUserRepository<T> where T : class
    {
        Task<IdentityResult> CreateAsync(T user, string password);
        Task DeleteAsync(T user);
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Guid userId);
        Task Update(T user);
    }
}
