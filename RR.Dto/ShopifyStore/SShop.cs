using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class ShopifyObject
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("admin_graphql_api_id")]
        public string AdminGraphQLAPIId { get; set; }
    }
    public class SShop : ShopifyObject
    {
        [JsonProperty("plan_name")]
        public string PlanName { get; set; }
        [JsonProperty("plan_display_name")]
        public string PlanDisplayName { get; set; }
        [JsonProperty("password_enabled")]
        public bool? PasswordEnabled { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("primary_locale")]
        public string PrimaryLocale { get; set; }
        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("province_code")]
        public string ProvinceCode { get; set; }
        [JsonProperty("ships_to_countries")]
        public string ShipsToCountries { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("shop_owner")]
        public string ShopOwner { get; set; }
        [JsonProperty("tax_shipping")]
        public bool? TaxShipping { get; set; }
        [JsonProperty("taxes_included")]
        public bool? TaxesIncluded { get; set; }
        [JsonProperty("county_taxes")]
        public bool? CountyTaxes { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("iana_timezone")]
        public string IANATimezone { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("has_storefront")]
        public bool? HasStorefront { get; set; }
        [JsonProperty("setup_required")]
        public bool? SetupRequired { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("weight_unit")]
        public string WeightUnit { get; set; }
        [JsonProperty("myshopify_domain")]
        public string MyShopifyDomain { get; set; }
        [JsonProperty("money_with_currency_format")]
        public string MoneyWithCurrencyFormat { get; set; }
        [JsonProperty("address1")]
        public string Address1 { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        [JsonProperty("country_name")]
        public string CountryName { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }
        [JsonProperty("customer_email")]
        public string CustomerEmail { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("multi_location_enabled")]
        public bool? MultiLocationEnabled { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("enabled_presentment_currencies")]
        public string[] EnabledPresentmentCurrencies { get; set; }
        [JsonProperty("force_ssl")]
        public bool? ForceSSL { get; set; }
        [JsonProperty("google_apps_domain")]
        public string GoogleAppsDomain { get; set; }
        [JsonProperty("google_apps_login_enabled")]
        public string GoogleAppsLoginEnabled { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        [JsonProperty("money_format")]
        public string MoneyFormat { get; set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
