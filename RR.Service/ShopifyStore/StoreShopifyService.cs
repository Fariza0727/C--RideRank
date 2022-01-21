using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RR.Core;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
    public class StoreShopifyService : IStoreShopifyService
    {
        private readonly AppSettings _appSettings;

        public StoreShopifyService( IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<SCustomer> AddEditCustomerAsync(SCustomerDto entity)
        {
            var data = new Dictionary<string, SCustomerDto>() { { "customer", entity } };
            var postDatsting = JsonConvert.SerializeObject(data).Replace(@"\","");
            //JObject data = JObject.Parse("{}");
            //data["customer"] = json_.ToString();
            //var postDatsting = data.ToString();
            ShopifyResponse result_ = new ShopifyResponse();
            string endPoint = "customers.json";

            if (entity.Id > 0)
            {
                endPoint = string.Concat("customers/", entity.Id, ".json");
                result_ = await PutAsync("customer", endPoint, postDatsting.ToString());
            }
            else
            {
                result_ = await PostAsync("customer", endPoint, postDatsting.ToString());
            }
          
            var srirlize = JsonConvert.DeserializeObject<SCustomer>(result_.JsonObject);
            return await Task.FromResult(srirlize);
        }

        public async Task<SCustomer> GetCustomerAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var data = await GetAsync("customer", string.Concat("customers/", id, ".json"));
                var srirlize = JsonConvert.DeserializeObject<SCustomer>(data.JsonObject);
                return await Task.FromResult(srirlize);
            }
            return await Task.FromResult(default(SCustomer));
             
        }

        public async Task<bool> DeleteCustomerAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var data = await DeleteAsync("customer", string.Concat("customers/", id, ".json"));
                return await Task.FromResult(data.Success);
            }
            return await Task.FromResult(false);
        }

        public async Task<SWebhook> AddEditWebhookAsync(SWebhook entity)
        {
            var data = new Dictionary<string, SWebhook>() { { "webhook", entity } };
            var postDatsting = JsonConvert.SerializeObject(data).Replace(@"\", "");
            ShopifyResponse result_ = new ShopifyResponse();
            string endPoint = "webhooks.json";

            if (entity.Id > 0)
            {
                endPoint = string.Concat("webhooks/", entity.Id, ".json");
                result_ = await PutAsync("webhook", endPoint, postDatsting.ToString());
            }
            else
            {
                result_ = await PostAsync("webhook", endPoint, postDatsting.ToString());
            }

            var srirlize = JsonConvert.DeserializeObject<SWebhook>(result_.JsonObject);
            return await Task.FromResult(srirlize);
        }

        public async Task<IEnumerable<SWebhook>> GetWebhookAsync()
        {
            var data = await GetAsync("webhooks", string.Concat("webhooks.json"));
            var srirlize = JsonConvert.DeserializeObject<IEnumerable<SWebhook>>(data.JsonObject);
            return await Task.FromResult(srirlize);
        }

        public async Task<SWebhook> GetWebhookAsync(long id)
        {
            if (id > 0)
            {
                var data = await GetAsync("webhook", string.Concat($"webhooks/{id}.json"));
                var srirlize = JsonConvert.DeserializeObject<SWebhook>(data.JsonObject);
                return await Task.FromResult(srirlize);
            }

            return await Task.FromResult(default(SWebhook));
        }


        public void Dispose()
        {
            
        }

        private async Task<ShopifyResponse> GetAsync(string objectName, string endpoint)
        {
            ShopifyResponse jsonResult = new ShopifyResponse();
            try
            {
                HttpWebRequest request = getHttpRequest(endpoint, "GET");
                var response = await request.GetResponseAsync();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResult.JsonObject = reader.ReadToEnd();
                    jsonResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                jsonResult.Message = ex.GetActualError();
                jsonResult.Success = false;
            }
            if (jsonResult.Success)
            {
                JObject object_ = JObject.Parse(jsonResult.JsonObject);
                jsonResult.JsonObject = object_[objectName].ToString();
            }
            return jsonResult;
        }
        private async Task<ShopifyResponse> PutAsync(string objectName, string endpoint, string Postdata)
        {
            ShopifyResponse jsonResult = new ShopifyResponse();
            try
            {

                HttpWebRequest request = getHttpRequest(endpoint, "PUT");
                //// Create POST data and convert it to a byte array.
                byte[] byteArray = Encoding.UTF8.GetBytes(Postdata);
                request.ContentLength = byteArray.Length;

                // Get the request stream.
                Stream dataStream = await request.GetRequestStreamAsync();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    jsonResult.JsonObject = reader.ReadToEnd();
                    jsonResult.Success = true;
                }

                // Close the response.
                response.Close();

            }
            catch (Exception ex)
            {
                jsonResult.Message = ex.GetActualError();
                jsonResult.Success = false;
            }
            if (jsonResult.Success)
            {
                JObject object_ = JObject.Parse(jsonResult.JsonObject);
                jsonResult.JsonObject = object_[objectName].ToString();
            }
            return jsonResult;
        }
        private async Task<ShopifyResponse> PostAsync(string objectName, string endpoint, string Postdata)
        {
            ShopifyResponse jsonResult = new ShopifyResponse();
            try
            {

                HttpWebRequest request = getHttpRequest(endpoint, "POST");
                //// Create POST data and convert it to a byte array.
                byte[] byteArray = Encoding.UTF8.GetBytes(Postdata);
                request.ContentLength = byteArray.Length;

                // Get the request stream.
                Stream dataStream = await request.GetRequestStreamAsync();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                // Get the stream containing content returned by the server.
                // The using block ensures the stream is automatically closed.
                using (dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    jsonResult.JsonObject = reader.ReadToEnd();
                    jsonResult.Success = true;
                }

                // Close the response.
                response.Close();

            }
            catch (Exception ex)
            {
                jsonResult.Message = ex.GetActualError();
                jsonResult.Success = false;
            }
            if (jsonResult.Success)
            {
                JObject object_ = JObject.Parse(jsonResult.JsonObject);
                jsonResult.JsonObject = object_[objectName].ToString();
            }
            return jsonResult;
        }
        private async Task<ShopifyResponse> DeleteAsync(string objectName, string endpoint)
        {
            ShopifyResponse jsonResult = new ShopifyResponse();
            try
            {
                HttpWebRequest request = getHttpRequest(endpoint, "DELETE");
                var response = await request.GetResponseAsync();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResult.JsonObject = reader.ReadToEnd();
                    jsonResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                jsonResult.Message = ex.GetActualError();
                jsonResult.Success = false;
            }
            return jsonResult;
        }
        private HttpWebRequest getHttpRequest(string endPoint, string MethodType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                    .Create(string.Concat(_appSettings.ShopifyHost, "api/", _appSettings.ShopifyAPIsVersion, "/", endPoint));

            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Method = MethodType;
            request.Headers["X-Shopify-Access-Token"] = _appSettings.ShopifyAPIsKey;

            return request;
        }

        public async Task<IEnumerable<SCustomer>> GetCustomersAsync()
        {
            var data = await GetAsync("customers", string.Concat("customers.json"));
            var srirlize = JsonConvert.DeserializeObject<IEnumerable<SCustomer>>(data.JsonObject);
            return await Task.FromResult(srirlize);
        }
    }
}
