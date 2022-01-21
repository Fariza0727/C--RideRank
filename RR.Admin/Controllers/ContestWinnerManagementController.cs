using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RR.AdminService;
using RR.Dto;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]    
    public class ContestWinnerManagementController : Controller
    {
        #region Constructor

        private readonly IContestService _contestService;
        private readonly IAwardTypeService _awardTypeService;
        private readonly IAwardService _awardService;
        private readonly IEventService _eventService;
        private readonly IContestWinnerService _contestWinnerService;

        public ContestWinnerManagementController(IContestService contestService,
            IAwardTypeService awardTypeService,
            IAwardService awardService,
            IEventService eventService,
            IContestWinnerService contestWinnerService)
        {
            _contestService = contestService;
            _awardTypeService = awardTypeService;
            _awardService = awardService;
            _eventService = eventService;
            _contestWinnerService = contestWinnerService;
        }

        #endregion

        [Route("ContestWinners/{id}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> Index(long id)
        {
            ContestAwardDto model = new ContestAwardDto();
            model.ContestId = id;
            var contest = await _contestService.GetContest(id);
            model.WinnerCount = contest != null ? contest.Winners : 0;
            model.Winners = await _contestWinnerService.GetContestWinner(id);
            foreach (var item in model.Winners)
            {
                if (item.Merchandise > 0)
                    item.MerchandiseList.Where(x => x.Value == item.Merchandise.ToString()).FirstOrDefault().Selected = true;
                if (item.OtherReward > 0)
                    item.OtherRewardList.Where(x => x.Value == item.OtherReward.ToString()).FirstOrDefault().Selected = true;
                item.MerchandiseList.Insert(0, new SelectListItem() { Text = "Select Merchandise", Value = "0" });
                item.OtherRewardList.Insert(0, new SelectListItem() { Text = "Select Other Rewards", Value = "0" });
            }


            var MarchendiseList = await _awardService.GetMerchandiseAward();
            model.MerchandiseList = MarchendiseList.Select(x => new SelectListItem()
            {
                Text = x.Message,
                Value = x.Id.ToString()
            }).ToList();
            model.MerchandiseList.Insert(0, new SelectListItem() { Text = "Select Merchandise", Value = "0" });

            var otherList = await _awardService.GetOtherAward();
            model.OtherRewardList = otherList.Select(x => new SelectListItem()
            {
                Text = x.Message,
                Value = x.Id.ToString()
            }).ToList();
            model.OtherRewardList.Insert(0, new SelectListItem() { Text = "Select Other Rewards", Value = "0" });

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> AddEditWinner(ContestAwardDto model)
        {
            await _contestWinnerService.AddEditWinners(model, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Content("success");
        }

        [Route("DeleteContestWinner/{id}")]
        public async Task<IActionResult> DeleteContestWinner(long id)
        {
            await _contestWinnerService.DeleteContestWinner(id);
            return Content("success");
        }

        [HttpPost]
        public async Task<IActionResult> AddEditWinnerMTT(ContestAwardDto model)
        {
            await _contestWinnerService.AddEditWinnerMTT(model, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Content("success");
        }

        [HttpPost]
        public async Task<JsonResult> Getawards(long id)
        {
            var datas = await _awardService.GetAwards(id);
            return Json(datas);

        }
    }
}