using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RR.Admin.Models;
using RR.AdminService;
using RR.Core;
using RR.Dto.Award;
using RR.Dto.AwardType;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
    public class AwardManagementController : BaseController
    {
        #region Constructor

        private readonly IAwardTypeService _awardTypeService;
        private readonly IAwardService _awardService;
        private readonly IHostingEnvironment _environment;

        public AwardManagementController(IAwardTypeService awardTypeService,
            IAwardService awardService,
           IHostingEnvironment environment,
           IOptions<AppSettings> appSettings) :
              base(appSettings)
        {
            _awardTypeService = awardTypeService;
            _awardService = awardService;
            _environment = environment;
        }

        #endregion

        #region Award Type Management
        
        [Route("award-type")]
        [Authorize(Policy = "PagePermission")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("awards/allawardtypes")]        
        public async Task<JsonResult> GetAllAwardTypes()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            //Global search field
            var search = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            var sort = Request.Form["order[0][dir]"];

            var awardTypes = await _awardTypeService.GetAllAwardType(skip / pageSize, pageSize, search, sort);

            var data = awardTypes.Item1;
            int recordsTotal = awardTypes.Item2;

            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            });
        }

        
        [Route("addeditawardtype/{id}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> AddEditAwardType(int id)
        {
            AwardTypeDto model = new AwardTypeDto();
            model.Id = id;
            if (id > 3)
            {
                model = await _awardTypeService.GetAwardType(id);
            }
            return PartialView(model);
        }

        [HttpPost]
        [Route("addeditawardtype/{id}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> AddEditAwardType(AwardTypeDto model)
        {
            int id = model.Id;
            if (ModelState.IsValid)
            {
                if (model.Id < 4 && model.Id != 0)
                {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> requested award type can not be updated.</div>");
                }
                else
                {
                    model.CreatedBy = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    await _awardTypeService.AddEditAwardType(model);
                    if (id == 0)
                        return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> award type has been added successfully.</div>");
                    else
                        return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> award type has been edited successfully.</div>");
                }
            }
            return PartialView();
        }

        [Route("deleteawardtype/{id}")]
        [Authorize(Policy = "PagePermission")]        
        public async Task<IActionResult> DeleteAwardType(int id)
        {
            AwardTypeDto model = new AwardTypeDto();
            model.Id = id;
            if (id > 3 && id != 5)
            {
                var isAssociate = await _awardTypeService.CheckAwardTypeAssociateWithAward(id);
                if (!isAssociate)
                {
                    await _awardTypeService.DeleteAwardType(id);
                    return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> award type has been deleted successfully.</div>");
                }
                else
                {
                    return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> requested award type can not be deleted because it is associated with award.</div>");
                }
            }
            else
            {
                return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> requested award type can not be deleted.</div>");
            }
        }

        #endregion

        #region Award Management

        [Route("awards")]
        [Authorize(Policy = "PagePermission")]        
        public IActionResult Awards()
        {
            return View();
        }

        [Route("addeditaward/{id}")]
        [Authorize(Policy = "PagePermission")]        
        public async Task<IActionResult> AddEditAward(int id)
        {
            AwardDto model = new AwardDto();
            model.Id = id;
            if (id > 0)
            {
                model = await _awardService.GetAward(id);
            }
            var types = await _awardTypeService.GetAwardTypes();
            model.AwardTypes = types.Where(x => x.Name != "Cash" && x.Name != "Token").Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            model.AwardTypes.Insert(0, new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text = "Select award type", Value = "" });
            return PartialView(model);
        }

        [HttpPost]
        [Route("addeditaward/{id}")]
        [Authorize(Policy = "PagePermission")]        
        public async Task<IActionResult> AddEditAward(AwardDto model, IFormCollection form)
        {
            long Id = model.Id;
            try
            {
                string fileName = "";
                AwardDto awardDto = new AwardDto();
                if (Id > 0)
                {
                    awardDto = await _awardService.GetAward(model.Id);
                    fileName = awardDto != null ? !string.IsNullOrEmpty(awardDto.Image) ? awardDto.Image : "" : "";
                }
                if (form.Files.Count > 0)
                {
                    #region Save profile pic

                    fileName = DateTime.Now.Ticks + "_" + form.Files[0].FileName;

                    string imagePath = _appSettings.AwardPicPath;

                    var path = Path.Combine(_environment.ContentRootPath, imagePath, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await form.Files[0].CopyToAsync(stream);
                    }

                    #endregion
                }
                model.Image = fileName;
                if (Id == 0)
                {
                    model.CreatedBy = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    model.CreatedDate = DateTime.Now;
                }
                await _awardService.AddEditAward(model);
                if (Id == 0)
                    return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> award has been added successfully.</div>");
                else
                    return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> award has been edited successfully.</div>");

            }
            catch (Exception)
            {
                return Content("<div class='alert alert-danger alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Oops!!!</strong> Some thing went wrong. Requested award can not be updated.</div>");
            }

        }

        [HttpPost]
        [Route("awards/allawards")]        
        public async Task<JsonResult> GetAllAwards()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            //Global search field
            var search = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            var sort = Request.Form["order[0][dir]"];
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);

            var awards = await _awardService.GetAllAward(skip / pageSize, pageSize, column, search, sort);

            var data = awards.Item1;
            int recordsTotal = awards.Item2;

            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            });
        }

        [Route("deleteaward/{id}")]
        [Authorize(Policy = "PagePermission")]        
        public async Task<IActionResult> DeleteAward(int id)
        {
            var isAssociate = await _awardService.CheckAwardAssociateWithContest(id);
            if (!isAssociate)
            {
                AwardTypeDto model = new AwardTypeDto();
                model.Id = id;
                await _awardService.DeleteAward(id);
                return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> award has been deleted successfully.</div>");
            }
            else
            {
                return Content("<div class='alert alert-danger alert-dismissible' role='alert'><strong>Oops!!!</strong> requested award can not be deleted because it is associated with contest winning.</div>");
            }
        }

        #endregion
    }
}