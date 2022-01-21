using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Service;
using System;
using System.Threading.Tasks;

namespace RR.WebApi.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     public class PrivateContestController : Controller
     {
          #region ctr

          private readonly IPrivateContestService _privateContestService;
          private readonly IContestWinnerService _contestWinnerService;

          public PrivateContestController(IPrivateContestService privateContestService,
                  IContestWinnerService contestWinnerService)
          {
               _privateContestService = privateContestService;
               _contestWinnerService = contestWinnerService;
          }

          #endregion

          /// <summary>
          /// Add Private Contest
          /// </summary>
          /// <param name="privateContestDto">Private ContestDto</param>
          /// <param name="userId">User Id</param>
          /// <returns></returns>
          [HttpPost]
          [Route("addprivatecontest")]
          public async Task<OkObjectResult> AddPrivateContest([FromForm] PrivateContestLiteDto privateContestLiteDto)
          {
               try
               {
                    var privateContestDto = new PrivateContestDto
                    {
                         ContestCategoryId = privateContestLiteDto.ContestCategoryId,
                         CreatedBy = privateContestLiteDto.UserId,
                         UpdatedBy = privateContestLiteDto.UserId,
                         UserId = privateContestLiteDto.UserId,
                         EventId = privateContestLiteDto.EventId,
                         JoiningFee = privateContestLiteDto.JoiningFee,
                         WinnerTitle = privateContestLiteDto.WinnerTitle,
                         Members = privateContestLiteDto.Members
                    };


                    var privateContest = await _privateContestService.AddEditContest(privateContestDto);

                    ContestAwardDto model = new ContestAwardDto
                    {
                         ContestId = privateContest.Id,
                         CreatedBy = privateContest.UserId,
                         RankFrom = privateContestLiteDto.RankFrom,
                         RankTo = privateContestLiteDto.RankTo,
                         Value = privateContestLiteDto.Value
                    };

                    await _contestWinnerService.AddEditWinners(model);
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Message = string.Format("Keep This Generated Code For Inviting Friends {0}", privateContest.UniqueCode),
                         Success = true
                    });
               }
               catch (Exception ex)
               {
                    return Ok(new ApiResponse
                    {
                         IpAddress = Helpers.GetIpAddress(),
                         Message = ex.Message,
                         Success = false
                    });
               }
          }
     }
}