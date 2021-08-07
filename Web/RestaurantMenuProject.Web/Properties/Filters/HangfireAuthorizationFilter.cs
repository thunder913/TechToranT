using Hangfire.Annotations;
using Hangfire.Dashboard;
using RestaurantMenuProject.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantMenuProject.Web.Properties.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole(GlobalConstants.AdministratorRoleName);
        }
    }
}
