using DataAccess;
using DataAccess.Repos;
using DataAccess.Repos.IRepos;
using DbInitailizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Stripe;
using Utility;

namespace Airline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            builder.Services.AddDbContext<ApplicationDbContext>(
                option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                );

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });

            builder.Services.AddScoped<IDbInitialize, DbInitialize>();
            builder.Services.AddScoped<IApplicationUserRepo, ApplicationUserRepo>();
            builder.Services.AddScoped<ICountryRepo, CountryRepo>();
            builder.Services.AddScoped<ICityRepo, CityRepo>();
            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<ICityCategoryRepo, CityCategoryRepo>();
            builder.Services.AddScoped<ITicketRepo, TicketRepo>();
            builder.Services.AddScoped<ITripRepo, TripRepo>();
            builder.Services.AddScoped<ICompanyRepo, CompanyRepo>();
            builder.Services.AddScoped<IRequestCompanyRepo, RequestCompanyRepo>();
            builder.Services.AddScoped<ISeatRepo, SeatRepo>();
            builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Begin}/{id?}");

            app.Run();
        }
    }
}
