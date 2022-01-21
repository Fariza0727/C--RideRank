using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Core;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ThirdpartyApisController : BaseController
    {
        private readonly AppSettings _appSettings;

        #region Constructor

        public ThirdpartyApisController(IOptions<AppSettings> appSettings) :
              base(appSettings)
        {
            _appSettings = appSettings.Value;
        }

        #endregion

        [Route("apisresponse")]
        public IActionResult ApisResponse()
        {
            return View();
        }

        [Route("apisresponse")]
        [HttpPost]
        public async Task<Object> ApisResponse(Enums.ApiEnum api, string apiKey, string apiUrl ="")
        {
            string response = "";

            if (!string.IsNullOrEmpty(apiUrl))
            {
                var uri = new Uri(apiUrl);
                var qs = System.Web.HttpUtility.ParseQueryString(uri.Query);
                ViewBag.Key = qs["key"]??"";
                ViewBag.Data = qs["data"]??"";

                
                try
                {
                    response = await Core.WebProxy.APIResponse((string)ViewBag.Data, (string)ViewBag.Key);
                }
                catch (Exception ex)
                {
                    response = ex.Message;
                }    
                
            }

            if (api > 0 && !string.IsNullOrEmpty("apiKey"))
            {
                try
                {
                    switch (api)
                    {
                        case Enums.ApiEnum.GetBullsRecord:
                            response = await Core.WebProxy.APIResponse("seedbulls", apiKey);
                            break;
                        case Enums.ApiEnum.GetRidersRecord:
                            response = await Core.WebProxy.APIResponse("seedriders", apiKey);
                            break;
                        case Enums.ApiEnum.GetPastEventRecord:
                            response = await Core.WebProxy.APIResponse("events_past", apiKey);
                            break;
                        case Enums.ApiEnum.GetFutureEventRecord:
                            response = await Core.WebProxy.APIResponse("events_future_full", apiKey);
                            break;
                        case Enums.ApiEnum.GetCurrentEventRecord:
                            response = await Core.WebProxy.APIResponse("event_current", apiKey);
                            break;
                        case Enums.ApiEnum.VelocityLevelEvents:
                            response = await Core.WebProxy.APIResponse("events_future_velo", apiKey);
                            break;
                    }

                    
                }
                catch (Exception ex)
                {
                    response = ex.Message;
                }
            }

            if (response != "false")
            {
                return await Task.FromResult(response);
            }
            else
            {
                return await Task.FromResult(Json(new { message = response }));
            }

        }

        

    }



}