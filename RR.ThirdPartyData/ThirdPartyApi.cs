using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RR.Dto;
using RR.Repo;
using RR.StaticData;
using RR.StaticService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace RR.ThirdPartyData
{
    class ThirdPartyApi
    {
        private static IServiceProvider _serviceProvider;
        private static readonly ILog log = LogManager
             .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string apiKey = ConfigurationManager.AppSettings["ThirdPartyApikey"];
        private static string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        private static int HrtoUpdate = Convert.ToInt32(ConfigurationManager.AppSettings["HourToUpdate"]);

        /// <summary>
        /// Register All Services Here For Use
        /// </summary>
        public void RegisterServices()
        {
            var collection = new ServiceCollection();

            collection.AddDbContext<RankRideStaticContext>(options => options
            .UseSqlServer(connectionString));

            collection.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            //Add services

            collection.AddScoped<IRiderService, RiderService>();
            collection.AddScoped<IEventService, EventService>();
            collection.AddScoped<IBullService, BullService>();
            collection.AddScoped<IEmailSender, EmailSender>();

            _serviceProvider = collection.BuildServiceProvider();
        }

        /// <summary>
        /// This method is used for getting result of all bulls from api
        /// </summary>
        public async Task GetBullsRecord()
        {
            try
            {
                Console.WriteLine("Seed Bulls API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                var response = await Core.WebProxy.APIResponse("seedbulls", apiKey);
                var returnBullsResult = JsonConvert.DeserializeObject<List<BullDto>>(response);
                if (returnBullsResult != null && returnBullsResult.Count > 0)
                {
                    foreach (var item in returnBullsResult)
                    {
                        //Console.WriteLine("Bull Record API Call for - " + item.Id);
                        await AddBullRecord(item.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        /// <summary>
        /// This method is used for getting all riders record from api
        /// </summary>
        public async Task GetRidersRecord()
        {
            try
            {
                Console.WriteLine("Seed Rider API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                var response = await Core.WebProxy.APIResponse("seedriders", apiKey);
                Console.WriteLine("Rider Record response triggered");
                var returnRidersResult = JsonConvert.DeserializeObject<List<RiderDto>>(response);
                Console.WriteLine("Rider Record Count - " + returnRidersResult.Count);
                if (returnRidersResult != null && returnRidersResult.Count > 0)
                {
                    foreach (var item in returnRidersResult)
                    {
                        //Console.WriteLine("Rider Record API Call for - " + item.Id);
                        await AddRiderRecord(item.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error caught" + ex.Message);
                log.Error(ex.Message);
            }
        }

        /// <summary>
        /// This method is used for get all past events
        /// </summary>
        public async Task GetPastEventRecord()
        {
            try
            {
                var response = await Core.WebProxy.APIResponse("events_past", apiKey);
                var eventService = _serviceProvider.GetService<IEventService>();
                var returnPastEventsResult = JsonConvert.DeserializeObject<List<EventDto>>(response);
                if (returnPastEventsResult != null && returnPastEventsResult.Count > 0)
                {
                    await eventService.AddEditEvents(returnPastEventsResult);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// This method is used for getting all future events
        /// </summary>
        public async Task GetFutureEventRecord()
        {
            try
            {
                Console.WriteLine("Future Event API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                var response = await Core.WebProxy.APIResponse("events_future_full", apiKey);
                var eventService = _serviceProvider.GetService<IEventService>();
                var returnFutureEventsResult = JsonConvert.DeserializeObject<List<EventDto>>(response);
                if (returnFutureEventsResult != null && returnFutureEventsResult.Count > 0)
                {
                    Console.WriteLine("Awaiting event_service");
                    await eventService.AddEditEvents(returnFutureEventsResult);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// This method id used for getting all information 
        /// of current event which is happening from api
        /// </summary>
        public async Task GetCurrentEventRecord()
        {
            #region Check Event status.

            var eventService = _serviceProvider.GetService<IEventService>();

            var eventsData = await eventService.GetUpcomingCurrentEvent();

            foreach (var item in eventsData.OrderBy(x => x.StartDate))
            {
                //Check event detail using pbrid
                var response = await Core.WebProxy.APIResponse($"event&id={item.PBRID}", apiKey);

                if (string.IsNullOrEmpty(response) || response == "[]")
                    response = await Core.WebProxy.APIResponse($"event&id={item.EventId}", apiKey);

                if (response != "[]")
                {
                    var eventObject = JsonConvert.DeserializeObject<EventObjectDto>(response);

                    if (eventObject != null)
                    {
                        var drawAdded = await eventService.GetEventDrawDeclared(item.Id);

                        //Add Event draw in DB
                        if (eventObject.rid_status.ToLower() == "valid" && eventObject.result_count == "0" && !drawAdded && item.StartDate > DateTime.Now)
                        {
                            #region Event Draw Saving Code

                            Console.WriteLine("Current Event API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                            var bullService = _serviceProvider.GetService<IBullService>();
                            Console.WriteLine("bull service fetched");
                            var riderService = _serviceProvider.GetService<IRiderService>();
                            Console.WriteLine("rider services_fetched");
                            try
                            {   
                                Console.WriteLine("awaiting api");
                                var resp = await Core.WebProxy.APIResponse($"event_current&id={item.EventId}", apiKey);
                                
                                var returnCurrentEventResult = JsonConvert.DeserializeObject<List<CurrentEventDto>>(resp);
                                Console.WriteLine("CurrentEventResult deserialized");
                                if (returnCurrentEventResult != null && returnCurrentEventResult.Count > 0)
                                {
                                    Console.WriteLine("current events found");
                                    foreach (var eventItem in returnCurrentEventResult)
                                    {
                                        if (eventItem.EventDto.PerfTime < DateTime.Now)
                                            continue;
                                        //get all bulls from DB.
                                        var dbBulls = bullService.GetAllBulls(0, Int32.MaxValue, 0);
                                        //get all riders from DB.
                                        Console.WriteLine("bulls got");
                                        var dbRiders = riderService.GetAllRiders(0, Int32.MaxValue, 0);
                                        Console.WriteLine("riders got");

                                        var eventDto = eventItem.EventDto;
                                        var eventRiderList = eventItem.EventRiderDto;
                                        var eventBullList = eventItem.EventBullDto;

                                        //Get only those bulls which are not exists in DB
                                        HashSet<int> bIDs = new HashSet<int>(dbBulls.Result.Item1.Select(s => s.BullId));
                                        var bullResult = eventBullList.Where(m => !bIDs.Contains(m.BullId));
                                        //Get only those riders which are not exists in DB
                                        HashSet<int> rIDs = new HashSet<int>(dbRiders.Result.Item1.Select(s => s.RiderId));
                                        var riderResult = eventRiderList.Where(m => !rIDs.Contains(m.RiderId));

                                        Console.WriteLine("not in db bulls and riders checked");

                                        foreach (var bull in bullResult)
                                        {
                                            await AddBullRecord(bull.BullId);
                                        }
                                        foreach (var rider in riderResult)
                                        {
                                            await AddRiderRecord(rider.RiderId);
                                        }
                                        Console.WriteLine("rider and bull records added");
                                    }
                                    await eventService.CurrentAddEditEvents(returnCurrentEventResult);
                                    Console.WriteLine("event service triggered");
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex.Message);
                                Console.WriteLine("ERROR " + ex.Message);
                            }

                            #endregion
                        }

                        //Update Eventid for cancelled Event.
                        if (eventObject.rid_status.ToLower() != "valid" && item.StartDate < DateTime.Now)
                        {
                            await eventService.EditEvent(eventObject.pbrid);
                        }
                    }
                }
            }

            #endregion

            #region Old Code

            //Console.WriteLine("Current Event API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
            //var bullService = _serviceProvider.GetService<IBullService>();
            //var riderService = _serviceProvider.GetService<IRiderService>();
            //try
            //{
            //    var response = await Core.WebProxy.APIResponse("event_current", apiKey);

            //    var returnCurrentEventResult = JsonConvert.DeserializeObject<List<CurrentEventDto>>(response);
            //    if (returnCurrentEventResult != null && returnCurrentEventResult.Count > 0)
            //    {
            //        foreach (var item in returnCurrentEventResult)
            //        {
            //            if (item.EventDto.PerfTime < DateTime.Now)
            //                continue;
            //            //get all bulls from DB.
            //            var dbBulls = bullService.GetAllBulls(0, Int32.MaxValue, 0);
            //            //get all riders from DB.
            //            var dbRiders = riderService.GetAllRiders(0, Int32.MaxValue, 0);


            //            var eventDto = item.EventDto;
            //            var eventRiderList = item.EventRiderDto;
            //            var eventBullList = item.EventBullDto;

            //            //Get only those bulls which are not exists in DB
            //            HashSet<int> bIDs = new HashSet<int>(dbBulls.Result.Item1.Select(s => s.BullId));
            //            var bullResult = eventBullList.Where(m => !bIDs.Contains(m.BullId));
            //            //Get only those riders which are not exists in DB
            //            HashSet<int> rIDs = new HashSet<int>(dbRiders.Result.Item1.Select(s => s.RiderId));
            //            var riderResult = eventRiderList.Where(m => !rIDs.Contains(m.RiderId));

            //            foreach (var bull in bullResult)
            //            {
            //                await AddBullRecord(bull.BullId);
            //            }
            //            foreach (var rider in riderResult)
            //            {
            //                await AddRiderRecord(rider.RiderId);
            //            }
            //        }
            //        await eventService.CurrentAddEditEvents(returnCurrentEventResult);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log.Error(ex.Message);
            //}

            #endregion
        }

        public async Task AddBullRecord(int id)
        {
            var bullResponse = await Core.WebProxy.APIResponse($"bull&&id={id}");
            var returnBullDetailResult = new BullDto();
            var bullService = _serviceProvider.GetService<IBullService>();
            returnBullDetailResult = JsonConvert.DeserializeObject<BullDto>(bullResponse);
            if (returnBullDetailResult != null)
            {
                bullService.AddEditBulls(returnBullDetailResult);
            }
        }

        public async Task AddRiderRecord(int id)
        {
            var riderService = _serviceProvider.GetService<IRiderService>();
            var riderResponse = await Core.WebProxy.APIResponse($"rider&&id={id}");
            var returnRiderDetailResult = new RiderDto();
            returnRiderDetailResult = JsonConvert.DeserializeObject<RiderDto>(riderResponse);
            if (returnRiderDetailResult != null)
            {
                riderService.AddEditRiders(returnRiderDetailResult);
            }
        }

        /// <summary>
        /// This method is used for getting 
        /// all velocity level events of future
        /// </summary>
        public async Task VelocityLevelEvents()
        {
            try
            {
                Console.WriteLine("Velocity Event API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                var eventService = _serviceProvider.GetService<IEventService>();
                var response = await Core.WebProxy.APIResponse("events_future_velo", apiKey);
                var returnFutureEventsResult = JsonConvert.DeserializeObject<List<EventDto>>(response);
                if (returnFutureEventsResult != null && returnFutureEventsResult.Count > 0)
                {
                    await eventService.AddEditEvents(returnFutureEventsResult);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }



        /// <summary>
        /// This method is used for getting all future events and save the details accordingly
        /// </summary>
        public async Task GetFutureEventRecordUpdated()
        {
            try
            {
                Console.WriteLine("Future Event API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                var response = await Core.WebProxy.APIResponse("events_future", apiKey);
                var eventService = _serviceProvider.GetService<IEventService>();
                var returnFutureEventsResult = JsonConvert.DeserializeObject<List<EventDto>>(response);
                if (returnFutureEventsResult != null && returnFutureEventsResult.Count > 0)
                {
                    await eventService.ManageEvents(returnFutureEventsResult);

                    var filteredResults = returnFutureEventsResult.Where(x => x.StartDate.Date > 
                    DateTime.Now.Date).OrderBy(x => x.StartDate).ToList();

                    foreach (var item in filteredResults)
                    {
                        try
                        {
                            var drawAdded = await eventService.GetEventDrawDeclared(0, item.PBRID);

                            //Add Event draw in DB
                            if (item.rid_status.ToLower() == "valid" && item.is_current == 1 && item.result_count == "0" && !drawAdded && item.StartDate > DateTime.Now)
                            {
                                #region Event Draw Saving Code

                                Console.WriteLine("Current Event API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                                var bullService = _serviceProvider.GetService<IBullService>();
                                Console.WriteLine("bull service fetched");
                                var riderService = _serviceProvider.GetService<IRiderService>();
                                Console.WriteLine("rider service fetched");
                                try
                                {
                                    Console.WriteLine("awating api");
                                    var resp = await Core.WebProxy.APIResponse($"event_current&id={item.EventId}", apiKey);
                                    Console.WriteLine("api fetched using" + item.EventId);
                                    var returnCurrentEventResult = JsonConvert.DeserializeObject<List<CurrentEventDto>>(resp);
                                    Console.WriteLine("api json deserialized");
                                    if (returnCurrentEventResult != null && returnCurrentEventResult.Count > 0)
                                    {
                                        foreach (var eventItem in returnCurrentEventResult)
                                        {
                                            if (eventItem.EventDto.PerfTime < DateTime.Now)
                                                continue;
                                            //get all bulls from DB.
                                            Console.WriteLine("getting a billion bulls");
                                            var dbBulls = bullService.GetAllBulls(0, Int32.MaxValue, 0);
                                            //get all riders from DB.
                                            Console.WriteLine("getting a billion riders");
                                            var dbRiders = riderService.GetAllRiders(0, Int32.MaxValue, 0);


                                            var eventDto = eventItem.EventDto;
                                            var eventRiderList = eventItem.EventRiderDto;
                                            var eventBullList = eventItem.EventBullDto;
                                            Console.WriteLine("dtos fetched");
                                            //Get only those bulls which are not exists in DB
                                            HashSet<int> bIDs = new HashSet<int>(dbBulls.Result.Item1.Select(s => s.BullId));
                                            var bullResult = eventBullList.Where(m => !bIDs.Contains(m.BullId));
                                            //Get only those riders which are not exists in DB
                                            HashSet<int> rIDs = new HashSet<int>(dbRiders.Result.Item1.Select(s => s.RiderId));
                                            var riderResult = eventRiderList.Where(m => !rIDs.Contains(m.RiderId));

                                            foreach (var bull in bullResult)
                                            {
                                                await AddBullRecord(bull.BullId);
                                            }
                                            Console.WriteLine("bull records added");
                                            foreach (var rider in riderResult)
                                            {
                                                await AddRiderRecord(rider.RiderId);                                                
                                            }
                                            Console.WriteLine("rider and bull records added");
                                        }
                                        Console.WriteLine("going for event service");
                                        await eventService.CurrentAddEditEvents(returnCurrentEventResult);
                                        Console.WriteLine("event service called");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex.Message);
                                    Console.WriteLine(ex.Message);
                                }

                                #endregion
                            }
                            //Manage Event Rider/Bull Dropout
                            else if (item.rid_status.ToLower() == "valid" && !drawAdded && item.StartDate <= DateTime.Now.AddHours(HrtoUpdate))
                            {
                                Console.WriteLine("Current Event API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                                var resp = await Core.WebProxy.APIResponse($"event_current&id={item.EventId}", apiKey);
                                var returnCurrentEventResult = JsonConvert.DeserializeObject<List<CurrentEventDto>>(resp);
                                if (returnCurrentEventResult != null && returnCurrentEventResult.Count > 0)
                                {
                                    await eventService.CurrentAddEditEventsDropOut(returnCurrentEventResult);
                                }
                            }
                            //Update Eventid for cancelled Event.
                            if (item.rid_status.ToLower() != "valid" && item.StartDate < DateTime.Now)
                            {
                                await eventService.EditEvent(item.PBRID);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Dispose All Services
        /// </summary>
        public void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
