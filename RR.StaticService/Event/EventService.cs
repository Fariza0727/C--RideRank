using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RR.Dto;
using RR.Dto.Event;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.StaticService
{
    public class EventService : IEventService
    {
        #region Constructor

        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;
        private readonly IRepository<EventBull, RankRideStaticContext> _repoEventBull;
        private readonly IRepository<EventRider, RankRideStaticContext> _repoEventRider;
        private readonly IRepository<EventDraw, RankRideStaticContext> _repoDraw;
        private readonly IBullService _bullService;
        private readonly IRiderService _riderService;
        private readonly IEmailSender _emailSender;

        public EventService(IRepository<Event, RankRideStaticContext> repoEvent,
                          IRepository<EventBull, RankRideStaticContext> repoEventBull,
                          IRepository<EventRider, RankRideStaticContext> repoEventRider,
                          IRepository<EventDraw, RankRideStaticContext> repoDraw,
                          IRepository<Rider, RankRideStaticContext> repoRider,
                          IRepository<Bull, RankRideStaticContext> repoBull,
                          IBullService bullService,
                          IRiderService riderService,
                          IEmailSender emailSender)
        {
            _repoEvent = repoEvent;
            _repoEventBull = repoEventBull;
            _repoEventRider = repoEventRider;
            _repoDraw = repoDraw;
            _repoRider = repoRider;
            _repoBull = repoBull;
            _bullService = bullService;
            _riderService = riderService;
            _emailSender = emailSender;
        }

        #endregion

        /// <summary>
        /// Current Add Edit Events
        /// </summary>
        /// <param name="model">List of Curent EventDto</param>
        /// <returns></returns>
        public async Task CurrentAddEditEvents(List<CurrentEventDto> model)
        {
            
            Console.WriteLine("begin CurrentAddEditEvents");
            if (model != null && model.Count() > 0)
            {
                Console.WriteLine("model != null and model.Count > 0" + model.Count());
                foreach (var item in model)
                {
                    if (item.EventDto.PerfTime < DateTime.Now)
                        continue;
                    var eventDto = item.EventDto;
                    var eventRiderList = item.EventRiderDto;
                    var eventBullList = item.EventBullDto;
                    var eventDrawList = item.EventDrawDto;

                    Console.WriteLine("begin inserts for event ID" + eventDto.EventId);

                    //Insert Data of bulls for specific event into eventbull table
                    await AddEventBulls(eventBullList, eventDto);
                    Console.WriteLine("bull inserts done");

                    Console.WriteLine("begin rider inserts for event ID" + eventDto.EventId);
                    //Insert Data of riders for specific event into eventrider table
                    await AddEventRiders(eventRiderList, eventDto);

                    Console.WriteLine("begin draw inserts for event ID" + eventDto.EventId);
                    //Insert data of which rider is sanction to which bull 
                    //for particular event into event draw table
                    AddEventDraws(eventDrawList, eventDto);
                }

            }
        }

        /// <summary>
        /// Current Add Edit Events
        /// </summary>
        /// <param name="model">List of Curent EventDto</param>
        /// <returns></returns>
        public async Task CurrentAddEditEventsDropOut(List<CurrentEventDto> model)
        {
            if (model != null && model.Count() > 0)
            {
                foreach (var item in model)
                {
                    if (item.EventDto.PerfTime < DateTime.Now)
                        continue;
                    var eventDto = item.EventDto;
                    var eventRiderList = item.EventRiderDto;
                    var eventBullList = item.EventBullDto;
                    var eventDrawList = item.EventDrawDto;

                    //Insert Data of bulls for specific event into eventbull table
                    await AddEventBullsDropout(eventBullList, eventDto);

                    //Insert Data of riders for specific event into eventrider table
                    await AddEventRidersDropout(eventRiderList, eventDto);

                    //Insert data of which rider is sanction to which bull 
                    //for particular event into event draw table
                    AddEventDraws(eventDrawList, eventDto);
                }

            }
        }

        /// <summary>
        /// Add Edit Events
        /// </summary>
        /// <param name="eventDto">List Of EventDto</param>
        /// <returns></returns>
        public async Task AddEditEvents(List<EventDto> eventDto)
        {
            #region Old Code

            // var events = new List<Event>();

            // var eventDetail = _repoEvent.Query()
            //.Filter(x => eventDto.Select(y => y.EventId).Contains(x.EventId))
            //.Get();

            // var eventInsert = eventDto.Where(x => !eventDetail
            //                         .Select(y => y.EventId).Contains(x.EventId)).ToList();

            // if (eventInsert != null && eventInsert.Count > 0)
            // {
            //     events.AddRange(eventInsert.Select(x => new Event
            //     {
            //         EventId = x.EventId,
            //         Title = !string.IsNullOrEmpty(x.Title) ? x.Title : "",
            //         Location = !string.IsNullOrEmpty(x.Location) ? x.Location : "",
            //         City = x.City,
            //         State = x.State,
            //         Type = !string.IsNullOrEmpty(x.Type) ? x.Type : "",
            //         Season = x.Season,
            //         StartDate = x.StartDate,
            //         PerfTime = x.PerfTime,
            //         Sanction = !string.IsNullOrEmpty(x.Sanction) ? x.Sanction : "",
            //         CreatedDate = DateTime.Now,
            //         UpdatedDate = DateTime.Now
            //     }));

            //     _repoEvent.InsertCollection(events);

            //     string emailBody = Utilities.GetEmailTemplateValue("NewEventArrived/Body", "");
            //     string emailSubject = Utilities.GetEmailTemplateValue("NewEventArrived/Subject", "");
            //     emailBody = emailBody.Replace("@@@UserEmail", "RankRide Team");

            //     await _emailSender.SendEmailAsync(
            //         "info@rankridefantasy.com",
            //         emailSubject,
            //         emailBody);
            // }

            #endregion

            #region New Code
            int count = 0;
            foreach (var item in eventDto.Where(x => x.StartDate.Date > DateTime.Now.Date))
            {
                var eventExists = _repoEvent.Query()
             .Filter(x => x.Pbrid == item.PBRID) //x.EventId == item.EventId ||
             .Get().FirstOrDefault();

                if (eventExists != null)
                {
                    //update
                    eventExists.EventId = item.EventId;
                    eventExists.Title = !string.IsNullOrEmpty(item.Title) ? item.Title : "";
                    eventExists.Location = !string.IsNullOrEmpty(item.Location) ? item.Location : "";
                    eventExists.City = item.City;
                    eventExists.State = item.State;
                    eventExists.Type = !string.IsNullOrEmpty(item.Type) ? item.Type : "";
                    eventExists.Season = item.Season;
                    eventExists.StartDate = item.StartDate;
                    eventExists.PerfTime = item.PerfTime;
                    eventExists.Sanction = !string.IsNullOrEmpty(item.Sanction) ? item.Sanction : "";
                    eventExists.Pbrid = item.PBRID;
                    eventExists.UpdatedDate = DateTime.Now;
                    await _repoEvent.UpdateAsync(eventExists);
                }
                else
                {
                    //insert
                    eventExists = new Event();
                    eventExists.EventId = item.EventId;
                    eventExists.Title = !string.IsNullOrEmpty(item.Title) ? item.Title : "";
                    eventExists.Location = !string.IsNullOrEmpty(item.Location) ? item.Location : "";
                    eventExists.City = item.City;
                    eventExists.State = item.State;
                    eventExists.Type = !string.IsNullOrEmpty(item.Type) ? item.Type : "";
                    eventExists.Season = item.Season;
                    eventExists.StartDate = item.StartDate;
                    eventExists.PerfTime = item.PerfTime;
                    eventExists.Sanction = !string.IsNullOrEmpty(item.Sanction) ? item.Sanction : "";
                    eventExists.Pbrid = item.PBRID;
                    eventExists.IsActive = true;
                    eventExists.IsDelete = false;
                    eventExists.UpdatedDate = DateTime.Now;
                    eventExists.CreatedDate = DateTime.Now;
                    await _repoEvent.InsertAsync(eventExists);
                    count = count + 1;
                }
            }

            if (count > 0)
            {
                string emailBody = Utilities.GetEmailTemplateValue("NewEventArrived/Body", "");
                string emailSubject = Utilities.GetEmailTemplateValue("NewEventArrived/Subject", "");
                emailBody = emailBody.Replace("@@@UserEmail", "RankRide Team");

                await _emailSender.SendEmailAsync(
                    "info@rankridefantasy.com",
                    emailSubject,
                    emailBody);
            }
            #endregion
        }

        /// <summary>
        /// Add Event Riders
        /// </summary>
        /// <param name="eventRiderDto">List Of EventRiderDto</param>
        /// <returns></returns>
        public async Task AddEventRiders(List<EventRidersDto> eventRiderDto, EventDto eventDto)
        {
            var eventRider = new List<EventRider>();

            var eventDetail = _repoEvent
                         .Query()
                         .Filter(x => x.EventId.ToLower() == eventDto.EventId.ToLower())
                         .Get()
                         .SingleOrDefault();

            var eventRiders = (from rider in _repoRider.Query()
                                    .Get()
                               join eveRider in eventRiderDto on rider.RiderId equals eveRider.RiderId
                               select new { rider, eveRider }).ToList();

            if (eventRiders.Count() == eventRiderDto.Count())
            {
                //Check Event Bull already associated
                HashSet<int> bIDs = new HashSet<int>(eventRiderDto.Select(s => s.RiderId));
                var da = _repoEventRider.Query().Filter(x => x.EventId == eventDetail.Id).Get();
                var eventRiderExists = _repoEventRider.Query().Filter(x => x.EventId == eventDetail.Id).Includes(x => x.Include(y => y.Rider)).Get().ToList();
                //Insert all the bulls if there is no any record in DB
                if (eventRiderExists.Count == 0)
                {
                    eventRider.AddRange(eventRiders.Select(x => new EventRider
                    {
                        EventId = eventDetail.Id,
                        EventTierRank = x.eveRider.EventTierRank,
                        EventTier = x.eveRider.EventTier,
                        EventTierScore = x.eveRider.EventTierScore,
                        CwrpBonus = x.eveRider.CwrpBonus,
                        RiderId = x.rider.Id,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsActive = true,
                        IsDropout = false
                    }));
                    _repoEventRider.InsertCollection(eventRider);
                }
                else //Check exists bulls first, if not exists thn enter in DB
                {
                    var existIds = eventRiderExists.Select(x => x.Rider.RiderId);
                    var riderResult1 = eventRiders.Where(m => !existIds.Contains(m.rider.RiderId) && m.eveRider.EventId == eventDto.EventId).ToList();
                    if (riderResult1.Count > 0)
                    {
                        eventRider.AddRange(riderResult1.Select(x => new EventRider
                        {
                            EventId = eventDetail.Id,
                            EventTierRank = x.eveRider.EventTierRank,
                            EventTier = x.eveRider.EventTier,
                            EventTierScore = x.eveRider.EventTierScore,
                            CwrpBonus = x.eveRider.CwrpBonus,
                            RiderId = x.rider.Id,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            IsActive = true,
                            IsDropout = false

                        }));
                        _repoEventRider.InsertCollection(eventRider);
                    }
                }

            }
            else
            {
                var riderList = eventRiderDto.Where(x => !eventRiders.Select(y => y.rider.RiderId).Contains(x.RiderId)).ToList();
                foreach (var item in riderList)
                {
                    var riderResponse = Core.WebProxy.APIResponse($"rider&&id={item.RiderId}");
                    var returnRiderDetailResult = new RiderDto();
                    if (!string.IsNullOrEmpty(riderResponse.Result))
                    {
                        returnRiderDetailResult = JsonConvert.DeserializeObject<RiderDto>(riderResponse.Result);
                        if (returnRiderDetailResult != null)
                        {
                            _riderService.AddEditRiders(returnRiderDetailResult);
                        }
                    }
                }
                await AddEventRiders(eventRiderDto, eventDto);
            }
        }

        /// <summary>
        /// Add EventBulls
        /// </summary>
        /// <param name="eventBullDto">List Of EventBullDto</param>
        /// <returns></returns>
        public async Task AddEventBulls(List<EventBullsDto> eventBullDto, EventDto eventDto)
        {
            Console.WriteLine("begin AddEventBulls");
            var eventBull = new List<EventBull>();

            var eventDetail = _repoEvent
                        .Query()
                        .Filter(x => x.EventId.ToLower() == eventDto.EventId.ToLower())
                        .Get()
                        .SingleOrDefault();

            var eventBullDetail = (from bull in _repoBull.Query()
                                   .Get()
                                   join eveBull in eventBullDto on bull.BullId equals eveBull.BullId
                                   select new { bull, eveBull }).ToList();
            Console.WriteLine("Event detail and event bull detail set for" + eventBullDto.Count() + eventBullDetail.Count());
            if (eventBullDto.Count() == eventBullDetail.Count())
            {
                Console.WriteLine("eventBullDto.Count() == eventBullDetail.Count()");
                //Check Event Bull already associated
                HashSet<int> bIDs = new HashSet<int>(eventBullDto.Select(s => s.BullId));
                var eventBullExists = _repoEventBull.Query().Filter(x => x.EventId == eventDetail.Id).Includes(x => x.Include(y => y.Bull)).Get().ToList();
                //Insert all the bulls if there is no any record in DB
                if (eventBullExists.Count == 0)
                {
                    Console.WriteLine("eventBullExists.Count == 0");
                    eventBull.AddRange(eventBullDetail.Select(x => new EventBull
                    {
                        EventId = eventDetail.Id,
                        TierScore = x.eveBull.TierScore,
                        EventTier = x.eveBull.EventTier,
                        BullId = x.bull.Id,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsActive = true,
                        IsDropout = false
                    }));
                    _repoEventBull.InsertCollection(eventBull);
                    Console.WriteLine("addrange performed for this bull");
                }
                else //Check exists bulls first, if not exists thn enter in DB
                {
                    Console.WriteLine("eventBullExists.Count DOES NOT == 0");
                    var existIds = eventBullExists.Select(x => x.Bull.BullId);
                    var bullResult1 = eventBullDetail.Where(m => !existIds.Contains(m.bull.BullId)).ToList();
                    if (bullResult1.Count > 0)
                    {
                        eventBull.AddRange(bullResult1.Select(x => new EventBull
                        {
                            EventId = eventDetail.Id,
                            TierScore = x.eveBull.TierScore,
                            EventTier = x.eveBull.EventTier,
                            BullId = x.bull.Id,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            IsActive = true,
                            IsDropout = false
                        }));
                        _repoEventBull.InsertCollection(eventBull);
                        Console.WriteLine("bull insert performed");
                    }
                }
            }
            else
            {
                Console.WriteLine("eventBullDto.Count()DOES NOT == eventBullDetail.Count()");
                var bullList = eventBullDto.Where(x => !eventBullDetail.Select(y => y.bull.BullId).Contains(x.BullId)).ToList();
                Console.WriteLine("bullList fetch performed");
                foreach (var item in bullList)
                {
                    Console.WriteLine("fetching from api bull " + item.BullId);
                    var bullResponse = Core.WebProxy.APIResponse($"bull&&id={item.BullId}");
                    var returnBullDetailResult = new BullDto();
                    if (!string.IsNullOrEmpty(bullResponse.Result))
                    {
                        returnBullDetailResult = JsonConvert.DeserializeObject<BullDto>(bullResponse.Result);
                        if (returnBullDetailResult != null)
                        {
                            _bullService.AddEditBulls(returnBullDetailResult);
                        }

                    }
                }
                Console.WriteLine("calling AddEventBulls");
                await AddEventBulls(eventBullDto, eventDto);
                Console.WriteLine("AddEventBulls done");
            }
        }

       
        /// <summary>
        /// Add Event Riders Dropouts
        /// </summary>
        /// <param name="eventRiderDto">List Of EventRiderDto</param>
        /// <returns></returns>
        public async Task AddEventRidersDropout(List<EventRidersDto> eventRiderDto, EventDto eventDto)
        {
            var eventRider = new List<EventRider>();

            var eventDetail = _repoEvent
                         .Query()
                         .Filter(x => x.EventId.ToLower() == eventDto.EventId.ToLower())
                         .Get()
                         .SingleOrDefault();

            var entRider = _repoEventRider.Query().Filter(d => d.EventId == eventDetail.Id).Get();
            foreach (var item in entRider)
            {
                item.IsDropout = true;
                _repoEventRider.Update(item);
            }

            var eventRiders = (from rider in _repoRider.Query()
                                    .Get()
                               join eveRider in eventRiderDto on rider.RiderId equals eveRider.RiderId
                               select new { rider, eveRider }).ToList();

            if (eventRiders.Count() == eventRiderDto.Count())
            {
                //Check Event Bull already associated
                HashSet<int> bIDs = new HashSet<int>(eventRiderDto.Select(s => s.RiderId));
                var da = _repoEventRider.Query().Filter(x => x.EventId == eventDetail.Id).Get();
                var eventRiderExists = _repoEventRider.Query().Filter(x => x.EventId == eventDetail.Id).Includes(x => x.Include(y => y.Rider)).Get().ToList();
                //Insert all the bulls if there is no any record in DB
                if (eventRiderExists.Count == 0)
                {
                    eventRider.AddRange(eventRiders.Select(x => new EventRider
                    {
                        EventId = eventDetail.Id,
                        EventTierRank = x.eveRider.EventTierRank,
                        EventTier = x.eveRider.EventTier,
                        EventTierScore = x.eveRider.EventTierScore,
                        CwrpBonus = x.eveRider.CwrpBonus,
                        RiderId = x.rider.Id,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsActive = true,
                        IsDropout = false
                    }));
                    _repoEventRider.InsertCollection(eventRider);
                }
                else //Check exists bulls first, if not exists thn enter in DB
                {
                    var existIds = eventRiderExists.Select(x => x.Rider.RiderId);
                    var riderResult1 = eventRiders.Where(m => !existIds.Contains(m.rider.RiderId) && m.eveRider.EventId == eventDto.EventId).ToList();
                    if (riderResult1.Count > 0)
                    {
                        eventRider.AddRange(riderResult1.Select(x => new EventRider
                        {
                            EventId = eventDetail.Id,
                            EventTierRank = x.eveRider.EventTierRank,
                            EventTier = x.eveRider.EventTier,
                            EventTierScore = x.eveRider.EventTierScore,
                            CwrpBonus = x.eveRider.CwrpBonus,
                            RiderId = x.rider.Id,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            IsActive = true,
                            IsDropout = false

                        }));
                        _repoEventRider.InsertCollection(eventRider);
                    }
                }

            }
            else
            {
                var riderList = eventRiderDto.Where(x => !eventRiders.Select(y => y.rider.RiderId).Contains(x.RiderId)).ToList();
                foreach (var item in riderList)
                {
                    var riderResponse = Core.WebProxy.APIResponse($"rider&&id={item.RiderId}");
                    var returnRiderDetailResult = new RiderDto();
                    if (!string.IsNullOrEmpty(riderResponse.Result))
                    {
                        returnRiderDetailResult = JsonConvert.DeserializeObject<RiderDto>(riderResponse.Result);
                        if (returnRiderDetailResult != null)
                        {
                            _riderService.AddEditRiders(returnRiderDetailResult);
                        }
                    }
                }
                await AddEventRidersDropout(eventRiderDto, eventDto);
            }
        }

        /// <summary>
        /// Add EventBulls Dropouts
        /// </summary>
        /// <param name="eventBullDto">List Of EventBullDto</param>
        /// <returns></returns>
        public async Task AddEventBullsDropout(List<EventBullsDto> eventBullDto, EventDto eventDto)
        {
            var eventBull = new List<EventBull>();

            var eventDetail = _repoEvent
                        .Query()
                        .Filter(x => x.EventId.ToLower() == eventDto.EventId.ToLower())
                        .Get()
                        .SingleOrDefault();

            var eventBulls = _repoEventBull.Query().Filter(d => d.EventId == eventDetail.Id).Get();
            foreach (var item in eventBulls)
            {
                item.IsDropout = true;
                _repoEventBull.Update(item);
            }

            var eventBullDetail = (from bull in _repoBull.Query()
                                   .Get()
                                   join eveBull in eventBullDto on bull.BullId equals eveBull.BullId
                                   select new { bull, eveBull }).ToList();

            if (eventBullDto.Count() == eventBullDetail.Count())
            {
                //Check Event Bull already associated
                HashSet<int> bIDs = new HashSet<int>(eventBullDto.Select(s => s.BullId));
                var eventBullExists = _repoEventBull.Query().Filter(x => x.EventId == eventDetail.Id).Includes(x => x.Include(y => y.Bull)).Get().ToList();
                //Insert all the bulls if there is no any record in DB
                if (eventBullExists.Count == 0)
                {
                    eventBull.AddRange(eventBullDetail.Select(x => new EventBull
                    {
                        EventId = eventDetail.Id,
                        TierScore = x.eveBull.TierScore,
                        EventTier = x.eveBull.EventTier,
                        BullId = x.bull.Id,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsActive = true,
                        IsDropout = false
                    }));
                    _repoEventBull.InsertCollection(eventBull);
                }
                else //Check exists bulls first, if not exists thn enter in DB
                {
                    var existIds = eventBullExists.Select(x => x.Bull.BullId);
                    var bullResult1 = eventBullDetail.Where(m => !existIds.Contains(m.bull.BullId)).ToList();
                    if (bullResult1.Count > 0)
                    {
                        eventBull.AddRange(bullResult1.Select(x => new EventBull
                        {
                            EventId = eventDetail.Id,
                            TierScore = x.eveBull.TierScore,
                            EventTier = x.eveBull.EventTier,
                            BullId = x.bull.Id,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            IsActive = true,
                            IsDropout = false
                        }));
                        _repoEventBull.InsertCollection(eventBull);
                    }
                }
            }
            else
            {
                var bullList = eventBullDto.Where(x => !eventBullDetail.Select(y => y.bull.BullId).Contains(x.BullId)).ToList();
                foreach (var item in bullList)
                {
                    var bullResponse = Core.WebProxy.APIResponse($"bull&&id={item.BullId}");
                    var returnBullDetailResult = new BullDto();
                    if (!string.IsNullOrEmpty(bullResponse.Result))
                    {
                        returnBullDetailResult = JsonConvert.DeserializeObject<BullDto>(bullResponse.Result);
                        if (returnBullDetailResult != null)
                        {
                            _bullService.AddEditBulls(returnBullDetailResult);
                        }

                    }
                }
                await AddEventBullsDropout(eventBullDto, eventDto);
            }
        }

        /// <summary>
        /// Add EventDraw
        /// </summary>
        /// <param name="eventDrawDto">List Of EventDrawDto</param>
        /// <returns></returns>
        public void AddEventDraws(List<EventDrawDto> eventDrawDto, EventDto eventDto)
        {
            try
            {
                var eventDraw = new List<EventDraw>();

                var eventDetail = _repoEvent
                           .Query()
                           .Filter(x => x.EventId.ToLower() == eventDto.EventId.ToLower())
                           .Get()
                           .SingleOrDefault();

                var eventDrawInsert = _repoDraw
                     .Query()
                     .Filter(x => x.EventId == eventDetail.Id)
                     .Get()
                     .FirstOrDefault();

                if (eventDrawInsert == null)
                {
                    eventDraw.AddRange(eventDrawDto.Select(x => new EventDraw
                    {

                        EventId = eventDetail.Id,
                        BullId = x.BullId,
                        BullName = !string.IsNullOrEmpty(x.BullName) ? x.BullName : "",
                        RiderId = x.RiderId,
                        RiderName = !string.IsNullOrEmpty(x.RiderName) ? x.RiderName : "",
                        Round = x.Round
                    }));

                    _repoDraw.InsertCollection(eventDraw);
                }

            }
            catch (Exception ex)
            {
            }

        }

        /// <summary>
        /// Dispose All Services
        /// </summary>
        public void Dispose()
        {
            if (_repoEvent != null)
            {
                _repoEvent.Dispose();
            }
            if (_repoEventBull != null)
            {
                _repoEventBull.Dispose();
            }
            if (_repoEventRider != null)
            {
                _repoEventRider.Dispose();
            }
            if (_repoRider != null)
            {
                _repoRider.Dispose();
            }
            if (_repoBull != null)
            {
                _repoBull.Dispose();
            }
            if (_repoDraw != null)
            {
                _repoDraw.Dispose();
            }
            if (_bullService != null)
            {
                _bullService.Dispose();
            }

            if (_riderService != null)
            {
                _riderService.Dispose();
            }
        }

        /// <summary>
        /// returns the list of events which completed recently.
        /// </summary>
        /// <returns></returns>
        public List<EventServiceDto> GetRecentlyCompletedEvent()
        {
            var events = new List<EventServiceDto>();

            events = _repoEvent.Query()
           .Filter(x => (x.WinningDistributed == false && x.PerfTime < DateTime.Now) || (x.EventResult == "[]" && x.PerfTime < DateTime.Now))
           .Get()
           .Select(x => new EventServiceDto() { EventId = x.EventId, Id = x.Id }).ToList();
            return events;
        }

        /// <summary>
        /// update event winning distribution status
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task UpdateEventWinningDistribution(int eventId)
        {
            var eventExits = _repoEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();

            if (eventExits != null)
            {
                eventExits.WinningDistributed = true;
                await _repoEvent.UpdateAsync(eventExits);
            }
        }

        /// <summary>
        /// Update event result response in DB
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task UpdateEventResultResponse(int eventId, string response)
        {
            var eventExits = _repoEvent.Query().Filter(x => x.Id == eventId).Get().FirstOrDefault();

            if (eventExits != null)
            {
                eventExits.EventResult = response;
                eventExits.WinningDistributed = true;
                await _repoEvent.UpdateAsync(eventExits);
            }
        }


        /// <summary>
        /// returns the list of events which upcoming/current recently.
        /// </summary>
        /// <returns></returns>
        public async Task<List<EventDto>> GetUpcomingCurrentEvent()
        {
            var events = new List<EventDto>();

            DateTime dt = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now.AddDays(15);

            events = _repoEvent.Query()
           .Filter(x => (x.WinningDistributed == false && x.StartDate >= dt && x.StartDate < endDate))
           .Get()
           .Select(x => new EventDto() { EventId = x.EventId, Id = x.Id, PBRID = x.Pbrid, StartDate = x.StartDate, PerfTime = x.PerfTime }).ToList();
            return await Task.FromResult(events);
        }

        /// <summary>
        /// returns the list of events which upcoming/current recently.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetEventDrawDeclared(int eventId, string pbrId = "")
        {
            if (!string.IsNullOrEmpty(pbrId))
            {
                var eventObj = _repoEvent
                       .Query()
                       .Filter(x => x.Pbrid == pbrId)
                       .Get()
                       .FirstOrDefault();

                if (eventObj != null)
                    eventId = eventObj.Id;
            }

            var obj = _repoDraw
                       .Query()
                       .Filter(x => x.EventId == eventId)
                       .Get()
                       .FirstOrDefault();
            bool response = false;

            response = obj != null ? true : false;

            return await Task.FromResult(response);
        }

        /// <summary>
        /// Edit Event
        /// </summary>
        /// <param name="pbrid"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        public async Task EditEvent(string pbrid)
        {

            var eventExists = _repoEvent.Query()
         .Filter(x => x.Pbrid == pbrid) //x.EventId == item.EventId ||
         .Get().FirstOrDefault();

            if (eventExists != null)
            {
                //update
                eventExists.EventId = $"CAN_{GenerateRandomCode().ToUpper()}";
                eventExists.UpdatedDate = DateTime.Now;
                eventExists.WinningDistributed = true;
                await _repoEvent.UpdateAsync(eventExists);
            }

        }

        /// <summary>
        /// Add Edit Events
        /// </summary>
        /// <param name="eventDto">List Of EventDto</param>
        /// <returns></returns>
        public async Task ManageEvents(List<EventDto> eventDto)
        {
            #region New Code
            int count = 0;
            foreach (var item in eventDto.Where(x => x.StartDate.Date > DateTime.Now.Date))
            {
                try
                {
                    var eventExists = _repoEvent.Query()
             .Filter(x => x.Pbrid == item.PBRID) //x.EventId == item.EventId ||
             .Get().FirstOrDefault();

                    if (eventExists != null)
                    {
                        //update
                        eventExists.EventId = item.EventId;
                        eventExists.Title = !string.IsNullOrEmpty(item.Title) ? item.Title : "";
                        eventExists.Location = !string.IsNullOrEmpty(item.Location) ? item.Location : "";
                        eventExists.City = item.City;
                        eventExists.State = item.State;
                        eventExists.Type = !string.IsNullOrEmpty(item.Type) ? item.Type : "";
                        eventExists.Season = item.Season;
                        eventExists.StartDate = item.StartDate;
                        eventExists.PerfTime = item.PerfTime;
                        eventExists.Sanction = !string.IsNullOrEmpty(item.Sanction) ? item.Sanction : "";
                        eventExists.Pbrid = item.PBRID;
                        eventExists.UpdatedDate = DateTime.Now;
                        await _repoEvent.UpdateAsync(eventExists);
                    }
                    else
                    {
                        //insert
                        eventExists = new Event();
                        eventExists.EventId = item.EventId;
                        eventExists.Title = !string.IsNullOrEmpty(item.Title) ? item.Title : "";
                        eventExists.Location = !string.IsNullOrEmpty(item.Location) ? item.Location : "";
                        eventExists.City = item.City;
                        eventExists.State = item.State;
                        eventExists.Type = !string.IsNullOrEmpty(item.Type) ? item.Type : "";
                        eventExists.Season = item.Season;
                        eventExists.StartDate = item.StartDate;
                        eventExists.PerfTime = item.PerfTime;
                        eventExists.Sanction = !string.IsNullOrEmpty(item.Sanction) ? item.Sanction : "";
                        eventExists.Pbrid = item.PBRID;
                        eventExists.IsActive = true;
                        eventExists.IsDelete = false;
                        eventExists.UpdatedDate = DateTime.Now;
                        eventExists.CreatedDate = DateTime.Now;
                        await _repoEvent.InsertAsync(eventExists);
                        count = count + 1;
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (count > 0)
            {
                string emailBody = Utilities.GetEmailTemplateValue("NewEventArrived/Body", "");
                string emailSubject = Utilities.GetEmailTemplateValue("NewEventArrived/Subject", "");
                emailBody = emailBody.Replace("@@@UserEmail", "RankRide Team");

                await _emailSender.SendEmailAsync(
                    "info@rankridefantasy.com",
                    emailSubject,
                    emailBody);
            }
            #endregion
        }

        /// <summary>
        /// Check Event is completed and winnig distributed.
        /// </summary>
        /// <param name="pbrID"></param>
        /// <returns></returns>
        public async Task<EventCompleteDto> CheckEventComplete(string pbrID)
        {
            EventCompleteDto eventComplete = null;
            var eventExists = _repoEvent.Query()
         .Filter(x => x.Pbrid == pbrID) //x.EventId == item.EventId ||
         .Get().FirstOrDefault();

            if (eventExists != null)
            {
                eventComplete = new EventCompleteDto
                {
                    Id = eventExists.Id,
                    PBRId = eventExists.Pbrid,
                    RId = eventExists.EventId,
                    WinningDistributed = eventExists.WinningDistributed.HasValue ? eventExists.WinningDistributed.Value : false
                };
            }

            return await Task.FromResult(eventComplete);

        }

        private string GenerateRandomCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[4];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
    }
}
