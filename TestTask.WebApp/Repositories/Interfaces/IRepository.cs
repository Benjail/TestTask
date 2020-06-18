using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestTask.Models;

namespace TestTask.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class 
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Guid Id);
        Task<T> GetAsync(Item item);
        Task<List<T>> GetAsync(Customer customer);
        Task<List<T>> GetAsync(OrderStatus orderStatus); 
        Task<List<T>> GetAsync(Order order); 
        Task CreateAsync(T Item);                                           
        Task Update(T Item);
        Task DeleteAsync(Guid ItemId);
      
      
       
    }
}
