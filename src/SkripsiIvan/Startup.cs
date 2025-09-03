using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using SkripsiIvan.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace SkripsiIvan
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services - use controllers for .NET Core 3.0+
            services.AddControllersWithViews(); // Replaces AddMvc()

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/Home/Login";
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
                        options.LogoutPath = "/Home/LogOut";
                        options.AccessDeniedPath = "/Home/Error";
                    });

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            var connection = Configuration.GetConnectionString("DefaultConnection")
                ?? @"Data Source=localhost;Initial Catalog=SkripsiIvan;Integrated Security=True;TrustServerCertificate=true";

            services.AddDbContext<SkripsiIvanContext>(options =>
                options.UseSqlServer(connection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting(); // Add routing middleware

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}