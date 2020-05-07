using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.Models;
using System;
using System.Threading.Tasks;

namespace TestTask.Database
{
    public static class Seed
    {
        public static async Task CreateRoles(IServiceProvider provider, IConfiguration configuration)
        {
            var userManager = provider.GetRequiredService<UserManager<Customer>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            //получение всех ролей из appsettings
            string[] roles = configuration.GetSection("Roles").Get<string[]>();
            //заполнение всех ролей в IdentityRole
            foreach (var role in roles) 
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
            //создание суперпользователя из appSettings
            var superUser = new Customer
            {
                Name = configuration.GetSection("AdminSettings")["Name"],
                UserName = configuration.GetSection("AdminSettings")["Email"],                
                Email = configuration.GetSection("AdminSettings")["Email"]                 
            }; 
            //проверка на наличие superUser                                             
            if (await userManager.FindByEmailAsync(superUser.Email) == null)
            {
                string password = configuration.GetSection("AdminSettings")["Password"];
                var SuResult = await userManager.CreateAsync(superUser, password); //создание superuser с поощью userManager 
                if (SuResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(superUser, "Admin");//присвоение роли superuser
                }
            }
        }
    }
}
