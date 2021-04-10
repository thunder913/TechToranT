using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RestaurantMenuProject.Web.ViewModels
{
    public class EditPromoCodeViewModel : AddPromoCodeViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }
    }
}
