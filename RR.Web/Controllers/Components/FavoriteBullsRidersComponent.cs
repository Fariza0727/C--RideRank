using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RR.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Web.Controllers.Components
{
    [ViewComponent(Name = "FavoriteBullsRidersComponent")]
    public class FavoriteBullsRidersComponent : ViewComponent
    {
        private readonly IFavoriteBullsRidersService _FavoriteBullsRiders;
        private readonly IHttpContextAccessor _httpContext;

        public FavoriteBullsRidersComponent(IFavoriteBullsRidersService favoriteBullsRiders, IHttpContextAccessor httpContext)
        {
            _FavoriteBullsRiders = favoriteBullsRiders;
            _httpContext = httpContext;
        }

        /// <summary>
        /// Get Favorite Bulls/Riders of User
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var datamodel = await _FavoriteBullsRiders.GetUserFavoriteBullsRiders(userId);
            return View(datamodel);
        }
    }
}
