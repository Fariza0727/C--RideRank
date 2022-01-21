using RR.AdminData;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
     public static class NewsMapper
     {
          /// <summary>
          /// Map News to NewsDto
          /// </summary>
          /// <param name="news">List Of News</param>
          /// <returns>List Of NewsDto</returns>
          public static IEnumerable<NewsDto> Map(IEnumerable<News> news)
          {
               return news.Select(p => MapDto(p));
          }

          /// <summary>
          /// Map Dto
          /// </summary>
          /// <param name="newsDto">The News</param>
          /// <returns>The NewsDto</returns>
          public static NewsDto MapDto(News news)
          {
               return new NewsDto
               {
                    Id = news.Id,
                    PicPath = news.PicPath,
                    NewsContent = news.NewsContent,
                    NewsDate = news.NewsDate,
                    NewsTag = news.NewsTag,
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
