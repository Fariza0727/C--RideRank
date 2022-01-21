using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class SAddress : ShopifyObject
    {
        public SAddress() { }

        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("longitude")]
        public decimal? Longitude { get; set; }
        [JsonProperty("latitude")]
        public decimal? Latitude { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("default")]
        public bool? Default { get; set; }
        [JsonProperty("country_name")]
        public string CountryName { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("company")]
        public string Company { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("address2")]
        public string Address2 { get; set; }
        [JsonProperty("address1")]
        public string Address1 { get; set; }
        [JsonProperty("province_code")]
        public string ProvinceCode { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("id")]
        public long? addressId { get; set; }
        [JsonProperty("customer_id")]
        public long? CustomerId { get; set; }
        
    }

    public class SAddressDto : ShopifyObject
    {
        public SAddressDto() { }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("address1")]
        public string Address1 { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("default")]
        public bool? IsDefault { get; set; }
        
        [JsonProperty("id")]
        public long? addressId { get; set; }
        [JsonProperty("customer_id")]
        public long? CustomerId { get; set; }
    }
}
