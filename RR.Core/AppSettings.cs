namespace RR.Core
{
    public class AppSettings
    {
        /// <summary>
        /// Secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Redis CacheConnection
        /// </summary>
        public string CacheConnection { get; set; }

        /// <summary>
        /// Redis CacheEnvKey
        /// </summary>
        public string CacheEnvKey { get; set; }

        /// <summary>
        /// Voucherify Api Key
        /// </summary>
        public string VoucherifyApiKey { get; set; }

        /// <summary>
        /// Voucherify Api Token
        /// </summary>
        public string VoucherifyApiToken { get; set; }

        /// <summary>
        /// Voucherify Referral Program Campaign Name
        /// </summary>
        public string VoucherifyReferralCampaignName { get; set; }

        /// <summary>
        /// Voucherify Referral Program Credit per one Referred
        /// </summary>
        public int VoucherifyReferralPoints { get; set; }

        /// <summary>
        /// Constant Contacting AccessToken
        /// </summary>
        public string ConstantContactAccessToken { get; set; }

        /// <summary>
        /// Constant Contacting RefreshToken
        /// </summary>
        public string ConstantContactRefreshToken { get; set; }

        /// <summary>
        /// Constant Contacting ListId
        /// </summary>
        public string ConstantContactListId { get; set; }

        /// <summary>
        /// AzureBlobConnectionString
        /// </summary>
        public string AzureBlobConnectionString { get; set; }

        /// <summary>
        /// AzureBlobContainerName
        /// </summary>
        public string AzureBlobContainerName { get; set; }

        /// <summary>
        /// PayPalSandbox
        /// </summary>
        public bool PayPalSandbox { get; set; }

        /// <summary>
        /// PayPalClientID
        /// </summary>
        public string PayPalClientID { get; set; }

        /// <summary>
        /// PayPalClientSecret
        /// </summary>
        public string PayPalClientSecret { get; set; }

        /// <summary>
        /// Calcutta our Drag
        /// </summary>
        public decimal CalcuttaDrag { get; set; }

        /// <summary>
        /// Calcutta our Drag
        /// </summary>
        public int CalcuttaPlaceBreak { get; set; }

        /// <summary>
        /// Redis SwaggerUser
        /// </summary>
        public string SwaggerUser { get; set; }

        /// <summary>
        /// Redis SwaggerUserPassword
        /// </summary>
        public string SwaggerUserPassword { get; set; }
        /// <summary>
        /// Connection String
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Api Url
        /// </summary>
        public string APIUrl { get; set; }

        /// <summary>
        /// ProfilePicPath
        /// </summary>
        public string ProfilePicPath { get; set; }

        /// <summary>
        /// AwardPicPath
        /// </summary>
        public string AwardPicPath { get; set; }

        /// <summary>
        /// NewsImagePath
        /// </summary>
        public string NewsImagePath { get; set; }

        /// <summary>
        /// AdminSiteURL
        /// </summary>
        public string AdminSiteURL { get; set; }

        /// <summary>
        /// MainSiteURL
        /// </summary>
        public string MainSiteURL { get; set; }
        /// <summary>
        /// StoreURL
        /// </summary>
        public string StoreURL { get; set; }

        /// <summary>
        /// ToEmailAddress
        /// </summary>
        public string ToEmailAddress { get; set; }
        public string BannerImagePath { get; set; }

        /// <summary>
        /// Updated on 2/1/2020
        /// </summary>
        public string DefaultToken { get; set; }
        public string BannerSharedPath { get; set; }
        public string NewsSharedPath { get; set; }
        public string BannerFolderPath { get; set; }
        public int ChatAutoDeleteDays { get; set; }

        public string SaveasNewsWeb { get; set; }
        public string SaveasBannersWeb { get; set; }
        public StaticContents StaticContents { get; set; }

        public string ShopifyHost { get; set; }
        public string ShopifyAPIsVersion { get; set; }
        public string ShopifyAPIsKey { get; set; }
        public string DateTimeFormate { get; set; }
        public string SaveasbullriderImage { get; set; }
        public string Logfilepath { get; set; }
        public string ThirdPartyApikey { get; set; }
        public bool ShowDynamicBullRiderImg { get; set; }
        public string MembershipURL { get; set; }
        public bool ShowMembership { get; set; }
        public bool IsDebugMode { get; set; }
        public int CanUpdateHr { get; set; }
        public string CustomerServiceEmail { get; set; }
    }
}
