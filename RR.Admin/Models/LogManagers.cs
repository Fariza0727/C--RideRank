using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RR.Admin.Models;
using RR.Core;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RR.Admin.Models
{
    public class LogManagers
    {
        public static IEnumerable<LogResponse> GetLogs(string filepath)
        {
            using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                string json = sr.ReadToEnd();
                string jsonArray = string.Concat("[", json.TrimEnd(','), "]");
                return JsonConvert.DeserializeObject<IEnumerable<LogResponse>>(jsonArray);
            }
            
        }

        public static string GetLogDetails(Object obj, string parent = "")
        {

            Type t = obj.GetType();
            //Console.WriteLine("Type is: {0}", t.Name);
            PropertyInfo[] props = t.GetProperties();
            //Console.WriteLine("Properties (N = {0}):", props.Length);

            List<string> tblTr = new List<string>();
            foreach (var prop in props)
            {
                
                if (prop.GetIndexParameters().Length == 0)
                {
                    if (prop.Name.ToLower().Equals(nameof(Eventid).ToLower()))
                    {
                        if(prop.GetValue(obj)!=null)
                            tblTr.Add(GetLogDetails((Eventid)prop.GetValue(obj), "Event:"));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(prop.GetValue(obj)?.ToString()))
                        {
                             tblTr.Add($"<div class=\"logrow\"><label>{parent} {prop.Name}</label><span>{prop.GetValue(obj)}</span></div>");
                        }
                    }

                   
                    //Console.WriteLine("{0} ({1}): {2}", prop.Name,
                    //                  prop.PropertyType.Name,
                    //                  prop.GetValue(obj));
                }
                else
                {
                   
                    //Console.WriteLine("{0} ({1}): <Indexed>", prop.Name,
                    //                  prop.PropertyType.Name);
                }
            }
            return string.Join(string.Empty, tblTr);
        }

    }

    public class LogUserNameMiddleware
    {
        private readonly RequestDelegate next;

        public LogUserNameMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            LogContext.PushProperty("UserName", context.User?.FindFirst(ClaimTypes.Name)?.Value);
            LogContext.PushProperty("UserId", context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return next(context);
        }
    }

    public class LogResponse
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        
        public Properties Properties { get; set; }
        public Renderings Renderings { get; set; }
    }

    public class Properties
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string SourceContext { get; set; }
        public string Protocol { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public object ContentLength { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string PathBase { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string HostingRequestStartingLog { get; set; }
        public Eventid EventId { get; set; }
        public string RequestId { get; set; }
        public string RequestPath { get; set; }
        public object CorrelationId { get; set; }
        public string EndpointName { get; set; }
        public string RouteData { get; set; }
        public string MethodInfo { get; set; }
        public string Controller { get; set; }
        public string AssemblyName { get; set; }
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string version { get; set; }
        public string contextType { get; set; }
        public string provider { get; set; }
        public string options { get; set; }
        public string ValidationState { get; set; }
        public string ActionResult { get; set; }
        public float ElapsedMilliseconds { get; set; }
        public string ViewName { get; set; }
        public string elapsed { get; set; }
        public string parameters { get; set; }
        public string commandType { get; set; }
        public int commandTimeout { get; set; }
        public string newLine { get; set; }
        public string commandText { get; set; }
        public string ViewComponentName { get; set; }
        public string ViewComponentId { get; set; }
        public string expression { get; set; }
        public string[] Scope { get; set; }
        public int StatusCode { get; set; }
        public string HostingRequestFinishedLog { get; set; }
    }

    public class Eventid
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Renderings
    {
        public Hostingrequeststartinglog[] HostingRequestStartingLog { get; set; }
        public Hostingrequestfinishedlog[] HostingRequestFinishedLog { get; set; }
    }

    public class Hostingrequeststartinglog
    {
        public string Format { get; set; }
        public string Rendering { get; set; }
    }

    public class Hostingrequestfinishedlog
    {
        public string Format { get; set; }
        public string Rendering { get; set; }
    }

}
