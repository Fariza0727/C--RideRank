
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RR.Admin.Models;
using RR.AdminService;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Dto.Player;
using RR.Repo;
using RR.Service.Email;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{

    [Authorize(Roles = "Admin,Subadmin")]
    public class UserManagementController : Controller
    {
        #region Constructor
        private readonly IUserService _userservice;
        private readonly ICommonService _commonService;
        private readonly IRepository<AspNetRoles, RankRideContext> _repoRoles;
        private readonly IEmailSender _emailSender;

        public UserManagementController(IUserService userService, ICommonService commonService,
            IRepository<AspNetRoles, RankRideContext> repoRoles, IEmailSender emailSender)
        {
            _userservice = userService;
            _commonService = commonService;
            _repoRoles = repoRoles;
            _emailSender = emailSender;
        }

        #endregion

        /// <summary>
        /// User management List View
        /// </summary>
        /// <returns>User Management View</returns>
        [Route("players")]
        [Authorize(Policy = "PagePermission")]
        public IActionResult Index()
        {
            if (TempData["IsSuccess"] != null)
            {
                ViewBag.IsSuccess = "true";
            }
            return View();
        }

        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="id">A User Id</param>
        /// <returns>user status detail</returns>
        [HttpPost]
        [Route("upadatestatus")]
        public async Task<JsonResult> UpdateStatus(int id)
        {
            try
            {
                await _userservice.UpdateStatus(id);
                var userDetail = await _userservice.GetUserDetail(id);
                return Json(userDetail);
            }
            catch (System.Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Get User Detail
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User Detail</returns>
        [HttpPost]
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> GetUserDetail(int id)
        {
            try
            {
                var userDetail = await _userservice.GetUserDetail(id);
                userDetail.CountryName = await _commonService.GetCountryById(userDetail.Country);
                userDetail.StateName = await _commonService.GetStateById(userDetail.State);
                userDetail.CityName = userDetail.City;

                return Json(userDetail);
            }
            catch (System.Exception ex)
            {
                return Json(ex.Message);
            }
        }

        /// <summary>
        /// Update User Detail
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User Detail</returns>
        [Route("player/detail/{id}")]
        [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> EditUserDetail(int id)
        {
            try
            {
                var userDetail = await _userservice.GetUserDetail(Convert.ToInt32(id));

                var countryList = await _commonService.GetCountries();
                var cityList = await _commonService.GetCitiesByCountryId(Convert.ToInt32(userDetail.Country));
                var stateList = await _commonService.GetStates(Convert.ToInt32(userDetail.Country));

                userDetail.CountryList = countryList;
                userDetail.CityList = cityList;
                userDetail.StateList = stateList;
                userDetail.UserRoleList = GetUserRoleList();
                return View(userDetail);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserDetail(UserDetailDto userDto)
        {
            try
            {
                await _userservice.AddEditUserDetail(userDto);
                await _userservice.UpdateUserRole(userDto.UserId, userDto.PlayerType);
                TempData["IsSuccess"] = "true";
                return RedirectToAction("Index");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get States
        /// </summary>
        /// <param name="countryId">Country Id</param>
        /// <returns>List Of States Of Given Country Id</returns>

        [Route("states")]
        public async Task<JsonResult> GetStates(int countryId)
        {
            try
            {
                return Json(await _commonService.GetStates(countryId));
            }
            catch (System.Exception ex)
            {
                return Json(ex.Message);
            }

        }

        /// <summary>
        /// Get Cities
        /// </summary>
        /// <param name="stateId">The StateId</param>
        /// <param name="countryId">The CountryId</param>
        /// <returns>List Of All Cities Of Given StateId or CountryId</returns>

        [Route("cities")]
        public async Task<JsonResult> GetCities(int stateId, int countryId)
        {
            try
            {
                var cities = await _commonService.GetCitiesByStateId(stateId);
                if (stateId == 0)
                {
                    cities = await _commonService.GetCitiesByCountryId(countryId);
                }
                return Json(cities);
            }
            catch (System.Exception ex)
            {
                return Json(ex.Message);
            }

        }

        /// <summary>
        /// Unlock User
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("updateuserlockstatus/")]
        [Authorize(Policy = "PagePermission")]
        public async Task<JsonResult> Updateuserlockstatus(string userName)
        {

            var data = await _userservice.Updateuserlockstatus(userName);

            return Json(new
            {
                status = data
            });
        }


        [HttpPost]
        public async Task<JsonResult> GetCurrentPageUsers()
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

            var users = await _userservice.GetAllExistUsers(skip / pageSize, pageSize, column, search, sort);

            var data = users.Item1;
            int recordsTotal = users.Item2;

            return Json(new
            {
                draw,
                recordsFiltered = recordsTotal,
                recordsTotal,
                data
            });
        }
        /// <summary>
        /// Delete Cms
        /// </summary>
        /// <param name="id">user Id</param>
        /// <returns></returns>
        [Authorize(Policy = "PagePermission")]
        [Route("deluser/{id}")]
        public async Task<string> DeleteUser(long id)
        {

            bool deleted = await _userservice.DeleteUserByID(id);

            if (deleted)
            {
                return "success";
            }
            else
            {
                return "failed";
            }



        }

        public List<SelectListItem> GetUserRoleList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Insert(0, new SelectListItem() { Text = "Play For FREE", Value = "NOVICE PLAYER" });
            list.Insert(1, new SelectListItem() { Text = "Top Gun Tour", Value = "INTERMEDIATE PLAYER" });
            list.Insert(2, new SelectListItem() { Text = "Mega Buck Thunder", Value = "PRO PLAYER" });

            return list; /*_repoRoles.Query().Get().ToList().Select(x=>new SelectListItem() { Value=x.NormalizedName, Text= })*/
        }

        [Route("BulkNotifications")]
        public async Task<IActionResult> Notifications()
        {
            return View();
        }

        [HttpPost]
        [Route("BulkNotifications")]
        public async Task<JsonResult> Notifications(IFormFile Excel, string Notify)
        {
            if (Excel.Length > 0 && !string.IsNullOrEmpty(Notify))
            {
                try
                {
                    using (var stream = Excel.OpenReadStream())
                    {
                        // Auto-detect format, supports:
                        //  - Binary Excel files (2.0-2003 format; *.xls)
                        //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                        using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                        {

                            //// 1. Use the reader methods
                            //do
                            //{
                            //    while (reader.Read())
                            //    {
                            //        //We don't need an empty object
                            //        //var firstName = reader.GetString(0);

                            //    }
                            //} while (reader.NextResult());

                            // 2. Use the AsDataSet extension method
                            var result = reader.AsDataSet();
                            if (result.Tables.Count > 0)
                            {
                                var table = result.Tables[0];
                                List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                                Dictionary<string, object> childRow;
                                Dictionary<string, string> columns = new Dictionary<string, string>();

                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    if (i == 0)
                                        continue;

                                    if (i == 1)
                                    {
                                        foreach (DataColumn col in table.Columns)
                                        {
                                            columns.Add(col.ColumnName, table.Rows[i][col].ToString());
                                        }
                                    }
                                    else
                                    {
                                        childRow = new Dictionary<string, object>();
                                        foreach (DataColumn col in table.Columns)
                                        {
                                            childRow.Add(columns[col.ColumnName].ToString(), table.Rows[i][col].ToString());
                                        }
                                        parentRow.Add(childRow);

                                    }
                                }



                                string json = JsonConvert.SerializeObject(parentRow, Formatting.Indented);
                                var players = JsonConvert.DeserializeObject<List<PlayerLiteDto>>(json);

                                string emailBody = Utilities.GetEmailTemplateValue("PlayerNotification/Body");
                                string emailSubject = Utilities.GetEmailTemplateValue("PlayerNotification/Subject");
                                emailBody = emailBody.Replace("@@@title", "Notification");
                                emailBody = emailBody.Replace("@@@Notification", Notify);

                                #region Notifiy to Mail
                                players.Where(d => d.IsNotifyEmail).ToList().ForEach(async m =>
                                {
                                    emailBody = emailBody.Replace("@@@UserEmail", m.Email);
                                    await _emailSender.SendEmailAsync(m.Email, emailSubject, emailBody
                                  );
                                });
                                #endregion

                                #region Notifiy to SMS
                                //Need SMS API
                                #endregion

                            }

                        }
                    }
                    return Json(new { status = true, message = "Success" });
                }
                catch (Exception ex)
                {

                    return Json(new { status = false, message = ex.GetActualError() });
                }
            }

            return Json(new { status = false, message = "Please fill required fields" });
        }
    }
}


