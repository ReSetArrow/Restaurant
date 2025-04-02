using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Restaurant.Filters;
using Restaurant.Models;

namespace Restaurant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<RestaurantContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantConnection")));

            builder.Services.AddScoped<LoginStatusFilter>();

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<LogFilter>();
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithReExecute("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
           
            app.UseAuthorization();

            app.UseSession();
            app.MapRazorPages();
            app.MapControllers(); 

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=TableList}/{action=Cart}/{id?}");
            app.Run();
        }
    }
}
