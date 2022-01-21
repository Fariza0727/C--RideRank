using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RR.Dto;
using RR.Service;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    [Authorize]
    public class PrivateContestController : Controller
    {
        #region ctr

        private readonly IPrivateContestService _privateContestService;
        private readonly IEventService _eventService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IContestWinnerService _contestWinnerService;
        private readonly IContestService _contestService;

        public PrivateContestController(IPrivateContestService privateContestService,
               UserManager<IdentityUser> userManager,
                IEventService eventService,
                IContestWinnerService contestWinnerService,
                IContestService contestService)
        {
            _privateContestService = privateContestService;
            _userManager = userManager;
            _eventService = eventService;
            _contestWinnerService = contestWinnerService;
            _contestService = contestService;
        }

        #endregion

        [Route("private-contest/{EventId}")]
        public async Task<IActionResult> Index(int EventId = 0)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            PrivateContestDto privateContestDto = new PrivateContestDto
            {
                EventId = EventId,
                CreatedBy = user.Id
            };
            return View(privateContestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PrivateContestDto privateContestDto)
        {
            var privateContest = await _privateContestService.AddEditContest(privateContestDto);

            //var contestDetail = await _contestService.GetContestByUniqueCode(privateContestCode);
            var userId = _userManager.GetUserId(HttpContext.User);

            ContestAwardDto model = new ContestAwardDto
            {
                ContestId = privateContest.Id,
                CreatedBy = userId,
                RankFrom = privateContestDto.RankFrom,
                RankTo = privateContestDto.RankTo,
                Value = privateContestDto.Value
            };

            await _contestWinnerService.AddEditWinners(model);
            TempData["message"] = string.Format("Keep This Generated Code For Inviting Friends {0}", privateContest.UniqueCode);
            TempData["code"] = privateContest.UniqueCode;
            return RedirectToAction("Thankyou", "Home");
        }
    }
}