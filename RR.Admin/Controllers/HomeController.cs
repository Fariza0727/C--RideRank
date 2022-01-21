using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Admin.Models;
using RR.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Subadmin, Admin")]
    [Authorize(Policy = "PagePermission")]
    public class HomeController : Controller
    {
        public static AppSettings _appSettings;
        private readonly IHostingEnvironment _environment;

        public HomeController(IOptions<AppSettings> appSettings,
            IHostingEnvironment environment)
        {
            _appSettings = appSettings.Value;
            _environment = environment;
        }
     
        public IActionResult Index()
        {
            
            var usernM = HttpContext.User.Identity.Name;


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Test()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Test(IFormFile Image)
        {
            try
            {
                if (Image != null && Image.Length != 0)
                {
                    //byte[] fileBytes;

                    TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                    string createdFileName = span.TotalSeconds + Image.FileName.Substring(Image.FileName.LastIndexOf("."));
                    var path = Path.Combine(@"\\172.24.32.172\d$\WebSite\wwwroot\images\BannerImages\", createdFileName);
                    NetworkCredential nc = new NetworkCredential("1056880-admin", "gti4eBrkwDTh");
                    //var stream = new FileStream(path, FileMode.Create);
                    //BinaryReader br = new BinaryReader(stream);
                    //byte[] image = br.ReadBytes((int)stream.Length);
                    //br.Close();
                    //stream.Close();
                    //using (var ms = new MemoryStream())
                    //{
                    //    Image.CopyTo(ms);
                    //    fileBytes = ms.ToArray();
                    //}

                    using (new NetworkConnection(@"\\172.24.32.172", nc))
                    {
                        var files = Directory.GetFiles(@"\\172.24.32.172\d$\WebSite\wwwroot\images\BannerImages\");
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            Image.CopyTo(stream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult Image(string name = "404.png")
        {
            string siteUrl = _appSettings.AdminSiteURL;
            try
            {
                var path = _environment.WebRootPath + @"\assets\BannerImage\" + name;
                if (System.IO.File.Exists(path))
                {
                    if (name.Contains(".jpg") || name.Contains(".jpeg"))
                        return File(System.IO.File.ReadAllBytes(path), "image/jpeg");
                    else
                        return File(System.IO.File.ReadAllBytes(path), "image/png");
                }
                else
                {
                    path = _environment.WebRootPath + @"\assets\BannerImage\404.png";
                    return File(System.IO.File.ReadAllBytes(path), "image/png");
                }
            }
            catch (FileNotFoundException e)
            {
                var path = _environment.WebRootPath + @"\assets\BannerImage\404.png";
                return File(System.IO.File.ReadAllBytes(path), "image/png");
            }
        }
    }
}
