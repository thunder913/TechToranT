using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantMenuProject.Web.Data;

[assembly: HostingStartup(typeof(RestaurantMenuProject.Web.Areas.Identity.IdentityHostingStartup))]
namespace RestaurantMenuProject.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<RestaurantMenuProjectWebContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("RestaurantMenuProjectWebContextConnection")));

            });
        }
    }
}