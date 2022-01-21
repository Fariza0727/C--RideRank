using log4net;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace RR.Core
{
    public class WebProxy
    {
        #region Contructor

        public WebProxy(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        public static IConfiguration Configuration { get; private set; }

        //public static string apiUrl =
        //     string.Format($"{0}?key={1}&&data=",
        //          Configuration.GetSection("AppSettings").GetSection("APIUrl").Value,
        //          Configuration.GetSection("AppSettings").GetSection("Secret").Value);

        //https://probullstats.com/rr/rrapi.php?key=test01459rrtest&view=json&data=
        public static string apiUrl = "https://probullstats.com/rr/rrapi.php?key=8f487e0a-cb41-42cf-ab18-08cfd71ab055&data=";
        public static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// API Response
        /// </summary>
        /// <param name="eventParam">An Event Parameter Passed in Api Url</param>
        /// <returns>Response from that Api</returns>
        public async static Task<string> APIResponse(string eventParam, string key = "")
        {
            string jsonResult;
            try
            {
                var url_ = apiUrl + eventParam;
                if (!string.IsNullOrEmpty(key))
                {
                    var uri = new Uri(url_);
                    var qs = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    qs.Set("key", key);

                    var uriBuilder = new UriBuilder(uri);
                    uriBuilder.Query = qs.ToString();
                    url_ = uriBuilder.Uri.ToString();

                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url_); //URI  
                request.Accept = "application/json";
                request.ContentType = "application/json";
                var response = await request.GetResponseAsync();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResult = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }

            return jsonResult;
        }

        public async static Task<string> CalcuttaAPIResponse(string eventParam)
        {
            string jsonResult;
            try
            {
                var baseURL = "https://chuteboss.com/api/";
                var url_ = baseURL + eventParam;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url_); //URI  
                //request.Accept = "application/json";
                //request.ContentType = "application/json";
                var response = await request.GetResponseAsync();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResult = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }

            return jsonResult;
        }

        public async static Task<string> PayoutBasicAPIResponse()
        {
            string jsonResult;
            try
            {
                var baseURL = "https://www.abbireg.com/jpad_update/bc_paytable.json";


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseURL); //URI  
                //request.Accept = "application/json";
                //request.ContentType = "application/json";
                var response = await request.GetResponseAsync();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResult = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }

            return jsonResult;
        }
        //public static ResponseToken PostForm()
        //{
        //     string jsonString;
        //     try
        //     {
        //          string url = "https://uat-accounts.payu.in/oauth/token";
        //          HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //          request.Method = "POST";

        //          StringBuilder postData = new StringBuilder();
        //          postData.Append(String.Format("client_id={0}&", "6f8bb4951e030d4d7349e64a144a534778673585f86039617c167166e9154f7e"));
        //          postData.Append(String.Format("grant_type={0}&", "password"));
        //          postData.Append(String.Format("scope={0}&", "create_payout_transactions"));
        //          postData.Append(String.Format("username={0}&", "garimachauhan24@gmail.com"));
        //          postData.Append(String.Format("password={0}", "Tester@123"));

        //          ASCIIEncoding ascii = new ASCIIEncoding();
        //          byte[] postBytes = ascii.GetBytes(postData.ToString());

        //          request.ContentType = "application/x-www-form-urlencoded";
        //          request.ContentLength = postBytes.Length;

        //          using (Stream requestStream = request.GetRequestStream())
        //          using (StreamWriter writer = new StreamWriter(requestStream, Encoding.ASCII))
        //          {
        //               writer.Write(postData.ToString());
        //          }

        //          HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //          // careful, non-2xx responses will throw an exception
        //          using (Stream responseStream = response.GetResponseStream())
        //          using (StreamReader reader = new StreamReader(responseStream))
        //          {
        //               jsonString = reader.ReadToEnd();
        //          }
        //     }
        //     catch (Exception ex)
        //     {

        //          throw;
        //     }
        //     var data = JsonConvert.DeserializeObject<ResponseToken>(jsonString);
        //     return data;
        //}
    }

    //public class ResponseToken
    //{
    //     public string access_token { get; set; }
    //     public string token_type { get; set; }
    //     public string expires_in { get; set; }
    //     public string refresh_token { get; set; }
    //     public string scope { get; set; }
    //     public string created_at { get; set; }
    //     public string user_uuid { get; set; }
    //}
}
