using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestTask.Requests;
using TestTask.Models;
using System.Security.Claims;
using TestTask.WebApp.Repositories.Interfaces;

namespace TestTask.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<Customer> userManager;
        private readonly IUnitOfWork unitOfWork;

        public AccountController(
            UserManager<Customer> userManager,
            IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationModel request)
        {
            if (ModelState.IsValid)
            {
                if (await unitOfWork.Customers.GetAsync(request.Email) == null) //проверка уникальности Email
                {
                    Customer customer = new Customer()
                    {
                        Name = request.Name,
                        UserName = request.Email,
                        Email = request.Email,
                        Code = GenarateCode(),
                        Discount = default
                    };

                    var result = await unitOfWork.Customers.CreateAsync(customer, request.Password);//создание пользователя 
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(customer, "User"); //добавление роли к пользователю
                        await Authenticate(customer, userManager, HttpContext);
                    }

                    await unitOfWork.SaveAsync();
                    return Ok(customer.Id);
                }
                return BadRequest("Email has been already taken: " + request.Email);
            }
            return BadRequest("Incorrect input");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            if (ModelState.IsValid)
            {
                var customer = await unitOfWork.Customers.GetAsync(request.Email);
                if (await userManager.CheckPasswordAsync(customer, request.Password))// проверка пароля
                {
                    await Authenticate(customer, userManager, HttpContext);
                    return Ok();
                }
                return BadRequest("Incorrect password");
            }
            return BadRequest("Incorrect request");
        }
        // [Authorize(AuthenticationSchemes="Cookies",Roles =("Manager, User"))]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
        public static async Task Authenticate(Customer customer, UserManager<Customer> userManager, HttpContext httpContext)
        {
            //создаю список клеймов
            List<Claim> claims = new List<Claim>
            {
                 new Claim(ClaimsIdentity.DefaultNameClaimType, customer.Email),
                  
            };
            //добавление в список всех ролей полльзователя
            var userRoles = await userManager.GetRolesAsync(customer);
            foreach (var userRole in userRoles)
            {
                claims.Add((new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole)));
            }
            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                "ApplicationCookie",
                // CookieAuthenticationDefaults.AuthenticationScheme, 
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        private string GenarateCode()
        {
            var day = DateTime.Now.Day;
            var mounth = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            return $"{day}{mounth}-{year}";
        }
    }
    
}
