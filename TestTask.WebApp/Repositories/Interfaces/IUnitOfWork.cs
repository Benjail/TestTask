using System.Threading.Tasks;
using TestTask.Models;
using TestTask.Repositories.Interfaces;

namespace TestTask.WebApp.Repositories.Interfaces
{
    public interface IUnitOfWork
    { 
        IRepository<Item> Items { get; } 
        IRepository<Order> Orders { get; }
        IRepository<OrderElement> OrderElements { get; } 
        IUserRepository<Customer> Customers { get; }
        Task SaveAsync();
    }
}
