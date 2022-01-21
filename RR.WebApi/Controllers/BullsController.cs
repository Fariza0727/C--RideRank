using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Service;
using System;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class BullsController : Controller
     {
          #region Constructor

          private readonly IBullService _bullService;

          public BullsController(IBullService bullService)
          {
               _bullService = bullService;
          }

          #endregion
          [Route("getbulls/{userid}")]
          [HttpGet]
          public async Task<OkObjectResult> GetAllBulls(string userid)
          {
               var bulls = await _bullService.GetCompleteBulls(userid);

               if (bulls != null)
               {
                    return Ok(new ApiResponse
                    {
                         Data = bulls,
                         Success = true,
                         Message = ""
                    });
               }
               return Ok(new ApiResponse
               {
                    Message = "No Record Found!!",
                    Success = false
               });
          }


    }
}