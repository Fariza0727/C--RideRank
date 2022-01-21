using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using RR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
    public class VoucherHelper
    {
        private Voucherify.Api _api ;
        private AppSettings _appSettings;
        public VoucherHelper(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _api = new Voucherify.Api(
                    _appSettings.VoucherifyApiKey,
                    _appSettings.VoucherifyApiToken
            ).WithSSL().WithHost("us1.api.voucherify.io");
        }
        public async Task<string> createReferalCodeAsync(IdentityUser user)
        {
            Voucherify.DataModel.Contexts.VoucherPublishSingle context = new Voucherify.DataModel.Contexts.VoucherPublishSingle();
            Voucherify.DataModel.Contexts.Customer customer = new Voucherify.DataModel.Contexts.Customer();
            customer.Email = user.Email;
            customer.Name = user.UserName;
            customer.SourceId = user.Email;

            context.Campaign = _appSettings.VoucherifyReferralCampaignName;
            context.Customer = customer;
            string referCode = "";
            try
            {
                var response = await _api.Distributions.Publish(context);
                referCode = response.Voucher.Code;
            }catch(Exception e)
            {
                referCode = "";
            }
            
            return referCode;
        }
        public async Task<string> signupReferralCodeAsync(IdentityUser user, string referralCode)
        {
            Voucherify.DataModel.Contexts.Customer customer = new Voucherify.DataModel.Contexts.Customer();
            customer.Email = user.Email;
            customer.Name = user.UserName;
            customer.SourceId = user.Email;
            ReferralCode refCode = new ReferralCode();
            refCode.Code = referralCode;
            
            CustomSignUpEvent eventObj = new CustomSignUpEvent();
            
            eventObj.Event = "signed_up";
            eventObj.Customer = customer;
            eventObj.ReferralCode = refCode;
            
            var response = await _api.Events.Create(eventObj);

            return response.ToString();
        }

       
    }

    [JsonObject]
    public class ReferralCode
    {
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }
    }
    [JsonObject]
    public class CustomSignUpEvent: Voucherify.DataModel.Contexts.EventCreate
    {
        [JsonProperty(PropertyName = "referral")]
        public ReferralCode ReferralCode { get; set; }
    }
}
