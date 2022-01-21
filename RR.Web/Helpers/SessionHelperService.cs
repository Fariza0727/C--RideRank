using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RR.Dto;
using System.Collections.Generic;

namespace RR.Web.Helpers
{
    public class SessionHelperService
    {
        private readonly IHttpContextAccessor _accessor;

        public SessionHelperService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public HttpContext HttpContext
        {
            get
            {
                return _accessor.HttpContext;
            }
        }

        public BecomeAPlayerDto UserDetail
        {
            get
            {
                var userDetail = _accessor.HttpContext.Session.GetString("UserDetail");

                if (!string.IsNullOrEmpty(userDetail))
                {
                    return JsonConvert.DeserializeObject<BecomeAPlayerDto>(userDetail);
                }

                return null;
            }
            set
            {
                var userDetail = JsonConvert.SerializeObject(value);
                _accessor.HttpContext.Session.SetString("UserDetail", userDetail);
            }
        }

        public string UserId
        {
            get
            {
                return _accessor.HttpContext.Session.GetString("UserId");
            }
            set
            {
                _accessor.HttpContext.Session.SetString("UserId", value);
            }
        }
        
    }
}
