namespace RestaurantMenuProject.Web.ViewModels
{
    using System;

    public class TableDisplayViewModel
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public int Capacity { get; set; }

        public string Code { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateGenerated { get; set; }
    }
}
