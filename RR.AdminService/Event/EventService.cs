using RR.AdminData;
using RR.AdminMapper;
using RR.Core;
using RR.Dto;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class EventService : IEventService
    {
        #region Constructor

        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IRepository<Contest, RankRideAdminContext> _repoContest;

        public EventService(
            IRepository<Event, RankRideStaticContext> repoEvent,
            IRepository<Contest, RankRideAdminContext> repoContest)
        {
            _repoEvent = repoEvent;
            _repoContest = repoContest;
        }

        #endregion

        /// <summary>
        /// Get All Events
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Records on each page</param>
        /// <param name="searchStr">Search String</param>
        /// <param name="sort">Order</param>
        /// <returns>List Of EventRecords</returns>
        public async Task<Tuple<IEnumerable<EventLiteDto>, int>> GetAllEvents(int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;

            var predicate = PredicateBuilder.True<Event>()
          .And(x => x.IsDelete == false && (searchStr == "" || x.Title.ToLower().Contains(searchStr.ToLower())
          || x.Location.Contains(searchStr.ToLower())
          || x.City.Contains(searchStr.ToLower())));

            var events = _repoEvent
                .Query()
                .Filter(predicate);

            if (FilterSortingVariable.EVENT_TITLE == column)
            {
                events = (sort == "desc" ? events.OrderBy(x => x.OrderByDescending(xx => xx.Title)) : events.OrderBy(x => x.OrderBy(xx => xx.Title)));
            }
            if (FilterSortingVariable.EVENT_LOCATION == column)
            {
                events = (sort == "desc" ? events.OrderBy(x => x.OrderByDescending(xx => xx.Location)) : events.OrderBy(x => x.OrderBy(xx => xx.Location)));
            }
            if (FilterSortingVariable.EVENT_CITY == column)
            {
                events = (sort == "desc" ? events.OrderBy(x => x.OrderByDescending(xx => xx.City)) : events.OrderBy(x => x.OrderBy(xx => xx.City)));
            }
            if (FilterSortingVariable.EVENT_TYPE == column)
            {
                events = (sort == "desc" ? events.OrderBy(x => x.OrderByDescending(xx => xx.Type)) : events.OrderBy(x => x.OrderBy(xx => xx.Type)));
            }
            if (FilterSortingVariable.EVENT_STATE == column)
            {
                events = (sort == "desc" ? events.OrderBy(x => x.OrderByDescending(xx => xx.State)) : events.OrderBy(x => x.OrderBy(xx => xx.State)));
            }
            if (FilterSortingVariable.EVENT_STARTDATE == column)
            {
                events = (sort == "desc" ? events.OrderBy(x => x.OrderByDescending(xx => xx.PerfTime)) : events.OrderBy(x => x.OrderBy(xx => xx.PerfTime)));
            }
            if (FilterSortingVariable.EVENT_PBRID == column)
            {
                events = (sort == "desc" ? events.OrderBy(x => x.OrderByDescending(xx => xx.Pbrid)) : events.OrderBy(x => x.OrderBy(xx => xx.Pbrid)));
            }

            return await Task.FromResult(new Tuple<IEnumerable<EventLiteDto>, int>(events
                    .GetPage(start, length, out count).Select(y => new EventLiteDto
                    {
                        Id = y.Id,
                        City = y.City,
                        Location = y.Location,
                        PerfTime = y.PerfTime,
                        Title = y.Title,
                        IsActive = y.IsActive,
                        State = y.State,
                        Type = y.Type,
                        PBRID = y.Pbrid
                    }), count));
        }

        /// <summary>
        /// Get Event By Id
        /// </summary>
        /// <param name="eventId">An Event Id</param>
        /// <returns>The Event detail</returns>
        public async Task<EventDto> GetEventById(int eventId)
        {
            var eventDetail = _repoEvent.Query()
               .Filter(e => e.Id == eventId)
               .Get()
               .SingleOrDefault();
            return await Task.FromResult(EventMapper.MapDto(eventDetail));
        }

        /// <summary>
        /// Update Event Detail
        /// </summary>
        /// <param name="eventDto">An Event Dto</param>
        /// <returns></returns>
        public async Task UpdateEventDetail(EventDto eventDto)
        {
            var eventData = _repoEvent.Query().Filter(x => x.Id == eventDto.Id).Get().FirstOrDefault();

            if (eventData != null)
            {
                eventData.Location = !string.IsNullOrEmpty(eventDto.Location) ? eventDto.Location : "";
                eventData.PerfTime = eventDto.PerfTime;
                eventData.Sanction = !string.IsNullOrEmpty(eventDto.Sanction) ? eventDto.Sanction : "";
                eventData.Season = eventDto.Season;
                eventData.StartDate = eventDto.StartDate;
                eventData.State = !string.IsNullOrEmpty(eventDto.State) ? eventDto.State : "";
                eventData.Title = !string.IsNullOrEmpty(eventDto.Title) ? eventDto.Title : "";
                eventData.Type = !string.IsNullOrEmpty(eventDto.Type) ? eventDto.Type : "";
                eventData.UpdatedDate = DateTime.Now;
                await _repoEvent.UpdateAsync(eventData);
            }
        }

        /// <summary>
        /// Update Event Status
        /// </summary>
        /// <param name="eventId">An Event Id</param>
        /// <returns></returns>
        public async Task UpdateStatus(int eventId)
        {
            var eventDetail = _repoEvent.Query()
                 .Filter(x => x.Id == eventId)
                .Get()
                .SingleOrDefault();
            eventDetail.IsActive = (eventDetail.IsActive == true) ? false : true;
            await _repoEvent.UpdateAsync(eventDetail);
        }

        /// <summary>
        /// Delete Event By Id
        /// </summary>
        /// <param name="eventId">An Event Id</param>
        /// <returns></returns>
        public async Task DeleteEventById(int eventId)
        {
            await _repoEvent.DeleteAsync(eventId);
        }

        /// <summary>
        /// Get Upcoming Events
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<EventDto>> GetUpcomingEvents()
        {
            var predicate = PredicateBuilder.True<Event>()
          .And(x => x.IsDelete == false && x.StartDate > DateTime.Now);

            var events = _repoEvent
                .Query()
                .Filter(predicate)
                .Get()
                .OrderBy(x => x.StartDate);

            return await Task.FromResult(EventMapper.Map(events));
        }
        
        public async Task<IEnumerable<EventDto>> GetUpcomingEventsFiltered(long eventId = 0)
        {
            var predicate = PredicateBuilder.True<Event>()
          .And(x => x.IsDelete == false && x.StartDate > DateTime.Now);

            var events = _repoEvent
                .Query()
                .Filter(predicate)
                .Get().GroupJoin(_repoContest.Query().Get(),
                e => e.Id, c => c.EventId,
                (e, c) => new { evt = e, cnt = c })
                .OrderBy(x => x.evt.StartDate).Where(r => r.cnt.Select(m=>m.Id).Contains(eventId) || r.cnt.Count() == 0);

            return await Task.FromResult(EventMapper.Map(events.Select(d => d.evt)));
        }

        /// <summary>
        /// Get All Events
        /// </summary>
        /// <returns>List Of Event Records</returns>
        public async Task<IEnumerable<EventLiteDto>> GetAllEvents()
        {
            var predicate = PredicateBuilder.True<Event>()
            .And(x => x.IsDelete == false);

            var events = _repoEvent
                .Query()
                .Filter(predicate).OrderBy(x => x.OrderBy(xx => xx.Title)).Get();


            return await Task.FromResult(events
                    .Select(y => new EventLiteDto
                    {
                        Id = y.Id,
                        City = y.City,
                        Location = y.Location,
                        PerfTime = y.PerfTime,
                        Title = y.Title,
                        IsActive = y.IsActive
                    }));
        }

        /// <summary>
        /// Dispose Event Service
        /// </summary>
        public void Dispose()
        {
            if (_repoEvent != null)
            {
                _repoEvent.Dispose();
            }
        }

        
        public async Task<Tuple<int, int>> GetEventsAsCard()
        {
            var total_ = _repoEvent.Query().Get().Count();
            var active_ = _repoEvent.Query().Filter(r => r.IsActive == true).Get().Count();
            return await Task.FromResult(new Tuple<int, int>(total_, active_));
        }
    }
}
