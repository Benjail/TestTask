using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestTask.Models;
using System;

namespace TestTask.Database
{
    public class AppDbContext : IdentityDbContext<Customer, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders{ get; set; }
        public DbSet<Item> Items { get; set; }                      
        public DbSet<OrderElement> OrderElements { get; set; }
    }
}
