using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using RR.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RR.Service
{
    public class ConstantContactHelper
    {
        private string _baseURL = "https://api.cc.email/v3/idfed";
        private string _clientID = "cd0df862-bc50-43dc-ba66-e6d92c6e5e17";
        private string _clientSecret = "ZOCYJFMMFY4BuIyc4A4h0A";
        private string _redirectURL = "";
        private string accessToken = "2Guk7g6MLpmBLlohYF4kADSoTmEj";
        private string refreshToken = "VzNTP3xHXvycZLxzVmJClhSzaLOJVxoIms4Fixs5RQ";
        private string contactListId = "07884aec-02b7-11ec-81ff-fa163efab12a";
        private AppSettings _appSettings;
        public ConstantContactHelper(AppSettings appSettings)
        {
            accessToken = appSettings.ConstantContactAccessToken;
            refreshToken = appSettings.ConstantContactRefreshToken;
            _redirectURL = HttpUtility.UrlEncode(appSettings.MainSiteURL + "ctct/");
            _appSettings = appSettings;
        }

        public async Task<string> GetAccessToken(string code)
        {
            string jsonResult="";
            
            string authUrl = "https://idfed.constantcontact.com/as/token.oauth2?code=" + code + "&redirect_uri=" + _redirectURL + "&grant_type=authorization_code&scope=contact_data";
            try
            {
                HttpClient client = new HttpClient();
                var authToken = Encoding.ASCII.GetBytes($"{_clientID}:{_clientSecret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var response = await client.PostAsync(authUrl, null);

                jsonResult = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<CCTokenInfo>(jsonResult);
                accessToken = dto.access_token;
                refreshToken = dto.refresh_token;
                _appSettings.ConstantContactAccessToken = accessToken;
                _appSettings.ConstantContactRefreshToken = refreshToken;

            }
            catch (Exception ex)
            {
                
                
            }
            return jsonResult;
        }
        
        public async Task<string> CreateOrUpdateEmail(string email)
        {
            string baseURL = "https://api.cc.email/v3/contacts/sign_up_form";
            string contactID = "";
            try
            {
                HttpClient client = new HttpClient();
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var newContact = new ConstantContact();

                newContact.email_address = email;

                newContact.list_memberships = new List<string>
                {
                    contactListId
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(newContact);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(baseURL, data);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await GetRefreshToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await client.PostAsync(baseURL, data);
                }
                var responseString = await response.Content.ReadAsStringAsync();
                var tmpData = JsonConvert.DeserializeObject<CCCreateUpdateResponse>(responseString);
                contactID = tmpData.contact_id;
            }
            catch (Exception ex)
            {


            }
            return contactID;
        }
        public async Task<string> GetRefreshToken()
        {
            string jsonResult = "";
            string authUrl = "https://idfed.constantcontact.com/as/token.oauth2?refresh_token=" + refreshToken + "&grant_type=refresh_token";
            try
            {
                HttpClient client = new HttpClient();
                var authToken = Encoding.ASCII.GetBytes($"{_clientID}:{_clientSecret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                var response = await client.PostAsync(authUrl, null);

                jsonResult = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<CCTokenInfo>(jsonResult);
                accessToken = dto.access_token;
                refreshToken = dto.refresh_token;
                _appSettings.ConstantContactAccessToken = accessToken;
                _appSettings.ConstantContactRefreshToken = refreshToken;

            }
            catch (Exception ex)
            {


            }
            return jsonResult;
        }
        public async Task<string> Unsubscribed(string email)
        {
            var contactID = await CreateOrUpdateEmail(email);
            string baseURL = "https://api.cc.email/v3/contacts/" + contactID;
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var upContact = new CCUnsubscribeDto();

                var emailAdress = new CCEmailAddress();
                emailAdress.permission_to_send = "unsubscribed";
                emailAdress.opt_out_reason = "No longer interested";
                emailAdress.address = email;
                upContact.email_address = emailAdress;
                upContact.update_source = "Contact";

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(upContact);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(baseURL, data);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await GetRefreshToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await client.PostAsync(baseURL, data);
                }
                var responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {


            }
            return "";
        }
        public async Task<string> Resubscribed(string email)
        {
            var contactID = await CreateOrUpdateEmail(email);
            string baseURL = "https://api.cc.email/v3/contacts/" + contactID;
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var upContact = new CCResubscribeDto();

                var emailAdress = new CCEmailAddress();
                emailAdress.permission_to_send = "explicit";
                emailAdress.opt_out_reason = "";
                emailAdress.address = email;
                upContact.email_address = emailAdress;
                upContact.update_source = "Contact";

                upContact.list_memberships = new List<string>
                {
                    contactListId
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(upContact);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(baseURL, data);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await GetRefreshToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    response = await client.PostAsync(baseURL, data);
                }
                var responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {


            }
            return "";
        }
    }
    public class ConstantContact
    {
        public string email_address { get; set; }
        public List<string> list_memberships { get; set; }
    }
    public class CCUnsubscribeDto
    {
        public string update_source { get; set; }
        public CCEmailAddress email_address { get; set; }
    }
    public class CCResubscribeDto
    {
        public string update_source { get; set; }
        public List<string> list_memberships { get; set; }
        public CCEmailAddress email_address { get; set; }
    }
    public class CCEmailAddress
    {
        public string address { get; set; }
        public string opt_out_reason { get; set; }
        public string permission_to_send { get; set; }
    }
    public class CCTokenInfo
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
    }
    public class CCCreateUpdateResponse
    {
        public string contact_id { get; set; }
        public string action { get; set; }
    }
}
