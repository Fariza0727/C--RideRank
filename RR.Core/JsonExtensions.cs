using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RR.Core
{
    public static class JsonExtensions
    {

        public static void ShopifyUserStatus(this Json_ShopifyObject _object, IHostingEnvironment env)
        {
            try
            {
                var filepath = string.Concat(env.WebRootPath, $"/Jsondata/ShopifyUsers_{DateTime.Now.ToString("ddMMyyyy")}.json");
                var file = new FileInfo(filepath);
                if (!file.Exists)
                    file.Directory.Create();

                using (var fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    string json = "[]";
                    var jsondata = File.ReadAllText(filepath);
                    if (!string.IsNullOrEmpty(jsondata))
                        json = jsondata;

                    //var jsonObj = JObject.Parse(json);
                    var userArrary = JsonConvert.DeserializeObject<List<Json_ShopifyObject>>(json)??new List<Json_ShopifyObject>();
                    var newUser = userArrary.FirstOrDefault(obj => obj.Customerid == _object.Customerid);
                    if (newUser != null)
                    {
                        newUser.Error = _object.Error;
                        newUser.Progress = _object.Progress;
                        newUser.CreateDate = _object.CreateDate;
                        newUser.UpdateDate = _object.UpdateDate;
                    }
                    else
                    {
                        userArrary.Add(_object);
                    }
                   
                    //jsonObj["shopifyuser"] = JsonConvert.SerializeObject(userArrary).ToString();
                    string newJsonResult = JsonConvert.SerializeObject(userArrary, Formatting.None).ToString();
                    File.WriteAllText(filepath, newJsonResult);
                    
                }
            }
            catch (Exception ex)
            {
                 
            }
            
        }

        //private string jsonFile = @"C:\Users\mk9\Documents\visual studio 2015\Projects\JsonAddAndUpdate\JsonAddAndUpdate\user.json";
        //private void AddCompany()
        //{
        //    Console.WriteLine("Enter Company ID : ");
        //    var companyId = Console.ReadLine();
        //    Console.WriteLine("\nEnter Company Name : ");
        //    var companyName = Console.ReadLine();

        //    var newCompanyMember = "{ 'companyid': " + companyId + ", 'companyname': '" + companyName + "'}";
        //    try
        //    {
        //        var json = File.ReadAllText(jsonFile);
        //        var jsonObj = JObject.Parse(json);
        //        var experienceArrary = jsonObj.GetValue("experiences") as JArray;
        //        var newCompany = JObject.Parse(newCompanyMember);
        //        experienceArrary.Add(newCompany);

        //        jsonObj["experiences"] = experienceArrary;
        //        string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
        //        File.WriteAllText(jsonFile, newJsonResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Add Error : " + ex.Message.ToString());
        //    }
        //}

        //private void UpdateCompany()
        //{
        //    string json = File.ReadAllText(jsonFile);

        //    try
        //    {
        //        var jObject = JObject.Parse(json);
        //        JArray experiencesArrary = (JArray)jObject["experiences"];
        //        Console.Write("Enter Company ID to Update Company : ");
        //        var companyId = Convert.ToInt32(Console.ReadLine());

        //        if (companyId > 0)
        //        {
        //            Console.Write("Enter new company name : ");
        //            var companyName = Convert.ToString(Console.ReadLine());

        //            foreach (var company in experiencesArrary.Where(obj => obj["companyid"].Value<int>() == companyId))
        //            {
        //                company["companyname"] = !string.IsNullOrEmpty(companyName) ? companyName : "";
        //            }

        //            jObject["experiences"] = experiencesArrary;
        //            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
        //            File.WriteAllText(jsonFile, output);
        //        }
        //        else
        //        {
        //            Console.Write("Invalid Company ID, Try Again!");
        //            UpdateCompany();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Console.WriteLine("Update Error : " + ex.Message.ToString());
        //    }
        //}

        //private void DeleteCompany()
        //{
        //    var json = File.ReadAllText(jsonFile);
        //    try
        //    {
        //        var jObject = JObject.Parse(json);
        //        JArray experiencesArrary = (JArray)jObject["experiences"];
        //        Console.Write("Enter Company ID to Delete Company : ");
        //        var companyId = Convert.ToInt32(Console.ReadLine());

        //        if (companyId > 0)
        //        {
        //            var companyName = string.Empty;
        //            var companyToDeleted = experiencesArrary.FirstOrDefault(obj => obj["companyid"].Value<int>() == companyId);

        //            experiencesArrary.Remove(companyToDeleted);

        //            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
        //            File.WriteAllText(jsonFile, output);
        //        }
        //        else
        //        {
        //            Console.Write("Invalid Company ID, Try Again!");
        //            UpdateCompany();
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //private void GetUserDetails()
        //{
        //    var json = File.ReadAllText(jsonFile);
        //    try
        //    {
        //        var jObject = JObject.Parse(json);

        //        if (jObject != null)
        //        {
        //            Console.WriteLine("ID :" + jObject["id"].ToString());
        //            Console.WriteLine("Name :" + jObject["name"].ToString());

        //            var address = jObject["address"];
        //            Console.WriteLine("Street :" + address["street"].ToString());
        //            Console.WriteLine("City :" + address["city"].ToString());
        //            Console.WriteLine("Zipcode :" + address["zipcode"]);
        //            JArray experiencesArrary = (JArray)jObject["experiences"];
        //            if (experiencesArrary != null)
        //            {
        //                foreach (var item in experiencesArrary)
        //                {
        //                    Console.WriteLine("company Id :" + item["companyid"]);
        //                    Console.WriteLine("company Name :" + item["companyname"].ToString());
        //                }

        //            }
        //            Console.WriteLine("Phone Number :" + jObject["phoneNumber"].ToString());
        //            Console.WriteLine("Role :" + jObject["role"].ToString());

        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        //static void Main(string[] args)
        //{
        //    Program objProgram = new JsonAddAndUpdate.Program();

        //    Console.WriteLine("Choose Your Options : 1 - Add, 2 - Update, 3 - Delete, 4 - Select \n");
        //    var option = Console.ReadLine();
        //    switch (option)
        //    {
        //        case "1":
        //            objProgram.AddCompany();
        //            break;
        //        case "2":
        //            objProgram.UpdateCompany();
        //            break;
        //        case "3":
        //            objProgram.DeleteCompany();
        //            break;
        //        case "4":
        //            objProgram.GetUserDetails();
        //            break;
        //        default:
        //            Main(null);
        //            break;
        //    }
        //    Console.ReadLine();

        //}
    }

    public class Json_ShopifyObject
    {
        public string Customerid { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public string Error { get; set; }
        public List<string> Progress { get; set; } = new List<string>();
    }
}

  
  
