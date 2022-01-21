using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface INewsService : IDisposable
     {
          /// <summary>
          /// Add Update News Detail
          /// </summary>
          /// <param name="newsDto">The NewsDto</param>
          /// <returns></returns>
          Task AddEditNews(NewsDto newsDto, string userId);

          /// <summary>
          /// Get All News
          /// </summary>
          /// <param name="start">Page Number</param>
          /// <param name="length">Number Of Record </param>
          /// <param name="searchStr"></param>
          /// <param name="sort"></param>
          /// <returns></returns>
          Task<Tuple<IEnumerable<NewsLiteDto>, int>> GetAllNewsRecords(int start, int length, int column, string searchStr = "", string sort = "");

          /// <summary>
          /// Get News By Id
          /// </summary>
          /// <param name="Id">News Id</param>
          /// <returns>The NewsDto</returns>
          Task<NewsDto> GetNewsById(int Id);

          /// <summary>
          /// Delete News By Id
          /// </summary>
          /// <param name="Id">News Id</param>
          /// <returns></returns>
          void DeleteNews(int Id);
     }
}
