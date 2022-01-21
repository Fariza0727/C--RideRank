using System;
using System.Xml;

namespace RR.StaticService
{
     public static class Utilities
     {
          /// <summary>
          /// Select email template from xml path
          /// </summary>
          /// <param name="xpath"></param>
          /// <returns></returns>
          public static string GetEmailTemplateValue(string xpath, string path = "")
          {
               string strValue = "";
               try
               {
                    // AppDomain.CurrentDomain.BaseDirectory + @"Templates\Template.xml";
                    string strPath = AppDomain.CurrentDomain.BaseDirectory + @"\Templates\Template.xml"; 
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
               }
               catch (Exception ex)
               {

               }
               return strValue;
          }
     }
}
