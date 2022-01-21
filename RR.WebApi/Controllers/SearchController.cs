using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Service;
using System;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class SearchController : Controller
     {
          #region Constructor

          private readonly ISearchResultService _searchResult;

          public SearchController(ISearchResultService searchResult)
          {
               _searchResult = searchResult;
          }

          #endregion

          /// <summary>
          /// Search Index
          /// </summary>
          /// <param name="keyword">The searching Keyword</param>
          /// <param name="page">Page Number</param>
          /// <returns>Results Of All Related Keyword Search on Search View</returns>
          [HttpPost]
          [Route("search-riders")]
          public async Task<OkObjectResult> SearchRiders([FromForm] string keyword, [FromForm] string userId)
          {
               try
               {
                    var data = await _searchResult.GetRiders(keyword, userId);
                    data.Keyword = keyword;
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Success = true,
                         Data = data.RidersList,
                         Message = ""
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Success = false,
                         Message = ex.Message
                    });
               }

          }

          /// <summary>
          /// Search Index
          /// </summary>
          /// <param name="keyword">The searching Keyword</param>
          /// <param name="page">Page Number</param>
          /// <returns>Results Of All Related Keyword Search on Search View</returns>
          [HttpPost]
          [Route("search-bulls")]
          public async Task<OkObjectResult> SearchBulls([FromForm] string keyword, [FromForm] string userId)
          {
               try
               {
                    var data = await _searchResult.GetBulls(keyword, userId);
                    data.Keyword = keyword;
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Success = true,
                         Data = data.BullsList,
                         Message = ""
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Success = false,
                         Message = ex.Message
                    });
               }

          }

     }
}