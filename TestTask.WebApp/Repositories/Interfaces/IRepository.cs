using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestTask.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Guid ItemId);
        Task CreateAsync(T Item);
        Task Update(T Item);
        Task DeleteAsync(Guid ItemId);
    }
}
