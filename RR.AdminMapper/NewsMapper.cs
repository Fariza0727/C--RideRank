using RR.AdminData;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
     public static class NewsMapper
     {
          /// <summary>
          /// Map News To NewsDto
          /// </summary>
          /// <param name="news">List Of News</param>
          /// <returns>List of Newsdto</returns>
          public static IEnumerable<NewsDto> Map(IEnumerable<News> news)
          {
               return news.Select(p => MapDto(p));
          }

          /// <summary>
          /// MapDto
          /// </summary>
          /// <param name="news">The News</param>
          /// <returns>The NewsDto</returns>
          public static NewsDto MapDto(News news)
          {
               return new NewsDto
               {
                    Id = news.Id,
                    NewsContent = news.NewsContent,
                    NewsDate = news.NewsDate,
                    NewsTag = news.NewsTag,
                    PicPath = news.PicPath,
                    Title = news.Title,
                    CreatedBy = news.CreatedBy,
                    CreatedDate = news.CreatedDate,
                    UpdatedDate = news.UpdatedDate,
                    VideoPath = news.VideoPath,
                    VideoUrl = news.VideoUrl
               };
          }
     }
}
