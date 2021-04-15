namespace RestaurantMenuProject.Web
{
    using System.Reflection;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using RestaurantMenuProject.Data;
    using RestaurantMenuProject.Data.Common;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Repositories;
    using RestaurantMenuProject.Data.Seeding;
    using RestaurantMenuProject.Services.Data;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Services.Messaging;
    using RestaurantMenuProject.Web.Hubs;
    using RestaurantMenuProject.Web.ViewModels;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            // Make it to yes in production
            services.AddDefaultIdentity<ApplicationUser>(opts =>
            {
                opts.SignIn.RequireConfirmedAccount = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireDigit = false;
            }).AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();


            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddControllersWithViews(
                options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    }).AddRazorRuntimeCompilation();
            services.AddRazorPages();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddAntiforgery(option =>
            {
                option.HeaderName = "X-CSRF-TOKEN";
            });
            services.AddSingleton(this.configuration);

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender>(serviceProvider => new SendGridEmailSender(this.configuration["SendGrid:ApiKey"]));
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IDishTypeService, DishTypeService>();
            services.AddTransient<IDrinkTypeService, DrinkTypeService>();
            services.AddTransient<IIngredientService, IngredientService>();
            services.AddTransient<IDishService, DishService>();
            services.AddTransient<IDrinkService, DrinkService>();
            services.AddTransient<IPackagingService, PackagingService>();
            services.AddTransient<IAllergenService, AllergenService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IBasketService, BasketService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<ITableService, TableService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IPromoCodeService, PromoCodeService>();
            services.AddTransient<IPickupItemService, PickupItemService>();

            services.Configure<AuthMessageSenderOptions>(this.configuration);
            services.AddSignalR();
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = this.configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = this.configuration["Authentication:Facebook:AppSecret"];
            })
            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection =
                    this.configuration.GetSection("Authentication:Google");

                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("drinkRoute", "Menu/Drinks/{type}/{id?}", new { controller = "Menu", action = "DisplayDrink" });
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("menuRoute", "Menu/{type}/{id?}", new { controller = "Menu", action = "DisplayFood", });
                        endpoints.MapHub<OrderHub>("/orderHub");
                        endpoints.MapRazorPages();
                    });
        }
    }
}
