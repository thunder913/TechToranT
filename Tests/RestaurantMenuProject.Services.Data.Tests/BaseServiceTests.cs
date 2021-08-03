namespace RestaurantMenuProject.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Moq;
    using RestaurantMenuProject.Data;
    using RestaurantMenuProject.Data.Common;
    using RestaurantMenuProject.Data.Common.Repositories;
    using RestaurantMenuProject.Data.Models;
    using RestaurantMenuProject.Data.Models.Enums;
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

        protected async Task PopulateDB()
        {
            await this.PopulateIngredientsAndTypes();
            await this.PopulateUsers();
            await this.PopulateDishes();
            await this.PopulateDrinks();
            await this.PopulateComments();
        }

        protected async Task PopulateIngredientsAndTypes()
        {
            this.DbContext.Ingredients.Add(new Ingredient()
            {
                Name = "test1",
                Id = 1,
            });

            this.DbContext.Ingredients.Add(new Ingredient()
            {
                Name = "test2",
                Id = 2,
            });

            this.DbContext.DishTypes.Add(new DishType()
            {
                Id = 1,
                Name = "test",
            });

            this.DbContext.DishTypes.Add(new DishType()
            {
                Id = 2,
                Name = "test2",
            });

            await this.DbContext.SaveChangesAsync();

            this.DbContext.DrinkTypes.Add(new DrinkType()
            {
                Id = 1,
                Name = "test",
            });

            this.DbContext.DrinkTypes.Add(new DrinkType()
            {
                Id = 2,
                Name = "test2",
            });

            this.DbContext.PackagingTypes.Add(new PackagingType()
            {
                Id = 1,
                Name = "test1",
            });

            this.DbContext.PackagingTypes.Add(new PackagingType()
            {
                Id = 2,
                Name = "test2",
            });

            await this.DbContext.SaveChangesAsync();
        }

        protected async Task PopulateUsers()
        {
            var role1 = new ApplicationRole()
            {
                Id = "role1",
                Name = "role1",
            };
            var role2 = new ApplicationRole()
            {
                Id = "role2",
                Name = "role2",
            };
            var role3 = new ApplicationRole()
            {
                Id = "role3",
                Name = "role3",
            };

            await this.DbContext.Roles.AddAsync(role1);
            await this.DbContext.Roles.AddAsync(role2);
            await this.DbContext.Roles.AddAsync(role3);
            await this.DbContext.SaveChangesAsync();

            await this.DbContext.Users.AddAsync(new ApplicationUser()
            {
                Id = "user1",
                FirstName = "first1",
                LastName = "last1",
                Email = "first@aaa.bg",
                PhoneNumber = "111111",
                Roles = new List<IdentityUserRole<string>>() { new IdentityUserRole<string>() { UserId = "user1", RoleId = "role1", } },
            });
            await this.DbContext.Users.AddAsync(new ApplicationUser()
            {
                Id = "user2",
                FirstName = "first2",
                LastName = "last2",
                Email = "second@aaa.bg",
                PhoneNumber = "222222",
                Roles = new List<IdentityUserRole<string>>() { new IdentityUserRole<string>() { UserId = "user2", RoleId = "role2", }, new IdentityUserRole<string>() { UserId = "user2", RoleId = "role3", } },
            });
            await this.DbContext.Users.AddAsync(new ApplicationUser()
            {
                Id = "user3",
                FirstName = "first3",
                LastName = "last3",
                Email = "third@aaa.bg",
                PhoneNumber = "333333",
                Roles = new List<IdentityUserRole<string>>() { new IdentityUserRole<string>() { UserId = "user3", RoleId = "role3", } },
            });

            await this.DbContext.SaveChangesAsync();
        }

        protected async Task PopulateDishes()
        {
            var firstIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1 || x.Id == 2).ToList();
            var secondIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1).ToList();
            await this.DbContext.Dishes.AddAsync(
                new Dish()
                {
                    Id = "test1",
                    Name = "test1",
                    AdditionalInfo = "test1",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test1", Extension = ImageExtension.jpeg },
                    DishTypeId = 1,
                    Ingredients = firstIngredients,
                    PrepareTime = 20,
                });

            await this.DbContext.Dishes.AddAsync(
                new Dish()
                {
                    Id = "test2",
                    Name = "test2",
                    AdditionalInfo = "test2",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test2", Extension = ImageExtension.jpeg },
                    DishTypeId = 1,
                    Ingredients = firstIngredients,
                    PrepareTime = 20,
                });

            await this.DbContext.SaveChangesAsync();
        }

        protected async Task PopulateDrinks()
        {
            var firstIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1 || x.Id == 2).ToList();
            var secondIngredients = this.DbContext.Ingredients.Where(x => x.Id == 1).ToList();
            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test3",
                    Name = "test1",
                    AdditionalInfo = "test1",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test5", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 1,
                });

            await this.DbContext.Drinks.AddAsync(
                new Drink()
                {
                    Id = "test4",
                    Name = "test2",
                    AdditionalInfo = "test2",
                    Weight = 50,
                    Price = 10,
                    Image = new Image() { Id = "test6", Extension = ImageExtension.jpeg },
                    DrinkTypeId = 1,
                    Ingredients = firstIngredients,
                    AlchoholByVolume = 20,
                    PackagingTypeId = 2,
                });

            await this.DbContext.SaveChangesAsync();
        }

        protected async Task PopulateComments()
        {
            await this.DbContext.Comments
                .AddAsync(new Comment()
                {
                    Id = 1,
                    CommentText = "test",
                    Rating = 3,
                    CommentedById = "user2",
                    DishId = "test1",
                });

            await this.DbContext.Comments
                .AddAsync(new Comment()
                {
                    Id = 2,
                    CommentText = "test2",
                    Rating = 3,
                    CommentedById = "user2",
                    DishId = "test1",
                });

            await this.DbContext.Comments
                .AddAsync(new Comment()
                {
                    Id = 3,
                    CommentText = "test2",
                    Rating = 3,
                    CommentedById = "user2",
                    DrinkId = "test3",
                });

            await this.DbContext.SaveChangesAsync();
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
            services.AddTransient<IUserDislikeService, UserDislikeService>();
            services.AddTransient<IUserLikeService, UserLikeService>();

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
