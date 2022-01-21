using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RR.Core;
using RR.Dto;
using RR.Service;
using RR.Service.Email;
using RR.Service.User;
using RR.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
    public class ShopifyWebhooksController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserService _userService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ICountryService _countryService;
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;
        private readonly IHostingEnvironment _env;
        private readonly IEmailSender _emailSender;

        public ShopifyWebhooksController(
            UserManager<IdentityUser> userManager,
            IUserService userService, SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ICountryService countryService,
            IStateService stateService,
            ICityService cityService,
            IHostingEnvironment env

            )
        {
            _userManager = userManager;
            _userService = userService;
            _signInManager = signInManager;
            _countryService = countryService;
            _stateService = stateService;
            _cityService = cityService;
            _env = env;
            _emailSender = emailSender;
        }


        [HttpPost]
        [Route("customercreate")]
        public async Task<IActionResult> CustomerCreate()
        {
            var statusObj = new Json_ShopifyObject();
            var customer_ = new SCustomer();

            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var data_ = await reader.ReadToEndAsync();
                    customer_ = JsonConvert.DeserializeObject<SCustomer>(data_);
                    statusObj.Customerid = customer_.Id.ToString();
                    statusObj.Progress.Add("Deserialize Success");
                }

                await CreateCustomer(customer_, statusObj);
                statusObj.Progress.Add("Successfully Created");
            }
            catch (Exception ex)
            {
                statusObj.Error = string.Concat($"Error: ShopifyUser({customer_.Id}) {ex.GetActualError()}");
            }

            statusObj.ShopifyUserStatus(_env);
            return Ok();
        }

        [HttpPost]
        [Route("customerupdate")]
        public async Task<IActionResult> CustomerUpdate()
        {
            var statusObj = new Json_ShopifyObject();
            var customer_ = new SCustomer();
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var data_ = await reader.ReadToEndAsync();
                    customer_ = JsonConvert.DeserializeObject<SCustomer>(data_);
                    statusObj.Customerid = customer_.Id.ToString();
                    statusObj.Progress.Add("Deserialize Success");
                }

                await UpdateCustomer(customer_, statusObj);
                statusObj.Progress.Add("Successfully Updated");
            }
            catch (Exception ex)
            {
                statusObj.Error = string.Concat($"Error: ShopifyUser({customer_.Id}) {ex.GetActualError()}");
            }

            statusObj.ShopifyUserStatus(_env);
            return Ok();
        }

        [HttpPost]
        [Route("customerdelete")]
        public async Task<IActionResult> CustomerDelete()
        {
            var statusObj = new Json_ShopifyObject();
            var customer_ = new SCustomer();
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    customer_ = JsonConvert.DeserializeObject<SCustomer>(await reader.ReadToEndAsync());
                    statusObj.Customerid = customer_.Id.ToString();
                    statusObj.Progress.Add("Deserialize Success");
                }

                if (customer_?.Id > 0)
                {
                    await _userService.DeleteCustomerUserDetail(customer_.Id);
                    statusObj.Progress.Add("Successfully Deleted");
                }
            }
            catch (Exception ex)
            {
                statusObj.Error = string.Concat($"Error: ShopifyUser({customer_.Id}) {ex.GetActualError()}");
            }

            statusObj.ShopifyUserStatus(_env);
            return Ok();
        }


        private async Task CreateCustomer(SCustomer customer_, Json_ShopifyObject shopifyObject)
        {
            if (customer_?.Id > 0)
            {

                var isExist = await _userManager.FindByEmailAsync(customer_.Email);
                if (isExist != null)
                {
                    await UpdateCustomer(customer_, shopifyObject);
                }
                else
                {

                    string password_ = ExtensionsHelper.GeneratePassword(8, 4);
                    var user = new IdentityUser {
                        UserName = customer_.Email,
                        Email = customer_.Email,
                        EmailConfirmed = true,
                        PhoneNumber = customer_.Phone ?? customer_.DefaultAddress?.Phone
                     };
                    var result = await _userManager.CreateAsync(user, password_);
                    if (result.Succeeded)
                    {
                        shopifyObject.Progress.Add($"User:({user.Id}) Added Successfully");
                        UserDetailDto userDetail = new UserDetailDto();
                        userDetail.CreatedDate = DateTime.Now;
                        userDetail.FirstName = customer_.FirstName;
                        userDetail.LastName = customer_.LastName;
                        userDetail.Email = customer_.Email;
                        userDetail.UserId = user.Id;
                        userDetail.IsActive = true;
                        userDetail.UserName = await IsUsernameExist(customer_.Email?.Split("@")[0]);
                        userDetail.ShopifyCustomerId = customer_.Id;
                        userDetail.ShopifyMembership = customer_.Tags;
                        userDetail.IsPaidMember = true;
                        userDetail.PhoneNumber = customer_.Phone ?? customer_.DefaultAddress?.Phone;
                        // Both fieds removed from apis request
                        // we removed requried attr and replaced the null value with static/default
                        userDetail.PlayerType = "PLAYFORFREE";
                        await _userService.AddEditUserDetail(userDetail,true);
                        shopifyObject.Progress.Add($"UserDetail:({user.Id}) Added Successfully");

                        //await _userManager.AddToRoleAsync(user, "User");
                        await _userManager.AddToRoleAsync(user, userDetail.PlayerType);
                        shopifyObject.Progress.Add($"UserRoles:({user.Id}) Added Successfully");

                        // TEMP COMMENT THIS PROCESS AS PER CLIENT REQUIRMENT
                        // AND ADDED EmailConfirmed = true DURING USER CREATE
                        #region
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Action(
                        //   "ConfirmEmail", "Account",
                        //   new { userId = user.Id, code = code },
                        //   protocol: Request.Scheme);

                        //callbackUrl = callbackUrl.Replace("Identity/", "").Replace("localhost:61198", "rankridefantasy.com/"); // 
                        //// Send Mail For Forget Password
                        //MailRequest(userDetail.Email, callbackUrl, "ForgetPassword", userDetail.UserName, password_);
                        #endregion

                        shopifyObject.Progress.Add($"User:({user.Id}) Sent TempPass Successfully");
                    }

                }


            }
        }
        private async Task UpdateCustomer(SCustomer customer_, Json_ShopifyObject shopifyObject)
        {
           if (customer_?.Id > 0)
            {
                var aspUser = await _userManager.FindByEmailAsync(customer_.Email);
                if (aspUser != null)
                {
                    aspUser.Email = customer_.Email;
                    aspUser.PhoneNumber = customer_.Phone ?? customer_.DefaultAddress?.Phone;
                    aspUser.UserName = customer_.Email;
                    var result = await _userManager.UpdateAsync(aspUser);
                    UserDetailDto userDetail = new UserDetailDto();
                    if (result.Succeeded)
                    {
                        shopifyObject.Progress.Add($"User:({aspUser.Id}) Updated Successfully");
                        userDetail = await _userService.GetUserDetail(aspUser.Id);
                        if (userDetail != null)
                        {
                            var country = await _countryService.GetCountryByName(customer_.DefaultAddress?.Country);
                            var state = await _stateService.GetStateByName(customer_.DefaultAddress?.Province);

                            userDetail.FirstName = customer_.FirstName;
                            userDetail.LastName = customer_.LastName;
                            userDetail.Email = customer_.Email;
                            userDetail.UserName = await IsUsernameExist(customer_.Email?.Split("@")[0], userDetail.UserId);
                            userDetail.ShopifyCustomerId = customer_.Id;
                            userDetail.Address1 = customer_.DefaultAddress?.Address1;
                            userDetail.ZipCode = customer_.DefaultAddress?.Zip;
                            userDetail.PhoneNumber = customer_.Phone ?? customer_.DefaultAddress?.Phone;
                            userDetail.City = customer_.DefaultAddress?.City;
                            userDetail.ShopifyMembership = customer_.Tags;
                            userDetail.IsPaidMember = true;
                            userDetail.ShopifyCustomerId = customer_.Id;

                            if (country != null && country.Id != null)
                                userDetail.Country = Convert.ToInt32(country.Id);

                            if (state != null)
                                userDetail.State = state.StateId;

                            await _userService.AddEditUserDetail(userDetail,true);
                            shopifyObject.Progress.Add($"UserDetail:({userDetail.UserId}) Updated Successfully");
                        }
                        else
                        {
                            userDetail = new UserDetailDto();
                            userDetail.CreatedDate = DateTime.Now;
                            userDetail.FirstName = customer_.FirstName;
                            userDetail.LastName = customer_.LastName;
                            userDetail.Email = customer_.Email;
                            userDetail.UserId = aspUser.Id;
                            userDetail.IsActive = true;
                            userDetail.UserName = await IsUsernameExist(customer_.Email?.Split("@")[0]);
                            userDetail.ShopifyCustomerId = customer_.Id;
                            userDetail.ShopifyMembership = customer_.Tags;
                            userDetail.IsPaidMember = true;
                            userDetail.PhoneNumber = customer_.Phone ?? customer_.DefaultAddress?.Phone;
                            // Both fieds removed from apis request
                            // we removed requried attr and replaced the null value with static/default
                            userDetail.PlayerType = "PLAYFORFREE";
                            await _userService.AddEditUserDetail(userDetail,true);
                            shopifyObject.Progress.Add($"UserDetail:({aspUser.Id}) Added Successfully");

                            //await _userManager.AddToRoleAsync(user, "User");
                            await _userManager.AddToRoleAsync(aspUser, userDetail.PlayerType);
                            shopifyObject.Progress.Add($"UserRoles:({aspUser.Id}) Added Successfully");
                        }

                    }
                }
                else
                {
                    await CreateCustomer(customer_, shopifyObject);
                }
            }
        }

        private async Task<string> IsUsernameExist(string username, string userid = "")
        {
            string userName = "";

            if (!string.IsNullOrEmpty(username))
            {
                int rechecCount = 1;
                userName = username;
                recheck:
                var isExist = await _userService.IsUserExist(username, userid);
                if (isExist)
                {
                    long ticks = DateTime.Now.Ticks;
                    string uniq = ticks.ToString().Substring(10, 5);

                    userName = string.Concat(username, "_", uniq);
                    if(rechecCount <= 3)
                    {
                        rechecCount++;
                        goto recheck;
                    }
                }
            }
            return await Task.FromResult(userName);
        }

        public async void MailRequest(string toEmail, string callBackUrl, string title, string userName, string password)
        {
            try
            {
                string emailBody = Utilities.GetEmailTemplateValue("ShopifyAccountActivation/Body");
                string emailSubject = Utilities.GetEmailTemplateValue("ShopifyAccountActivation/Subject");
                emailBody = emailBody.Replace("@@@Password", password);
                emailBody = emailBody.Replace("@@@title", title);
                emailBody = emailBody.Replace("@@@UserEmail", userName);
                emailBody = emailBody.Replace("@@@Url", HtmlEncoder.Default.Encode(callBackUrl));
                await _emailSender.SendEmailAsync(toEmail, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
            }
        }
    }
}