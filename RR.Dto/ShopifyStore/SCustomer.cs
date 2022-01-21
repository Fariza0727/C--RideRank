using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class SCustomer : ShopifyObject
    {
        public SCustomer() { }

        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
        [JsonProperty("total_spent")]
        public decimal? TotalSpent { get; set; }
        [JsonProperty("tax_exempt")]
        public bool? TaxExempt { get; set; }
        [JsonProperty("tags")]
        public string Tags { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("orders_count")]
        public int? OrdersCount { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
        [JsonProperty("last_order_name")]
        public string LastOrderName { get; set; }
        [JsonProperty("last_order_id")]
        public long? LastOrderId { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("multipass_identifier")]
        public string MultipassIdentifier { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("default_address")]
        public SAddress DefaultAddress { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }
        [JsonProperty("addresses")]
        public IEnumerable<SAddress> Addresses { get; set; }
        [JsonProperty("accepts_marketing")]
        public bool? AcceptsMarketing { get; set; }
        [JsonProperty("verified_email")]
        public bool? VerifiedEmail { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("password_confirmation")]
        public string ConfirmPassword { get; set; }
        [JsonProperty("send_email_welcome")]
        public bool SendWelComeEmail { get; set; }
        [JsonProperty("metafields")]
        public IEnumerable<SMetaField> Metafields { get; set; }
    }

    /// <summary>
    /// Add Customer Dto
    /// </summary>
    public class SCustomerDto : ShopifyObject
    {
        public SCustomerDto() { }
        public string UserId { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("verified_email")]
        public bool VerifiedEmail { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("password_confirmation")]
        public string ConfirmPassword { get; set; }
        [JsonProperty("send_email_welcome")]
        public bool SendWelComeEmail { get; set; }
        [JsonProperty("accepts_marketing")]
        public bool AcceptsMarketing { get; set; }
        [JsonProperty("addresses")]
        public IEnumerable<SAddressDto> Addresses { get; set; }
        
    }
}
