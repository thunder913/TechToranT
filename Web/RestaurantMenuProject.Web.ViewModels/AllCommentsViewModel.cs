namespace RestaurantMenuProject.Web.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class AllCommentsViewModel
    {
        public int Page { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; } = new HashSet<CommentViewModel>();

        public int CommentPerPage { get; set; }

        public int CommentCount { get; set; }

        public int PagesCount => (int)Math.Ceiling((double)this.CommentCount / this.CommentPerPage);

        public bool HasPreviousPage => this.Page > 1;

        public int PreviousPageNumber => this.Page - 1;

        public bool HasNextPage => this.Page < this.PagesCount;

        public int NextPageNumber => this.Page + 1;
    }
}
