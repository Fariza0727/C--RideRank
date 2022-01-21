using RR.AdminData;
using RR.AdminMapper;
using RR.Core;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public class NewsService : INewsService
     {
          #region Constructor 

          private readonly IRepository<News, RankRideAdminContext> _repoNews;

          public NewsService(IRepository<News, RankRideAdminContext> repoNews)
          {
               _repoNews = repoNews;
          }

          #endregion

          /// <summary>
          /// Add Update News Detail
          /// </summary>
          /// <param name="newsDto">The NewsDto</param>
          /// <returns></returns>
          public async Task AddEditNews(NewsDto newsDto,string userId)
          {
               try
               {
                    News newsEntity = new News();
                    if (newsDto.Id > 0)
                    {
                         newsEntity = _repoNews.Query().Filter(x => x.Id == newsDto.Id).Get().FirstOrDefault();
                         newsEntity.UpdatedBy = userId;
                         newsEntity.UpdatedDate = DateTime.Now;
                    }
                    else
                    {
                         newsEntity.CreatedDate = DateTime.Now;
                         newsEntity.CreatedBy = userId;
                         newsEntity.UpdatedBy = userId;
                         newsEntity.UpdatedDate = DateTime.Now;
                    }

                    newsEntity.Title = newsDto.Title;
                    newsEntity.PicPath = newsDto.PicPath;
                    newsEntity.VideoPath = newsDto.VideoPath;
                    newsEntity.VideoUrl = newsDto.VideoUrl;
                    newsEntity.UpdatedDate = DateTime.Now;
                    newsEntity.NewsDate = newsDto.NewsDate;
                    newsEntity.NewsTag = newsDto.NewsTag;

                    newsEntity.NewsContent = newsDto.NewsContent ?? string.Empty;

                    if (newsDto.Id > 0)
                    {
                         await _repoNews.UpdateAsync(newsEntity);
                    }
                    else
                    {
                         await _repoNews.InsertGraphAsync(newsEntity);
                    }
               }
               catch (Exception)
               {

               }
          }

          /// <summary>
          /// Get All News
          /// </summary>
          /// <param name="start">Page Number</param>
          /// <param name="length">Number Of Record </param>
          /// <param name="searchStr"></param>
          /// <param name="sort"></param>
          /// <returns></returns>
          public async Task<Tuple<IEnumerable<NewsLiteDto>, int>> GetAllNewsRecords(int start, int length, int column, string searchStr = "", string sort = "")
          {
               int count = 0;
               var predicate = PredicateBuilder.True<News>()
              .And(x => (searchStr == "" || x.Title.ToLower().Contains(searchStr.ToLower())
              || x.Id.ToString().Contains(searchStr.ToLower())));

               var news = _repoNews
                   .Query()
                   .Filter(predicate);

               if (FilterSortingVariable.NEWS_TITLE == column)
               {
                    news = (sort == "desc" ? news.OrderBy(x => x.OrderByDescending(xx => xx.Title)) : news.OrderBy(x => x.OrderBy(xx => xx.Title)));
               }
               if (FilterSortingVariable.NEWS_ID == column)
               {
                    news = (sort == "desc" ? news.OrderBy(x => x.OrderByDescending(xx => xx.Id)) : news.OrderBy(x => x.OrderBy(xx => xx.Id)));
               }

               return await Task.FromResult(new Tuple<IEnumerable<NewsLiteDto>, int>(news
                       .GetPage(start, length, out count).Select(y => new NewsLiteDto
                       {
                            Id = y.Id,
                            Title = y.Title
                       }), count));
          }

          /// <summary>
          /// Get News By Id
          /// </summary>
          /// <param name="Id">News Id</param>
          /// <returns>The NewsDto</returns>
          public async Task<NewsDto> GetNewsById(int Id)
          {
               var newsData = _repoNews.Query().Filter(x => x.Id == Id).Get().FirstOrDefault();

               return await Task.FromResult(NewsMapper.MapDto(newsData));
          }

          /// <summary>
          /// Delete News By Id
          /// </summary>
          /// <param name="Id">News Id</param>
          /// <returns></returns>
          public void DeleteNews(int Id)
          {
               _repoNews.Delete(Id);
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
