using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.Email;
using RR.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : BaseController
    {
        #region Constructor

        private readonly IEmailSender _emailSender;
        private readonly ICMSService _cmsService;
        private readonly INewsService _newsService;
        private readonly INewsLetterService _newsLetterService;

        public HomeController(ICMSService cmsService,
                              INewsService newsService,
                              IConfiguration config,
                              INewsLetterService newsLetterService,
                              IEmailSender emailSender,
                        IOptions<AppSettings> appSettings) :
        base(appSettings)
        {
            _cmsService = cmsService;
            _newsService = newsService;
            _newsLetterService = newsLetterService;
            _emailSender = emailSender;
        }

        #endregion

        [HttpPost]
        [Route("contact-us")]
        public async Task<OkObjectResult> PostContactUs([FromForm] ContactDto model)
        {
            try
            {
                string toEmail = _appSettings.ToEmailAddress;
                string emailBody = Utilities.GetEmailTemplateValue("Contactus/Body");
                string emailSubject = Utilities.GetEmailTemplateValue("Contactus/Subject");
                emailBody = emailBody.Replace("@@@title", "Rankride Enquiry");
                emailBody = emailBody.Replace("@@@Name", model.Name);
                emailBody = emailBody.Replace("@@@Phone", model.Phone);
                emailBody = emailBody.Replace("@@@Email", model.Email);
                emailBody = emailBody.Replace("@@@Message", model.Message);
                await _emailSender.SendEmailAsync(toEmail, emailSubject, emailBody);
                return Ok(new ApiResponse
                {
                    Data = "",
                    IpAddress = Helpers.GetIpAddress(),
                    Message = "Thank you. Your query has been submitted successfully. We will respond back as soon as possible.",
                    Success = true,
                    RedirectPath = ""
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Data = "",
                    IpAddress = Helpers.GetIpAddress(),
                    Message = ex.Message,
                    Success = false,
                    RedirectPath = ""
                });
            }

        }

        [HttpPost]
        [Route("news-subscribe")]
        public async Task<OkObjectResult> PostSubscribe([FromForm] SubscribeDto model)
        {
            try
            {
                await _newsLetterService.AddNewsLetter(model);
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Data = "",
                    IpAddress = Helpers.GetIpAddress(),
                    Message = ex.Message,
                    Success = false,
                    RedirectPath = ""
                });
            }
            return Ok(new ApiResponse
            {
                Data = "",
                IpAddress = Helpers.GetIpAddress(),
                Message = "Success",
                Success = true,
                RedirectPath = ""
            });
        }

        [HttpGet]
        [Route("news")]
        public async Task<OkObjectResult> GetNews()
        {
            ViewBag.NewsPicPath = _appSettings.AdminSiteURL + "assets/NewsImage/";
            IEnumerable<NewsDto> model = await _newsService.GetNews();
            model  = model.Select(r => AddPicPath(r));
            return Ok(new ApiResponse
            {
                Data = model,
                Message = "News",
                RedirectPath = "",
                IpAddress = Helpers.GetIpAddress(),
                Success = true
            });
        }

        [HttpPost]
        [Route("news")]
        public async Task<OkObjectResult> GetNewsDetail([FromForm]string title, [FromForm]int id)
        {
            title = title.Replace("-", " ");
            NewsDto newsDto = await _newsService.GetNewsDetail(title, id);
            newsDto = AddPicPath(newsDto);
            return Ok(new ApiResponse
            {
                Data = newsDto,
                Message = "News Detail",
                RedirectPath = "",
                IpAddress = Helpers.GetIpAddress(),
                Success = true
            });
        }

        [HttpGet]
        [Route("gettoprecentnews")]
        public async Task<OkObjectResult> GetTopRecentNews()
        {
            var newsResult = await _newsService.GetTopRecentNews();
            IEnumerable<NewsDto> recent = newsResult.Item1;
            IEnumerable<NewsDto> top = newsResult.Item2;
            recent = recent.Select(r => AddPicPath(r));
            top = recent.Select(r => AddPicPath(r));

            ViewBag.NewsPicPath = _appSettings.MainSiteURL + _appSettings.NewsSharedPath;
            return Ok(new ApiResponse
            {
                Data = new Tuple<IEnumerable<NewsDto>, IEnumerable<NewsDto>>(recent, top),
                IpAddress = Helpers.GetIpAddress(),
                Success = true,
                Message = ""
            });
        }

        private NewsDto AddPicPath(NewsDto item)
        {
            if(!string.IsNullOrEmpty(item.PicPath))
                item.PicPath  = string.Concat(_appSettings.MainSiteURL, _appSettings.NewsSharedPath, item.PicPath);

            if (!string.IsNullOrEmpty(item.VideoPath))
                item.VideoPath = string.Concat(_appSettings.MainSiteURL, _appSettings.NewsSharedPath, item.VideoPath);


            return item;
        }

    }
}