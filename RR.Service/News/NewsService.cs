using RR.AdminData;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service.News
{
    public class NewsService : INewsService
    {
        #region Constructor

        private readonly IRepository<RR.AdminData.News, RankRideAdminContext> _repoNews;

        public NewsService(IRepository<RR.AdminData.News, RankRideAdminContext> repoNews)
        {
            _repoNews = repoNews;
        }

        #endregion

        /// <summary>
        /// Get News
        /// </summary>
        /// <returns>List Of NewsDto</returns>
        public async Task<IEnumerable<NewsDto>> GetNews(int count = 0)
        {
            DateTime dateTime = DateTime.Now.Date;
            var news = _repoNews
                .Query()
                .Filter(x => x.NewsDate.Date == dateTime)
                .Get();

            if (count > 0)
                news = news.Take(count);

            if (news == null || news.Count() == 0)
            {
                news = _repoNews
                    .Query()
                    .Get();
            }
            return await Task.FromResult(NewsMapper.Map(news));
        }

        /// <summary>
        /// Get News Detail
        /// </summary>
        /// <param name="title">The Title</param>
        /// <param name="id">An Id</param>
        /// <returns>The NewsDto</returns>
        public async Task<NewsDto> GetNewsDetail(string title, int id)
        {
            var news = _repoNews.Query().Filter(x => x.Id == id && x.Title.ToLower() == title).Get().FirstOrDefault();
            return await Task.FromResult(NewsMapper.MapDto(news));
        }

        /// <summary>
        /// Get News Detail
        /// </summary>
        /// <param name="title">The Title</param>
        /// <param name="id">An Id</param>
        /// <returns>The NewsDto</returns>
        public async Task<Tuple<IEnumerable<NewsDto>, IEnumerable<NewsDto>>> GetTopRecentNews()
        {
            IEnumerable<NewsDto> recentNews, topStories;
            DateTime dateTime = DateTime.Now;
            recentNews = NewsMapper.Map(_repoNews.Query().Filter(x => x.NewsDate.Date < dateTime).Get().OrderByDescending(x => x.NewsDate).Take(4));
            topStories = NewsMapper.Map(_repoNews.Query().Filter(x => x.NewsDate <= dateTime).Get().OrderByDescending(x => Guid.NewGuid()).Take(4));
            return await Task.FromResult(new Tuple<IEnumerable<NewsDto>, IEnumerable<NewsDto>>(recentNews, topStories));
        }

        /// <summary>
        /// Get News Detail
        /// </summary>
        /// <param name="title">The Title</param>
        /// <param name="id">An Id</param>
        /// <returns>The NewsDto</returns>
        public async Task<Tuple<IEnumerable<NewsDto>, IEnumerable<NewsDto>, IEnumerable<NewsDto>>> GetHomePageNews()
        {
            IEnumerable<NewsDto> news, popularNews, topNews;
            DateTime dateTime = DateTime.Now.Date;
            var newsList = _repoNews.Query().Get();
            news = NewsMapper.Map(newsList.OrderByDescending(x => x.NewsDate).Take(6));
            topNews = NewsMapper.Map(newsList.Where(x => x.NewsDate <= dateTime).OrderByDescending(x => Guid.NewGuid()).Take(4));
            popularNews = NewsMapper.Map(newsList.Where(x => x.IsPopular).OrderByDescending(x => x.NewsDate).Take(5));
            return await Task.FromResult(new Tuple<IEnumerable<NewsDto>, IEnumerable<NewsDto>, IEnumerable<NewsDto>>(news, popularNews, topNews));
        }

        /// <summary>
        /// Get home page slider news
        /// </summary>
        /// <returns>The NewsDto</returns>
        public async Task<IEnumerable<NewsDto>> GetSliderNews()
        {
            var newsList = _repoNews.Query().OrderBy(x => x.OrderByDescending(xx => xx.CreatedDate)).Get().Take(4);
            return await Task.FromResult(NewsMapper.Map(newsList));
        }

        /// <summary>
        /// Dispose News Service
        /// </summary>
        public void Dispose()
        {
            if (_repoNews != null)
            {
                _repoNews.Dispose();
            }
        }
    }
}
