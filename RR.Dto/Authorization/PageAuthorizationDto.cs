using Microsoft.AspNetCore.Authorization;

namespace RR.Dto
{
    public class PageAuthorizationDto : IAuthorizationRequirement
    {
        public string PageName { get; set; }
        public string UserId { get; set; }
    }
}
