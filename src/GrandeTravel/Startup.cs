using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;
using GrandeTravel.Services;
using GrandeTravel.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GrandeTravel
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            

            //string conn = @"Server=(localdb)\mssqllocaldb ;Database= GrandeTravel; Trusted_Connection=True";
            //services.AddDbContext<DBContextService>(options => options.UseSqlServer(conn));

            
            services.AddIdentity<ApplicationUser, IdentityRole>
            (
                config =>
                {
                    config.Password.RequiredLength = 5;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    //When new user registers the email should be unique
                    //config.User.RequireUniqueEmail = true; 
                    //To change default login page location:
                    config.Cookies.ApplicationCookie.LoginPath = "/Account/Login";

                }
            ).AddEntityFrameworkStores<DbContextService>();

            services.AddDbContext<DbContextService>();
            services.AddMvc();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseIdentity();


            app.UseMvcWithDefaultRoute();
            //app.UseMvc(routes =>
              //      routes.MapRoute(name: "default", template: "{controller=Packages}/{action=Index}/{id?}"));

        }
    }
}
