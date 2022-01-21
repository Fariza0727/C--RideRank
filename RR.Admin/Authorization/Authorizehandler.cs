using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RR.AdminService;
using RR.Dto;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RR.Admin.Authorization
{
    public class Authorizehandler : AuthorizationHandler<PageAuthorizationDto>
    {
        #region Constructor
        private readonly IUserService _userService;
        private readonly IPageDetailService _pagePermission;
        private readonly UserManager<IdentityUser> _userManager;

        public Authorizehandler(IUserService userService, IPageDetailService pagePermission, UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _pagePermission = pagePermission;
            _userManager = userManager;
        }
        #endregion

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PageAuthorizationDto requirement)
        {
            if (context.User.Identity.Name != null)
            {
                if (!context.User.IsInRole("Admin"))
                {
                    bool flag = true;
                    string userName = context.User.Identity.Name;
                    var mvcContext = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;
                    var sdf = getRequest(mvcContext);
                    var requestPath = mvcContext.HttpContext.Request.Path.Value.ToString().ToLower();
                    requestPath = Regex.Replace(requestPath, @"\d", "").TrimEnd('/');

                    var userdetail = _userManager.FindByEmailAsync(userName).Result;
                    List<PageDto> permittedPages = _pagePermission.GetPermitedPages(userdetail.Id.ToString()).Result;

                    foreach (var item in permittedPages)
                    {
                        if (item.Selected == true && item.PageUrl.ToLower().TrimEnd('/').Contains(requestPath))
                        {
                            flag = false;
                        }

                    }
                    if (flag)
                    {
                        mvcContext.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    }

                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        private Dictionary<string,string> getRequest(Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext context)
        {
            var paterm_ = string.Join('/', context.RouteData.Values.Keys);
            var value = string.Join('/', context.RouteData.Values.Values);

            return new Dictionary<string, string>();
        } 
    }
}
