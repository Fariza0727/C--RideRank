using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface IContestCategoryService : IDisposable
     {
          /// <summary>
          /// Get Contest Categories
          /// </summary>
          /// <returns></returns>
          Task<IEnumerable<DropDownDto>> GetContestCategories();
     }
}
