namespace RestaurantMenuProject.Data
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using RestaurantMenuProject.Data.Common.Models;
    using RestaurantMenuProject.Data.Models;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
            typeof(ApplicationDbContext).GetMethod(
                nameof(SetIsDeletedQueryFilter),
                BindingFlags.NonPublic | BindingFlags.Static);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Allergen> Allergens { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Dish> Dishes { get; set; }

        public DbSet<Drink> Drinks { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDish> OrdersDishes { get; set; }

        public DbSet<PromoCode> PromoCodes { get; set; }

        public DbSet<OrderDrink> OrderDrinks { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<DishType> DishTypes { get; set; }

        public DbSet<DrinkType> DrinkTypes { get; set; }

        public DbSet<PackagingType> PackagingTypes { get; set; }

        public DbSet<Basket> Baskets { get; set; }

        public DbSet<BasketDrink> BasketsDrinks { get; set; }
        public DbSet<UserLike> UsersLikes { get; set; }

        public DbSet<UserDislike> UsersDislikes { get; set; }
        public DbSet<BasketDish> BasketsDishes { get; set; }

        public override int SaveChanges() => this.SaveChanges(true);

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            this.SaveChangesAsync(true, cancellationToken);

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            this.ApplyAuditInfoRules();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<OrderDish>()
                .HasKey(x => new { x.DishId, x.OrderId });

            builder.Entity<OrderDrink>()
               .HasKey(x => new { x.DrinkId, x.OrderId });

            builder.Entity<UserLike>()
               .HasKey(x => new { x.UserId, x.CommentId });

            builder.Entity<UserDislike>()
               .HasKey(x => new { x.UserId, x.CommentId });

            builder.Entity<UserLike>()
               .HasOne(x => x.Comment)
               .WithMany(x => x.Likes);

            builder.Entity<UserLike>()
               .HasOne(x => x.Comment)
               .WithMany(x => x.Likes);

            builder.Entity<UserDislike>()
               .HasOne(x => x.User)
               .WithMany(x => x.Dislikes);

            builder.Entity<UserDislike>()
               .HasOne(x => x.User)
               .WithMany(x => x.Dislikes);

            builder.Entity<Comment>()
                .HasOne(x => x.CommentedBy)
                .WithMany(x => x.Comments);

            builder.Entity<Order>()
                .HasOne(x => x.Client)
                .WithMany(x => x.Orders);

            builder.Entity<BasketDish>()
                .HasKey(x => new { x.BasketId, x.DishId });

            builder.Entity<BasketDrink>()
                .HasKey(x => new { x.BasketId, x.DrinkId });

            builder.Entity<Basket>()
                .HasOne(x => x.User)
                .WithOne(x => x.Basket);

            builder.Entity<BasketDish>()
                .HasOne(x => x.Basket)
                .WithMany(x => x.Dishes);

            builder.Entity<BasketDrink>()
                .HasOne(x => x.Basket)
                .WithMany(x => x.Drinks);

            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            this.ConfigureUserIdentityRelations(builder);

            EntityIndexesConfiguration.Configure(builder);

            var entityTypes = builder.Model.GetEntityTypes().ToList();

            // Set global query filter for not deleted entities only
            var deletableEntityTypes = entityTypes
                .Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
            foreach (var deletableEntityType in deletableEntityTypes)
            {
                var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
                method.Invoke(null, new object[] { builder });
            }

            // Disable cascade delete
            var foreignKeys = entityTypes
                .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
            where T : class, IDeletableEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }

        // Applies configurations
        private void ConfigureUserIdentityRelations(ModelBuilder builder)
             => builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

        private void ApplyAuditInfoRules()
        {
            var changedEntries = this.ChangeTracker
                .Entries()
                .Where(e =>
                    e.Entity is IAuditInfo &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in changedEntries)
            {
                var entity = (IAuditInfo)entry.Entity;
                if (entry.State == EntityState.Added && entity.CreatedOn == default)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                else
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}
