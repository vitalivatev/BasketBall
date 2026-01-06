using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVC.Intro.Data;
using MVC.Intro.Services;
using MVC.Intro.Models;

namespace MVC.Intro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = Environment.GetFolderPath(folder);
                var dbPath = Path.Join(path, "products.db");
                options.UseSqlite($"Data Source={dbPath}");
            });
            builder.Services
                .AddIdentity<Users, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddControllersWithViews();
            builder.Services.AddTransient<ProductService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "routing",
                pattern: "{controller=Routing}/{*default}",
                defaults: new { controller = "Routing", action = "Default" });

            app.Run();
        }
    }
}
