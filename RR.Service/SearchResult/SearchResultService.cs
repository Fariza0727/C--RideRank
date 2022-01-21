using Microsoft.Extensions.Options;
using RR.AdminData;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
    public class SearchResultService : ISearchResultService
    {
        #region Constructor

        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;
        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        private readonly IRepository<FavoriteBullRiders, RankRideContext> _repoFavoriteBullRiders;
        private readonly IRepository<RR.AdminData.News, RankRideAdminContext> _repoNews;
        private readonly IBullRiderPicturesService _picturesService;
        private readonly AppSettings _appsettings;

        public SearchResultService(IRepository<Event, RankRideStaticContext> repoEvent,
                                   IRepository<Bull, RankRideStaticContext> repoBull,
                                   IRepository<Rider, RankRideStaticContext> repoRider,
                                   IRepository<FavoriteBullRiders, RankRideContext> repoFavoriteBullRiders,
                                   IRepository<RR.AdminData.News, RankRideAdminContext> repoNews,
                                   IBullRiderPicturesService picturesService,
                                   IOptions<AppSettings> appsettings)
        {
            _repoEvent = repoEvent;
            _repoBull = repoBull;
            _repoRider = repoRider;
            _repoFavoriteBullRiders = repoFavoriteBullRiders;
            _repoNews = repoNews;
            _picturesService = picturesService;
            _appsettings = appsettings.Value;
        }

        #endregion

        /// <summary>
        /// Get Search Results
        /// </summary>
        /// <param name="keyword">The Keyword used for searching</param>
        /// <returns>The SearchDto Contains List Of All Events,Riders,Bulls,News</returns>
        public async Task<SearchDto> GetSearchResults(string keyword, int page)
        {
            int count = 0;
            keyword = keyword.Trim().ToUpper();
            var eventResultList = new Tuple<IEnumerable<Event>, int>(_repoEvent.Query()
                              .Filter(x => x.Title.Contains(keyword) ||
                              x.Location.Trim().ToUpper().Contains(keyword) ||
                              x.State.Trim().ToUpper().Contains(keyword) ||
                              x.City.Trim().ToUpper().Contains(keyword))
                              .GetPage(page, 5, out count), count);

            var riderResultList = new Tuple<IEnumerable<Rider>, int>(_repoRider.Query()
                              .Filter(x => x.Hand.Trim().ToUpper().Contains(keyword) || x.Name.Trim().ToUpper().Contains(keyword))
                              .GetPage(page, 5, out count), count);

            var bullResultList = new Tuple<IEnumerable<Bull>, int>(_repoBull.Query()
                                .Filter(x => x.Name.Trim().ToUpper().Contains(keyword) || x.Owner.Trim().ToUpper().Contains(keyword))
                                .GetPage(page, 5, out count), count);

            var newsResultList = new Tuple<IEnumerable<RR.AdminData.News>, int>(_repoNews.Query()
                                .Filter(x => x.Title.Trim().ToUpper().Contains(keyword) || x.NewsContent.Trim().ToUpper().Contains(keyword)
                                || x.NewsTag.Contains(keyword))
                                .GetPage(page, 5, out count), count);

            return await Task.FromResult(new SearchDto
            {
                EventsList = EventMapper.Map<EventDto>(eventResultList.Item1),
                BullsList = BullMapper.Map(bullResultList.Item1),
                RidersList = RiderMapper.Map(riderResultList.Item1),
                NewsList = NewsMapper.Map(newsResultList.Item1),
                PageCount = eventResultList.Item2 + riderResultList.Item2 + bullResultList.Item2 + newsResultList.Item2
            });
        }

        /// <summary>
        /// Get Riders
        /// </summary>
        /// <param name="keyword">The Keyword used for searching</param>
        /// <returns>The SearchDto Contains List Of All Riders</returns>
        public async Task<SearchDto> GetRiders(string keyword, string userId)
        {
            var riderResultList = _repoRider.Query()
                                 .Filter(x => x.Hand.Contains(keyword) || x.Name.Contains(keyword))
                                 .Get().Select(r=> 
                                 {
                                     var dto_ = RiderMapper.Map(r);
                                     dto_.Avatar = _picturesService.GetRiderPic(r.RiderId, _appsettings.MainSiteURL).Result;
                                     dto_.IsAddedFavorite = (_repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId && x.RiderId == r.RiderId).Get().SingleOrDefault() != null);
                                     return dto_;
                                 });


            return await Task.FromResult(new SearchDto
            {
                RidersList = riderResultList
            });
        }

        /// <summary>
        /// Get Bulls
        /// </summary>
        /// <param name="keyword">The Keyword used for searching</param>
        /// <returns>The SearchDto Contains List Of All Bulls</returns>
        public async Task<SearchDto> GetBulls(string keyword, string userId)
        {
            var bullResultList = _repoBull.Query()
                                .Filter(x => x.Name.Contains(keyword) || x.Owner.Contains(keyword))
                                .Get().Select(r =>
                                {
                                    var dto_ = BullMapper.Map(r);
                                    dto_.Avatar = _picturesService.GetBullPic(r.BullId, _appsettings.MainSiteURL).Result;
                                    dto_.IsAddedFavorite = (_repoFavoriteBullRiders.Query().Filter(x => x.UserId == userId && x.BullId == r.BullId).Get().SingleOrDefault() != null);
                                    return dto_;
                                }); ;


            return await Task.FromResult(new SearchDto
            {
                BullsList = bullResultList
            });
        }

        private string getPicPath(string pic)
        {
            if (!string.IsNullOrEmpty(pic))
            {
                return string.Concat(_appsettings.MainSiteURL, pic.TrimStart('/'));
            }
            return "";
        }

        /// <summary>
        /// Dispose All Service
        /// </summary>
        public void Dispose()
        {
            if (_repoBull != null)
            {
                _repoBull.Dispose();
            }
            if (_repoEvent != null)
            {
                _repoEvent.Dispose();
            }
            if (_repoRider != null)
            {
                _repoRider.Dispose();
            }
        }
    }
}
