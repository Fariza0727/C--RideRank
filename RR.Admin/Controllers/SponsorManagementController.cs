using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RR.AdminService;
using RR.Dto;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
  
    public class SponsorManagementController : Controller
     {
          #region Construtor

          private readonly IHostingEnvironment _hostingEnvironment;
          private readonly ISponsorService _sponsorService;

          public SponsorManagementController(IHostingEnvironment hostingEnvironment, ISponsorService sponsorService)
          {
               _hostingEnvironment = hostingEnvironment;
               _sponsorService = sponsorService;
          }

          #endregion

          /// <summary>
          /// Sponsor Index View
          /// </summary>
          /// <returns></returns>
          [Route("partners")]
          public IActionResult Index()
          {
               return View();
          }

          [HttpPost]
          public async Task<JsonResult> GetAllSponsors()
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

               var sponsors = await _sponsorService.GetAllSponsors(skip / pageSize, pageSize, column, search, sort);

               var data = sponsors.Item1;
               int recordsTotal = sponsors.Item2;

               return Json(new
               {
                    draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal,
                    data
               });
          }

          /// <summary>
          /// Edit Sponsor Detail if sponsorId is not null
          /// </summary>
          /// <param name="sponsorId">Sponsor Id</param>
          /// <returns></returns>
          [HttpGet]
          [Route("partner/addedit/{sponsorId}")]
          public async Task<IActionResult> CreateEditSponsor(int sponsorId = 0)
          {
               try
               {
                    if (sponsorId > 0)
                    {
                         return View(await _sponsorService.GetSponsorById(sponsorId));
                    }
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
               return View(new SponsorDto() { });
          }

          /// <summary>
          /// Add Or Update Sponsor Detail
          /// </summary>
          /// <param name="sponsorDto">An SponsorDto</param>
          /// <returns></returns>
          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<ActionResult> CreateSponsor(SponsorDto sponsorDto)
          {
               try
               {
                    var imageName = string.Empty;
                    if (sponsorDto.Image != null && sponsorDto.Image.Length != 0)
                    {
                         var stream = sponsorDto.Image.OpenReadStream();
                         imageName = Guid.NewGuid() + System.IO.Path.GetExtension(sponsorDto.Image.FileName);
                         string basePath = _hostingEnvironment.ContentRootPath + Path.Combine("/wwwroot/assets/SponsorLogo");
                         string path = System.IO.Path.Combine(basePath, imageName);
                         if (!Directory.Exists(basePath))
                         {
                              Directory.CreateDirectory(basePath);
                         }

                         using (var fileStream = System.IO.File.Create(path))
                         {
                              await stream.CopyToAsync(fileStream);
                         }
                         sponsorDto.SponsorLogo = imageName;
                    }

                    //if (sponsorDto.Id == 0)
                    //{
                    //     sponsorDto.CreatedBy = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    //}
                    //else
                    //{
                    //     sponsorDto.UpdatedBy = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    //}
                    await _sponsorService.AddUpdateSponsorDetail(sponsorDto, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                    return RedirectToAction("Index");
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          }

          /// <summary>
          /// Delete Sponsor
          /// </summary>
          /// <param name="sponsorId">Sponsor Id</param>
          /// <returns></returns>
          [HttpDelete]
          public async Task<JsonResult> DeleteSponsor(int sponsorId)
          {
               try
               {

                    var sponsorDetail = await _sponsorService.GetSponsorById(sponsorId);
                    if (!string.IsNullOrEmpty(sponsorDetail.SponsorLogo))
                    {
                         var filePath = _hostingEnvironment.WebRootPath + Path.Combine($"/assets/SponsorLogo/{sponsorDetail.SponsorLogo}");
                         if (System.IO.File.Exists(filePath))
                         {
                              System.IO.File.Delete(filePath);
                         }
                    }
                    await _sponsorService.DeleteSponsor(sponsorId);
                    return Json("Deleted");
               }
               catch (Exception ex)
               {
                    return Json(ex.Message);
               }
          }
     }
}