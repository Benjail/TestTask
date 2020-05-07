using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTask.Repositories;
using TestTask.Models;
using TestTask.Database;
using TestTask.Services;
using Newtonsoft.Json;

namespace TestTask
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config => config.EnableEndpointRouting = false)
                .AddNewtonsoftJson(opt=>opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddDbContext<AppDbContext>(config =>
                config.UseSqlServer(Configuration.GetConnectionString("TestTaskDb")
             //   b => b.MigrationsAssembly("MishaTask")
                ));

            services.AddIdentity<Customer, IdentityRole<Guid>>(config => //добавление сервиса Identity и настройка параметров Identity
            {
                config.Password.RequiredLength = 0;
                config.Password.RequireDigit = false;                                   
                config.Password.RequireLowercase = false;
                config.Password.RequiredUniqueChars = 0;                        
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<AppDbContext>();

            //добавление и настройка параметров аутентификации
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) 
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
                {
                    config.LoginPath = new Microsoft.AspNetCore.Http.PathString("/auth/login");
                    config.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/auth/logout");
                });
            services.AddAuthorization();
            services.AddScoped<Generator>();
            services.AddScoped<UnitOfWork>(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider) //метод встраивает в конвейер обработки запросов компоненты (Middleware)
        {                                                                                                  //выполняется только 1 раз при запуске приложения 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                                      
            }
            //кросс-доменные запросы
            app.UseCors(build =>
            {
                build.WithOrigins("http://localhost:1841"); 
                build.AllowAnyMethod();                                                             
                build.AllowAnyHeader(); 
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvc();
        }
    }
}
