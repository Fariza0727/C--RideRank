using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public class NewsLetterService : INewsLetterService
     {
          #region Constructor 

          private readonly IRepository<NewsLetterSubscribe, RankRideContext> _repoNewsLetter;

          public NewsLetterService(IRepository<NewsLetterSubscribe, RankRideContext> repoNewsLetter)
          {
               _repoNewsLetter = repoNewsLetter;
          }

          #endregion

          /// <summary>
          /// Get All News
          /// </summary>
          /// <param name="start">Page Number</param>
          /// <param name="length">Number Of Record </param>
          /// <param name="searchStr"></param>
          /// <param name="sort"></param>
          /// <returns></returns>
          public async Task<Tuple<IEnumerable<SubscribeDto>, int>> GetAllNewsLetterSubscribers(int start, int length, int column, string searchStr = "", string sort = "")
          {
               int count = 0;
               var predicate = PredicateBuilder.True<NewsLetterSubscribe>()
              .And(x => (searchStr == "" || x.Email.ToLower().Contains(searchStr.ToLower())
              || x.Id.ToString().Contains(searchStr.ToLower())));

               var newsLetter = _repoNewsLetter
                   .Query()
                   .Filter(predicate);

               if (FilterSortingVariable.NEWSLETTER_ID == column)
               {
                    newsLetter = (sort == "desc" ? newsLetter.OrderBy(x => x.OrderByDescending(xx => xx.Id)) : newsLetter.OrderBy(x => x.OrderBy(xx => xx.Id)));
               }
               if (FilterSortingVariable.NEWSLETTER_EMAIL == column)
               {
                    newsLetter = (sort == "desc" ? newsLetter.OrderBy(x => x.OrderByDescending(xx => xx.Email)) : newsLetter.OrderBy(x => x.OrderBy(xx => xx.Email)));
               }
               if (FilterSortingVariable.NEWSLETTER_SUBSCRIBERCREATED == column)
               {
                    newsLetter = (sort == "desc" ? newsLetter.OrderBy(x => x.OrderByDescending(xx => xx.CreatedOn)) : newsLetter.OrderBy(x => x.OrderBy(xx => xx.CreatedOn)));
               }

               return await Task.FromResult(new Tuple<IEnumerable<SubscribeDto>, int>(newsLetter
                       .GetPage(start, length, out count).Select(y => new SubscribeDto
                       {
                            Id = y.Id,
                            Email = y.Email,
                            CreatedOn = DateTime.Now.Date != y.CreatedOn.Date ?
                            string.Concat((DateTime.Now.Date - y.CreatedOn.Date).TotalDays, " day ago") :
                           Convert.ToInt32((DateTime.Now - y.CreatedOn).TotalMinutes) < 90 ?
                           string.Concat(Convert.ToInt32((DateTime.Now - y.CreatedOn).TotalMinutes), " minutes ago") :
                           string.Concat(Convert.ToInt32((DateTime.Now - y.CreatedOn).TotalHours), " hours ago")
                       }), count));
          }

          /// <summary>
          /// Delete News By Id
          /// </summary>
          /// <param name="Id">News Id</param>
          /// <returns></returns>
          public void DeleteNewsLetter(int newsletterId)
          {
               _repoNewsLetter.Delete(newsletterId);
          }

          /// <summary>
          /// Dispose News Service
          /// </summary>
          public void Dispose()
          {
               if (_repoNewsLetter != null)
               {
                    _repoNewsLetter.Dispose();
               }
          }
     }
}
