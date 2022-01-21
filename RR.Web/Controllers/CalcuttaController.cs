
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RR.Dto;
using RR.Dto.Calcutta;
using RR.Dto.Contest;
using RR.Dto.Team;
using RR.Service;
using RR.Service.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    public class CalcuttaController : Controller
    {
        #region Constructor     
        private readonly ICalcuttaService _calcuttaService;
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        
        private readonly UserManager<IdentityUser> _userManager;

        public CalcuttaController(ICalcuttaService calcuttaService,
             IEventService eventService,
             IUserService userService,
             ITeamService teamService,
              UserManager<IdentityUser> userManager)
        {
            _calcuttaService = calcuttaService;
            _eventService = eventService;
            _userManager = userManager;
            _userService = userService;
            _teamService = teamService;
        }

        #endregion

        [Route("event/bullcomp/{eventId}")]
        public async Task<IActionResult> GetEntryOfCalcutta(int eventId, bool status=false, decimal amount=0)
        {
            var user = _userManager.GetUserId(HttpContext.User);
            var dto = await _calcuttaService.GetEventDetail(eventId, user);
            ViewBag.Status = status;
            ViewBag.Amount = amount;

            return View(dto);
        }

        [Route("event/ridercomp/{eventId}")]
        public async Task<IActionResult> GetEntryOfRCCalcutta(int eventId, bool status = false, decimal amount = 0)
        {
            var user = _userManager.GetUserId(HttpContext.User);
            var dto = await _calcuttaService.GetRCEventDetail(eventId, user);
            ViewBag.Status = status;
            ViewBag.Amount = amount;

            return View(dto);
        }

        [Route("event/bullcomp/checkout/{eventId}")]
        public async Task<IActionResult> CheckoutEntryOfCalcutta(int eventId)
        {
            var user = _userManager.GetUserId(HttpContext.User);
            var dto = await _calcuttaService.GetCheckoutDetail(eventId, user);
            return View(dto);
        }

        [Route("event/ridercomp/checkout/{eventId}")]
        public async Task<IActionResult> CheckoutEntryOfRCCalcutta(int eventId)
        {
            var user = _userManager.GetUserId(HttpContext.User);
            var dto = await _calcuttaService.GetRCCheckoutDetail(eventId, user);
            return View(dto);
        }

        public async Task<ActionResult> CreateOrder(int eventId, bool isRiderComp = false)
        {
            try
            {
                var user = _userManager.GetUserId(HttpContext.User);
                var approvedUrl = await _calcuttaService.CreatePayment(eventId, user, isRiderComp);

                if (string.IsNullOrEmpty(approvedUrl))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Something error"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        approvedUrl = approvedUrl
                    });
                }
                
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            
        }
        public async Task<ActionResult> AddCart(string cartData)
        {
            try
            {
                var user = _userManager.GetUserId(HttpContext.User);
                if (user != null)
                {
                    var cart = JsonConvert.DeserializeObject<AddCartDto>(cartData);
                    var bSuccess = await _calcuttaService.AddCart(cart, user);
                    
                    return Json(new
                    {
                        success = bSuccess
                    });
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public async Task<ActionResult> RemoveCart(int cartId)
        {
            try
            {
                var user = _userManager.GetUserId(HttpContext.User);
                if (user != null)
                {
                    var bSuccess = await _calcuttaService.RemoveCart(cartId, user);

                    return Json(new
                    {
                        success = bSuccess
                    });
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [Route("event/bullcomp/success/{eventId}")]
        public async Task<IActionResult> SuccessOrder(int eventId, string token, string PayerID)
        {
            var result = await _calcuttaService.CapturePayment(eventId, token, PayerID, false);
            if (result > 0)
            {
                return RedirectToAction("GetEntryOfCalcutta", "Calcutta", new { eventId = eventId, status = true, amount = result });
            }
            else
            {
                return RedirectToAction("CheckoutEntryOfCalcutta", "Calcutta", new { eventId = eventId });
            }
            
        }
        [Route("event/bullcomp/cancel/{eventId}")]
        public async Task<IActionResult> CancelOrder(int eventId)
        {
            return RedirectToAction("CheckoutEntryOfCalcutta", "Calcutta", new { eventId = eventId });

        }

        [Route("event/ridercomp/success/{eventId}")]
        public async Task<IActionResult> SuccesRCsOrder(int eventId, string token, string PayerID)
        {
            var result = await _calcuttaService.CapturePayment(eventId, token, PayerID, true);
            if (result > 0)
            {
                return RedirectToAction("GetEntryOfRCCalcutta", "Calcutta", new { eventId = eventId, status = true, amount = result });
            }
            else
            {
                return RedirectToAction("CheckoutEntryOfRCCalcutta", "Calcutta", new { eventId = eventId });
            }

        }
        [Route("event/ridercomp/cancel/{eventId}")]
        public async Task<IActionResult> CancelRCOrder(int eventId)
        {
            return RedirectToAction("CheckoutEntryOfRCCalcutta", "Calcutta", new { eventId = eventId });

        }

        [HttpPost]
        [Route("getcalcuttauserhistoryajax")]
        public async Task<JsonResult> GetCurrentUserCalcuttaAjax()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];
            
            var histDetail = await _calcuttaService.GetHistoryOfCurrentUser(_userManager.GetUserId(HttpContext.User), Convert.ToInt32(start), Convert.ToInt32(length));

            var data = histDetail.Item1;
            int recordsTotal = histDetail.Item2;
            var temp = new Tuple<CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto>[3];
            temp[0] = new Tuple<CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp,
            });
        }
        [HttpPost]
        [Route("getcalcuttauserawardhistoryajax")]
        public async Task<JsonResult> GetCurrentUserCalcuttaAwardAjax()
        {

            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            var histDetail = await _calcuttaService.GetAwardHistoryOfCurrentUser(_userManager.GetUserId(HttpContext.User), Convert.ToInt32(start), Convert.ToInt32(length));

            var data = histDetail.Item1;
            int recordsTotal = histDetail.Item2;
            var temp = new Tuple<CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto>[3];
            temp[0] = new Tuple<CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto, CalcuttaHistoryLiteDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp,
            });
        }

        [Route("event/pick-team/{eventId}")]
        public async Task<IActionResult> SimplePickTeam(int eventId)
        {
            var user = _userManager.GetUserId(HttpContext.User);
            var dto = await _calcuttaService.GetSimplePickTeamDetail(eventId, user);
            
            return View(dto);
        }

        public async Task<ActionResult> CreateTeam(string teamData, int eventId, int teamID)
        {
            try
            {
                //var eventInfo = await _calcuttaService.GetEventById(eventId);

                /*bool CanEditTeam = (DateTime.UtcNow <= eventInfo.PerfTimeUTC);
                if (!CanEditTeam)
                {
                    return Json(new { timeout = true, message = "You can update the team up to 1 hour before the event starts" });
                }*/

                var user = _userManager.GetUserId(HttpContext.User);
                if (user != null)
                {
                    var team = JsonConvert.DeserializeObject<IEnumerable<SimpleTeamDto>>(teamData);

                    if (teamID > 0)
                        await _calcuttaService.DeleteTeam(teamID);
                    var teamId = await _calcuttaService.CreateTeam(team, eventId, user);

                    return Json(new
                    {
                        eventId,
                        teamId
                    });
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        
        
        [HttpGet]
        [Route("joined-user-team/{eventId}")]
        public async Task<IActionResult> JoinedUserTeam(int eventId)
        {
            var eventDetail = await _calcuttaService.GetEventById(eventId);
            return View(eventDetail);
        }


        [HttpPost]
        [Route("joinedteamajax/{eventId}")]
        public async Task<JsonResult> JoinedTeamAjax(int eventId)
        {

            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            var joinedUsers = await _calcuttaService.JoinedUserContestAjax(eventId, Convert.ToInt32(start), Convert.ToInt32(length));
            //var data = joinedUsers.Item1.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            var data = joinedUsers.Item1;
            int recordsTotal = joinedUsers.Item2;
            var temp = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>[3];
            temp[0] = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);



            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp,
            });
        }

        [Route("userteam/{userId}/{eventId}/{teamId}")]
        public async Task<IActionResult> GetTeamOfUser(string userId, int eventId, int teamId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var userInfo = await _userService.GetUserDetail(!string.IsNullOrEmpty(userId) ? userId : user != null ? user.Id : "");

                var event_ = await _calcuttaService.GetEventById(eventId);

                var joinedContestDetailOfUser = await _calcuttaService.GetTeamDetailOfCurrentUser(eventId, teamId);
                ViewBag.EventId = event_.Id;
                ViewBag.EventStatus = "";
                ViewBag.EventName = event_.Title;
                ViewBag.TeamName = String.IsNullOrEmpty(userInfo.TeamName) ? userInfo.UserName : userInfo.TeamName;
                ViewBag.Position = joinedContestDetailOfUser.Item3;
                ViewBag.NumberOfEntries = joinedContestDetailOfUser.Item2;
                ViewBag.TeamPoint = joinedContestDetailOfUser.Item4;
                ViewBag.JoinDate = joinedContestDetailOfUser.Item5;

                return View(joinedContestDetailOfUser.Item1);


            }
            catch (Exception ex)
            {
                return RedirectToAction("index", "home");
            }
        }

        [HttpPost]
        [Route("joined-current-user-teamajax")]
        public async Task<JsonResult> JoinedCurrentUserTeamAjax()
        {

            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            var joinedUsers = await _calcuttaService.JoinedCurrentUserContestAjax(_userManager.GetUserId(HttpContext.User), Convert.ToInt32(start), Convert.ToInt32(length));
            //var data = joinedUsers.Item1.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length));
            var data = joinedUsers.Item1;
            int recordsTotal = joinedUsers.Item2;
            var temp = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>[3];
            temp[0] = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>(data.Count() > 0 ? data.ElementAt(0) : null, data.Count() > 1 ? data.ElementAt(1) : null, data.Count() > 2 ? data.ElementAt(2) : null);
            temp[1] = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>(data.Count() > 3 ? data.ElementAt(3) : null, data.Count() > 4 ? data.ElementAt(4) : null, data.Count() > 5 ? data.ElementAt(5) : null);
            temp[2] = new Tuple<JoinUserContestLiteDto, JoinUserContestLiteDto, JoinUserContestLiteDto>(data.Count() > 6 ? data.ElementAt(6) : null, data.Count() > 7 ? data.ElementAt(7) : null, data.Count() > 8 ? data.ElementAt(8) : null);



            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data = temp,
            });
        }
    }
}