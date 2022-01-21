using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface INewsService
    {
        /// <summary>
        /// Get News
        /// </summary>
        /// <param name="count"></param>
        /// <returns>List Of NewsDto</returns>
        Task<IEnumerable<NewsDto>> GetNews(int count = 0);

        /// <summary>
        /// Get News Detail
        /// </summary>
        /// <param name="title">The Title</param>
        /// <param name="id">An Id</param>
        /// <returns>The NewsDto</returns>
        Task<NewsDto> GetNewsDetail(string title, int id);

        /// <summary>
        /// Get Top Recent News
        /// </summary>
        /// <returns>Recent News & Top Stories In Multiple List Of NewsDto</returns>
        Task<Tuple<IEnumerable<NewsDto>, IEnumerable<NewsDto>>> GetTopRecentNews();

        /// <summary>
        /// Get HomePage News
        /// </summary>
        /// <returns>News,Top News & Popular News In Multiple List Of NewsDto</returns>
        Task<Tuple<IEnumerable<NewsDto>, IEnumerable<NewsDto>, IEnumerable<NewsDto>>> GetHomePageNews();

        /// <summary>
        /// Get list of news for slider
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<NewsDto>> GetSliderNews();
    }
}
