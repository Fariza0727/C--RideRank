using RR.Dto;
using System;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface ICMSService : IDisposable
     {
          /// <summary>
          /// GetPageContentByPageName
          /// </summary>
          /// <param name="pageName">The Page Name</param>
          /// <returns>The CmsDto</returns>
          Task<CmsDto> GetPageContentByPageName(string pageName);
     }
}
