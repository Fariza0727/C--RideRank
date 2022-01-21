using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    [ViewComponent(Name = "PointTableComponent")]
    public class PointTableComponent : ViewComponent
    {
        #region Constructor

        private IConfiguration configuration;
        private readonly IPointDataService _pointservice;

        public PointTableComponent(IPointDataService pointservice, IConfiguration config)
        {
            _pointservice = pointservice;
            configuration = config;
        }

        #endregion

        /// <summary>
        /// Point Table Data
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var pointData = await _pointservice.GetAllPointTableData();
            return View(pointData);
        }
    }
}
