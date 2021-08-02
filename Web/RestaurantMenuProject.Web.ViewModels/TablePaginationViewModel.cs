namespace RestaurantMenuProject.Web.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class TablePaginationViewModel
    {
        public int Page { get; set; }

        public ICollection<TableDisplayViewModel> Tables { get; set; } = new HashSet<TableDisplayViewModel>();

        public int OrdersPerPage { get; set; }

        public int OrdersCount { get; set; }

        public int PagesCount => (int)Math.Ceiling((double)this.OrdersCount / this.OrdersPerPage);

        public bool HasPreviousPage => this.Page > 1;

        public int PreviousPageNumber => this.Page - 1;

        public bool HasNextPage => this.Page < this.PagesCount;

        public int NextPageNumber => this.Page + 1;
    }
}
