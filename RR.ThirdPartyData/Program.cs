using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace RR.ThirdPartyData
{
    public class Program
    {
        /// <summary>
        /// Main Method Of Console App
        /// </summary>
        /// <param name="args">An Argument</param>
        public static void Main(string[] args)
        {
            string savePath = ConfigurationManager.AppSettings["FolderPath"];
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Service Execution started at : " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            ThirdPartyApi apiCall = new ThirdPartyApi();
            apiCall.RegisterServices();
            apiCall.GetFutureEventRecordUpdated().Wait();
           
            sb.AppendLine("Service Registered Successfully.");
            
            Console.WriteLine("Pro bull API's Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            apiCall.GetBullsRecord().Wait();
            sb.AppendLine("Bull Records Saved successfully" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

            Console.WriteLine("Pro Rider API's Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            apiCall.GetRidersRecord().Wait();
            sb.AppendLine("Rider records saved successfully." + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

            Console.WriteLine("Pro Future Events API's Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            apiCall.GetFutureEventRecordUpdated().Wait();


            //////apiCall.GetPastEventRecord().Wait();
            //apiCall.GetFutureEventRecord().Wait();
            sb.AppendLine("future event records successfully." + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            //apiCall.GetCurrentEventRecord().Wait();
            //sb.AppendLine("current event records successfully.");
            //apiCall.VelocityLevelEvents().Wait();
            //sb.AppendLine("velocity event records successfully.");
            apiCall.DisposeServices();

            sb.AppendLine("Service Execution Completed at : " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            try
            {
                File.WriteAllText(savePath, Convert.ToString(sb));
            }
            catch (Exception wx)
            {

                
            }
        }
    }
}