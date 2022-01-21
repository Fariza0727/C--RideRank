using Microsoft.EntityFrameworkCore;
using RR.Core;
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
    public class EventService : IEventService
    {
        #region Constructor

        private readonly IRepository<EventDraw, RankRideStaticContext> _repoDraw;
        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IRepository<CalcuttaEvent, RankRideStaticContext> _repoCalcuttaEvent;
        private readonly IRepository<CalcuttaRC, RankRideStaticContext> _repoCalcuttaRC;

        public EventService(IRepository<EventDraw, RankRideStaticContext> repoDraw,
             IRepository<Event, RankRideStaticContext> repoEvent,
             IRepository<CalcuttaRC, RankRideStaticContext> repoCalcuttaRC,
             IRepository<CalcuttaEvent, RankRideStaticContext> repoCalcuttaEvent)
        {
            _repoDraw = repoDraw;
            _repoEvent = repoEvent;
            _repoCalcuttaRC = repoCalcuttaRC;
            _repoCalcuttaEvent = repoCalcuttaEvent;
        }

        #endregion

        /// <summary>
        /// Get EventDraw By Id
        /// </summary>
        /// <param name="id">An EventDraw Id</param>
        /// <returns>List of EventDrawDto</returns>
        public async Task<IEnumerable<EventDrawDto>> GetEventDrawById(int id)
        {

            var eventDraws = _repoDraw.Query()
                 .Filter(x => x.EventId == id)
                 .Includes(e => e.Include(ev => ev.Event).ThenInclude(er => er.EventRider).ThenInclude(r => r.Rider))
                 .Get();
            return await Task.FromResult(EventMapper.Map(eventDraws));
        }

        public async Task<Tuple<IEnumerable<EventDto>, IEnumerable<EventDto>, IEnumerable<EventDto>>> GetAllEvents()
        {
            var events = _repoEvent
             .Query();

            var upcomintEvents = events
                 .Filter(x => x.PerfTime > DateTime.Now && x.IsActive == true && x.IsDelete == false)
                 .Get().OrderBy(x => x.PerfTime);
            var currentEvents = events
                 .Filter(x => x.PerfTime == DateTime.Now && x.IsActive == true && x.IsDelete == false)
                 .Get().OrderBy(x => x.PerfTime);
            // var completedEvents = events
            //     .Filter(x => x.PerfTime < DateTime.Now && x.PerfTime > DateTime.Now.AddMonths(-1)
            //   && x.IsActive == true && x.IsDelete == false)
            // .Get().OrderBy(x => x.PerfTime);
            var completedEvents = events
                 .Filter(x => x.PerfTime < DateTime.Now && x.IsActive == true && x.IsDelete == false)
                 .Get().OrderBy(x => x.PerfTime);
            return await Task.FromResult(new Tuple<IEnumerable<EventDto>, IEnumerable<EventDto>, IEnumerable<EventDto>>(EventMapper.Map<EventDto>(upcomintEvents), EventMapper.Map<EventDto>(currentEvents), EventMapper.Map<EventDto>(completedEvents)));
        }

        /// <summary>
        /// Get Latest Upcoming Event
        /// </summary>
        /// <returns>EventDto</returns>
        public async Task<EventDto> GetUpcomingEvent()
        {
            var predicate = PredicateBuilder.True<Event>()
         .And(x => x.IsDelete == false && x.IsActive == true && x.PerfTimeUTC > DateTime.UtcNow);
            var upcomingEvent = _repoEvent
                .Query()
                .Filter(predicate)
                .Get()
                .OrderBy(x => x.PerfTime).FirstOrDefault();

            return await Task.FromResult(EventMapper.Map(upcomingEvent != null ? upcomingEvent : new Event()));

        }

        /// <summary>
        /// Get Upcoming Events
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<EventDto>> GetUpcomingEvents(int count=0)
        {
            var predicate = PredicateBuilder.True<Event>()
          .And(x => x.IsDelete == false && x.IsActive == true && ((x.PerfTimeUTC != null && x.PerfTimeUTC > DateTime.UtcNow) || (x.PerfTimeUTC != null && x.PerfTime > DateTime.UtcNow)));

            var events = _repoEvent
                .Query()
                .Filter(predicate)
                .Get()
                .OrderBy(x => x.PerfTime).Select(events => new EventDto
                {
                    City = events.City,
                    EventId = events.EventId,
                    Id = events.Id,
                    IsActive = events.IsActive,
                    Location = events.Location,
                    PerfTime = events.PerfTime,
                    PerfTimeUTC = events.PerfTimeUTC ?? events.PerfTime,
                    Sanction = events.Sanction,
                    Type = events.Type,
                    //Saves  = events.Saves,
                    Season = events.Season,
                    StartDate = events.StartDate,
                    State = events.State,
                    PerfTimeUTCString = events.PerfTimeUTC == null ? events.PerfTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") : events.PerfTimeUTC?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    Title = events.Title,
                    is_current = _repoDraw.Query().Filter(x => x.EventId == events.Id).Get().Count() > 0 ? 1: 0,
                    EventMode = 0, // pbr
                }).ToList();
            var calcuttaEvents = _repoCalcuttaEvent.Query()
                .Filter(x => x.ContestStatus != "results")
                .Get()
                .OrderBy(x => x.StartDate).Select(ev => new EventDto
                {
                    City = ev.City,
                    EventId = ev.ParentEventId,
                    Id = ev.Id,
                    StartDate = ev.StartDate,
                    State = ev.State,
                    Title = ev.Title,
                    PerfTimeUTCString = ev.ContestUTCLockTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    PerfTime = ev.ContestUTCLockTime,
                    Type = ev.ContestType,
                    is_current = ev.ContestStatus == "open" ? 1 : 0,
                    EventMode = 1, // calcuatta
                }).ToList();

            var simpleTeamEvents = _repoCalcuttaEvent.Query()
                .Filter(x => x.ContestStatus != "results")
                .Get()
                .OrderBy(x => x.StartDate).Select(ev => new EventDto
                {
                    City = ev.City,
                    EventId = ev.ParentEventId,
                    Id = ev.Id,
                    StartDate = ev.StartDate,
                    State = ev.State,
                    Title = ev.Title,
                    PerfTimeUTCString = ev.ContestUTCLockTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    PerfTime = ev.ContestUTCLockTime,
                    Type = ev.ContestType,
                    is_current = ev.ContestStatus == "open" ? 1 : 0,
                    EventMode = 3, // simple team pick game
                }).ToList();

            var calcuttaRCEvents = _repoCalcuttaRC.Query()
                .Filter(x => x.ContestStatus != "results")
                .Get()
                .OrderBy(x => x.StartDate).Select(ev => new EventDto
                {
                    City = ev.City,
                    EventId = ev.ParentEventId,
                    Id = ev.Id,
                    StartDate = ev.StartDate,
                    State = ev.State,
                    Title = ev.Title,
                    PerfTimeUTCString = ev.ContestUTCLockTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    PerfTime = ev.ContestUTCLockTime,
                    Type = ev.ContestType,
                    is_current = ev.ContestStatus == "open" ? 1 : 0,
                    EventMode = 2, // calcuatta ridercomp
                }).ToList();

            events.AddRange(calcuttaEvents);
            events.AddRange(simpleTeamEvents);
            events.AddRange(calcuttaRCEvents);

            var tmpevents = count == 0 ? events.OrderBy(x => x.StartDate).ToList(): events.OrderBy(x => x.StartDate).ToList().Take(count);

            return await Task.FromResult(tmpevents);
        }

        public async Task<EventDto> GetEventById(int eventId)
        {
            var eventDetail = _repoEvent.Query()
                 .Filter(x => x.Id == eventId)
                 .Get()
                 .SingleOrDefault();

            return await Task.FromResult(EventMapper.Map(eventDetail));
        }

        /// <summary>
        /// Dispose All Services
        /// </summary>
        public void Dispose()
        {
            if (_repoDraw != null)
            {
                _repoDraw.Dispose();
            }
            if (_repoCalcuttaRC != null)
            {
                _repoCalcuttaRC.Dispose();
            }
            if (_repoCalcuttaEvent != null)
            {
                _repoCalcuttaEvent.Dispose();
            }
        }

        /// <summary>
        /// Get Completed Events
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<EventDto>> GetCompletedEvents()
        {
            var completedEvents = _repoEvent.Query()
                 .Filter(x => x.PerfTime < DateTime.Now && x.PerfTime > DateTime.Now.AddMonths(-1)
                 && x.IsActive == true && x.IsDelete == false)
                 .Get().OrderBy(x => x.PerfTime);

            return Task.FromResult(EventMapper.Map<EventDto>(completedEvents));

        }

        public async Task<Tuple<IEnumerable<EventDto>, int>> GetAllCompletedEvents(int start, int length)
        {
            int count = 0;
            var compEvents = _repoEvent.Query().Filter(x => (((x.PerfTimeUTC != null && x.PerfTimeUTC < DateTime.UtcNow) || (x.PerfTimeUTC == null && x.PerfTime < DateTime.UtcNow))) && x.IsActive == true && x.IsDelete == false && !string.IsNullOrEmpty(x.EventResult)).OrderBy(x => x.OrderByDescending(xx => xx.PerfTime))
                 .Get().Select(y => new EventDto
                 {
                     City = y.City,
                     EventId = y.EventId,
                     Id = y.Id,
                     IsActive = y.IsActive,
                     Location = y.Location,
                     PerfTime = y.PerfTime,
                     PerfTimeUTC = y.PerfTimeUTC ?? y.PerfTime,
                     Sanction = y.Sanction,
                     Type = y.Type,
                     Season = y.Season,
                     StartDate = y.StartDate,
                     State = y.State,
                     PerfTimeUTCString = y.PerfTimeUTC == null ? y.PerfTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") : y.PerfTimeUTC?.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                     Title = y.Title,
                     EventMode = 0, // calcuatta
                 }).ToList();
            var calcuttaEvents = _repoCalcuttaEvent.Query()
                .Filter(x => x.ContestStatus == "results")
                .Get()
                .OrderBy(x => x.StartDate).Select(ev => new EventDto
                {
                    City = ev.City,
                    EventId = ev.ParentEventId,
                    Id = ev.Id,
                    StartDate = ev.StartDate,
                    State = ev.State,
                    Title = ev.Title,
                    PerfTimeUTCString = ev.ContestUTCLockTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    PerfTime = ev.ContestUTCLockTime,
                    Type = ev.ContestType,
                    EventMode = 1, // calcuatta
                }).ToList();
            var simpleTeamEvents = _repoCalcuttaEvent.Query()
                .Filter(x => x.ContestStatus == "results")
                .Get()
                .OrderBy(x => x.StartDate).Select(ev => new EventDto
                {
                    City = ev.City,
                    EventId = ev.ParentEventId,
                    Id = ev.Id,
                    StartDate = ev.StartDate,
                    State = ev.State,
                    Title = ev.Title,
                    PerfTimeUTCString = ev.ContestUTCLockTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    PerfTime = ev.ContestUTCLockTime,
                    Type = ev.ContestType,
                    EventMode = 3, // simple team pick
                }).ToList();
            var calcuttaRCEvents = _repoCalcuttaRC.Query()
                .Filter(x => x.ContestStatus == "results")
                .Get()
                .OrderBy(x => x.StartDate).Select(ev => new EventDto
                {
                    City = ev.City,
                    EventId = ev.ParentEventId,
                    Id = ev.Id,
                    StartDate = ev.StartDate,
                    State = ev.State,
                    Title = ev.Title,
                    PerfTimeUTCString = ev.ContestUTCLockTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    PerfTime = ev.ContestUTCLockTime,
                    Type = ev.ContestType,
                    EventMode = 2, // calcuatta ridercomp
                }).ToList();
            compEvents.AddRange(calcuttaEvents);
            compEvents.AddRange(simpleTeamEvents);
            compEvents.AddRange(calcuttaRCEvents);
            count = compEvents.Count();
            return await Task.FromResult(new Tuple<IEnumerable<EventDto>, int>(compEvents.OrderByDescending(xx => xx.PerfTime).ToList(), count));
        }
    }
}
