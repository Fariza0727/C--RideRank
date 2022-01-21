using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RR.Admin.Models;
using RR.Core;

namespace RR.Admin.Controllers
{
    public class ComponentController : Controller
    {
        private readonly AppSettings _settings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ComponentController(IOptions<AppSettings> settings, IHostingEnvironment hostingEnvironment)
        {
            _settings = settings.Value;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public IActionResult GetLogs(string Date)
        {
            var date = Convert.ToDateTime(Date);
            var logfile_ = string.Concat(_hostingEnvironment.ContentRootPath, "/", _settings.Logfilepath.Replace("{date}", date.ToString("yyyyMMdd")).TrimStart('/'));
            if (System.IO.File.Exists(logfile_))
            {
                using (var fs = new FileStream(logfile_, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    string json = sr.ReadToEnd();
                    string jsonArray = string.Concat("[", json.TrimEnd(','), "]");
                    var log = JsonConvert.DeserializeObject<IEnumerable<LogResponse>>(jsonArray)
                                  .Where(r => r.Timestamp == date).SingleOrDefault();

                    return PartialView("_Logreport", log);
                }
            }

            return Json(new { status = false, message = "file not found" });
            
            
        }

        [HttpPost]
        [Route("getlogsreport/{date}")]
        public async Task<JsonResult> GetlogsReport(string date)
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            //Global search field
            var search = Request.Form["search[value]"];

            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            var sort = Request.Form["order[0][dir]"];
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);
            var logfile_ = string.Concat(_hostingEnvironment.ContentRootPath, "/", _settings.Logfilepath.Replace("{date}", DateTime.ParseExact(date,"yyyy-MM-dd",null).ToString("yyyyMMdd")).TrimStart('/'));
            if (System.IO.File.Exists(logfile_))
            {

                using (var fs = new FileStream(logfile_, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    string json = sr.ReadToEnd();
                    string jsonArray = string.Concat("[", json.TrimEnd(','), "]");
                    var allLogs = JsonConvert.DeserializeObject<IEnumerable<LogResponse>>(jsonArray)
                                  .Where(r=> (search == "" || r.Level.Contains(search) || r.MessageTemplate.Contains(search) || r.Timestamp.ToString().Contains(search))).OrderBy(r=>r.Timestamp);
                    var logs = allLogs.Skip((skip / pageSize) * pageSize)
                            .Take(pageSize);

                    return Json(new
                    {
                        draw = draw,
                        recordsFiltered = allLogs.Count(),
                        recordsTotal = allLogs.Count(),
                        data = logs
                    });

                }
            }
            return Json(new
            {
                draw = draw,
                recordsFiltered = 0,
                recordsTotal = 0,
                data = new object()
            });
        }

    }
}
