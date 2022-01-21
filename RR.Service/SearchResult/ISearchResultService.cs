using RR.Dto;
using System;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface ISearchResultService : IDisposable
     {
          /// <summary>
          /// Get Search Results
          /// </summary>
          /// <param name="keyword">The Keyword used for searching</param>
          /// <returns>The SearchDto Contains List Of All Events,Riders,Bulls,News</returns>
          Task<SearchDto> GetSearchResults(string keyword, int page);

        /// <summary>
        /// Get Riders
        /// </summary>
        /// <param name="keyword">The Keyword used for searching</param>
        /// <param name="userId">userId</param>
        /// <returns>The SearchDto Contains List Of All Events,Riders,Bulls,News</returns>
        Task<SearchDto> GetRiders(string keyword, string userId = "");

        /// <summary>
        /// Get Bulls
        /// </summary>
        /// <param name="keyword">The Keyword used for searching</param>
        /// <param name="userId">userId</param>
        /// <returns>The SearchDto Contains List Of All Bulls</returns>
        Task<SearchDto> GetBulls(string keyword, string userId = "");
     }
}
