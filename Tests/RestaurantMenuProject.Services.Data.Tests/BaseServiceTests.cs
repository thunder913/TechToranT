namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Moq;
    using RestaurantMenuProject.Data;
    using RestaurantMenuProject.Data.Common;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Repositories;
    using RestaurantMenuProject.Services.Data.Contracts;
    using RestaurantMenuProject.Services.Mapping;
    using RestaurantMenuProject.Services.Messaging;
    using RestaurantMenuProject.Web.ViewModels;

    public abstract class BaseServiceTests : IDisposable
    {
        protected BaseServiceTests()
        {
            var services = this.SetServices();

            this.ServiceProvider = services.BuildServiceProvider();
            this.DbContext = this.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        protected IServiceProvider ServiceProvider { get; set; }

        protected ApplicationDbContext DbContext { get; set; }

        public void Dispose()
        {
            this.DbContext.Database.EnsureDeleted();
            this.SetServices();
        }

        protected IFormFile GetFile(string name)
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = $"{name}.jpeg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            var file = fileMock.Object;

            return file;
        }

        private ServiceCollection SetServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<ApplicationUser, ApplicationRole>(opts =>
            {
                opts.SignIn.RequireConfirmedAccount = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();


            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            // Application services
            // TODO add configuration to the unit tests
            // services.AddTransient<IEmailSender>(serviceProvider => new SendGridEmailSender(this.configuration["SendGrid:ApiKey"]));
            services.AddTransient(typeof(ILogger<>), typeof(Logger<>));
            services.AddTransient(typeof(ILoggerFactory), typeof(LoggerFactory));
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
            services.AddTransient<IOrderDishService, OrderDishService>();
            services.AddTransient<IOrderDrinkService, OrderDrinkService>();
            services.AddTransient<ICommentService, CommentService>();

            // AutoMapper
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            // SignalR Setup
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });

            return services;
        }
    }
}
