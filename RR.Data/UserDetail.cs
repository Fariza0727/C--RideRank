using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class UserDetail
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string OptPhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public int? State { get; set; }
        public int? Country { get; set; }
        public string Banking { get; set; }
        public string Avtar { get; set; }
        public bool? LeagueNotification { get; set; }
        public bool? IsBlock { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? Dob { get; set; }
        public int? WalletToken { get; set; }
        public DateTime? SubscriptionExpiryDate { get; set; }
        public string PlayerType { get; set; }
        public string ZipCode { get; set; }
        public string PaymentMode { get; set; }
        public string TeamName { get; set; }
        public bool? IsUserOnline { get; set; }
        public bool? IsPaidMember { get; set; }
        public long? ShopifyCustomerId { get; set; }
        public string ShopifyMembership { get; set; }
        public bool? IsNotifyEmail { get; set; }
        public bool? IsNotifySms { get; set; }

        public string ReferralCode { get; set; }
        public int? ReferredCustomers { get; set; }

        public string CustomReferralCode { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
