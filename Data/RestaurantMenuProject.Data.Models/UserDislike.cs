using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantMenuProject.Data.Models
{
    public class UserDislike
    {
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int CommentId { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
