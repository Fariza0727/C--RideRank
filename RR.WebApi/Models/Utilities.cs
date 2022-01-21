using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace RR.WebApi.Models
{
     public class Utilities
     {
          /// <summary>
          /// Select email template from xml path
          /// </summary>
          /// <param name="xpath"></param>
          /// <returns></returns>
          public static string GetEmailTemplateValue(string xpath, string path = "")
          {
               string strValue = "";
               // AppDomain.CurrentDomain.BaseDirectory + @"Templates\Template.xml";
               string strPath = AppDomain.CurrentDomain.BaseDirectory + @"Templates\Template.xml";
               strPath = strPath.Replace(@"bin\Debug\netcoreapp2.2\", "");
               XmlDocument doc = new XmlDocument();
               doc.Load(!string.IsNullOrEmpty(path) ? path : strPath);
               XmlElement root = doc.DocumentElement;
               XmlNode node = doc.DocumentElement.SelectSingleNode(xpath);
               XmlNode childNode = node.ChildNodes[0];
               if (childNode is XmlCDataSection)
               {
                    XmlCDataSection cdataSection = childNode as XmlCDataSection;
                    strValue = cdataSection.Value.ToString();
               }
               else
               {
                    strValue = childNode.Value.ToString();
               }

               return strValue;
          }
     }
}
