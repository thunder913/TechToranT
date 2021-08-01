namespace RestaurantMenuProject.Web.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class OrderViewModel
    {
        public int Page { get; set; }

        public ICollection<OrderInListViewModel> Orders { get; set; } = new HashSet<OrderInListViewModel>();

        public int OrdersPerPage { get; set; }

        public int OrdersCount { get; set; }

        public int PagesCount => (int)Math.Ceiling((double)this.OrdersCount / this.OrdersPerPage);

        public bool HasPreviousPage => this.Page > 1;

        public int PreviousPageNumber => this.Page - 1;

        public bool HasNextPage => this.Page < this.PagesCount;

        public int NextPageNumber => this.Page + 1;
    }
}
