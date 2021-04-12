// ReSharper disable VirtualMemberCallInConstructor
namespace RestaurantMenuProject.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Identity;
    using RestaurantMenuProject.Data.Common.Models;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public virtual ICollection<UserLike> Likes { get; set; } = new HashSet<UserLike>();

        public virtual ICollection<UserDislike> Dislikes { get; set; } = new HashSet<UserDislike>();

        public virtual Basket Basket { get; set; }

        public string BasketId { get; set; }

        [PersonalDataAttribute]
        public string FirstName { get; set; }

        [PersonalDataAttribute]
        public string LastName { get; set; }

        public ICollection<Order> WaiterOrders { get; set; } = new HashSet<Order>();
    }
}
