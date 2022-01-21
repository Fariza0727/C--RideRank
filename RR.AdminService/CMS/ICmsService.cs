using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface ICmsService : IDisposable
     {
          /// <summary>
          /// Add Edit Cms
          /// </summary>
          /// <param name="cmsDto">The CmsDto</param>
          /// <returns></returns>
          Task AddEditCms(CmsDto cmsDto, string userId);

          /// <summary>
          /// Get All Cms
          /// </summary>
          /// <param name="start">page Number</param>
          /// <param name="length">Number of Records On Page</param>
          /// <param name="searchStr">Search String</param>
          /// <param name="sort">Order</param>
          /// <returns>List of Cms Record</returns>
          Task<Tuple<IEnumerable<CmsLiteDto>, int>> GetAllCms(int start, int length, int column, string searchStr = "", string sort = "");

          /// <summary>
          /// Get Cms By Id
          /// </summary>
          /// <param name="id">Cms Id</param>
          /// <returns>The CmsDto</returns>
          Task<CmsDto> GetCmsById(int Id);

          /// <summary>
          /// Delete Cms By Id
          /// </summary>
          /// <param name="id">Cms Id</param>
          Task DeleteCmsById(int Id);

          /// <summary>
          /// Change Existence Of Cms Record name
          /// </summary>
          /// <param name="PageName">Page Name</param>
          /// <returns>True or False</returns>
          bool IsExistCmsRecordName(string PageName);

     }
}
