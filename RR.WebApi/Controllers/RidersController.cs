using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Service;
using System;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class RidersController : Controller
     {
          #region Constructor

          private readonly IRiderService _riderService;

          public RidersController(IRiderService riderService)
          {
               _riderService = riderService;
          }

          #endregion

          [Route("getriders/{userId}")]
          [HttpGet]
          public async Task<OkObjectResult> GetAllRiders(string userId)
          {
               var riders = await _riderService.GetCompleteRiders(userId);

               if (riders != null)
               {
                    return Ok(new ApiResponse
                    {
                         Data = riders,
                         Success = true,
                         Message = ""
                    });
               }
               return Ok(new ApiResponse
               {
                    Message = "No record found!!",
                    Success = false
               });
          }


    }
}