using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RR.Data.Membership.Data;
using RR.Dto;
using RR.Repo;
using RR.Service.Email;
using RR.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RR.DBManager
{
  public  class Program
    {
        private static IServiceProvider _serviceProvider;
        static string RankRide_CString = ConfigurationManager.AppSettings["RankRide_ConnectionString"];
        static string RankRideAdmin_CString = ConfigurationManager.AppSettings["RankRideAdmin_ConnectionString"];
        static string RankRideStatic_CString = ConfigurationManager.AppSettings["RankRideStatic_ConnectionString"];
        public static void Main(string[] args)
        {

            IServiceCollection collection = new ServiceCollection();

            collection.AddDbContext<Data.RankRideContext>(options => options
            .UseSqlServer(RankRide_CString));
            collection.AddDbContext<AdminData.RankRideAdminContext>(options => options
           .UseSqlServer(RankRideAdmin_CString));
            collection.AddDbContext<StaticData.RankRideStaticContext>(options => options
           .UseSqlServer(RankRideStatic_CString));
            collection.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            collection.AddScoped<IEmailSender, EmailSender>();
            //Add services
            _serviceProvider = collection.BuildServiceProvider();

            

            var _adminUser = _serviceProvider.GetService<IRepository<RR.AdminData.AspNetUsers, AdminData.RankRideAdminContext>>();
            var _adminRole = _serviceProvider.GetService<IRepository<RR.AdminData.AspNetRoles, AdminData.RankRideAdminContext>>();
            

            var _webUser = _serviceProvider.GetService<IRepository<RR.Data.AspNetUsers, Data.RankRideContext>>();
            var _webRole = _serviceProvider.GetService<IRepository<RR.Data.AspNetRoles, Data.RankRideContext>>();

            var _adminUserDetail = _serviceProvider.GetService<RR.AdminService.IUserService>();
            var _webUserDetail = _serviceProvider.GetService<RR.Service.User.IUserService>();

            #region
            string emailBody = Utilities.GetEmailTemplateValue("AccountCreated/Body");
            string emailSubject = Utilities.GetEmailTemplateValue("AccountCreated/Subject");
            emailBody = emailBody.Replace("@@@Title", "Reset Password Request");
            emailBody = emailBody.Replace("@@@Email", "govind.saini@arkasoftwares.com");
            emailBody = emailBody.Replace("@@@Password", "asdf");
            Execute("govind.saini@arkasoftwares.com", emailSubject, emailBody).Wait();
            #endregion

            //string hookServer = "https://rankridefantasy.com/";
            //var customer = new List<SCustomer>();
            //var missinguser = new List<string>();

            //foreach (var customer_ in customer)
            //{
            //    var user = await _userManager.FindByEmailAsync(customer_.Email);
            //    var isSuccess = true;
            //    var isMailRequired = false;
            //    string password_ = ExtensionsHelper.GeneratePassword(8, 4);

            //    var email = customer_.Email;
            //    var name = string.Concat(customer_.FirstName, " ", customer_.LastName);
            //    if (string.IsNullOrEmpty(name))
            //        name = email.Split("@")[0];

            //    if (user == null)
            //    {
            //        user = new IdentityUser
            //        {
            //            UserName = customer_.Email,
            //            Email = customer_.Email,
            //            EmailConfirmed = true,
            //            PhoneNumber = customer_.Phone ?? customer_.DefaultAddress?.Phone
            //        };
            //        var result = await _userManager.CreateAsync(user, password_);
            //        isSuccess = result.Succeeded;
            //        isMailRequired = result.Succeeded;
            //    }


            //    if (isSuccess)
            //    {
            //        var userDetail = await _userService.GetUserDetail(user.Id);
            //        if (userDetail == null)
            //        {
            //            userDetail = new UserDetailDto();
            //            userDetail.CreatedDate = DateTime.Now;
            //            userDetail.FirstName = customer_.FirstName;
            //            userDetail.LastName = customer_.LastName;
            //            userDetail.Email = customer_.Email;
            //            userDetail.UserId = user.Id;
            //            userDetail.IsActive = true;
            //            userDetail.UserName = name;
            //            userDetail.ShopifyCustomerId = customer_.Id;
            //            userDetail.ShopifyMembership = customer_.Tags;
            //            userDetail.IsPaidMember = true;
            //            userDetail.PhoneNumber = customer_.Phone ?? customer_.DefaultAddress?.Phone;
            //            // Both fieds removed from apis request
            //            // we removed requried attr and replaced the null value with static/default
            //            userDetail.PlayerType = "PLAYFORFREE";
            //            //await _userService.AddEditUserDetail(userDetail, true);
            //            //await _userManager.AddToRoleAsync(user, userDetail.PlayerType);

            //            if (isMailRequired)
            //            {
            //                #region
            //                //string emailBody = Utilities.GetEmailTemplateValue("AccountCreated/Body");
            //                //string emailSubject = Utilities.GetEmailTemplateValue("AccountCreated/Subject");
            //                //emailBody = emailBody.Replace("@@@Title", "Account Registration");
            //                //emailBody = emailBody.Replace("@@@Email", user.Email);
            //                //emailBody = emailBody.Replace("@@@Password", password_);
            //                //await _emailSender.SendEmailAsync(user.Email, emailSubject, emailBody);
            //                #endregion
            //            }

            //        }
            //    }
            //}



        }
        public static async Task Execute(string email, string subject, string message)
        {
            try
            {
                
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress("webmail@rankridefantasy.com", "Rank Ride")
                };
                mail.To.Add(new MailAddress(email));
                //mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient("smtp.office365.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("webmail@rankridefantasy.com", "ride118Web");
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }

            catch (Exception ex)
            {
            }
        }
    }
}
