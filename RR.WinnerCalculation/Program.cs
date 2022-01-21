using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RR.AdminData;
using RR.Data;
using RR.Dto;
using RR.Dto.Event;
using RR.Dto.Team;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;


namespace RR.WinnerCalculation
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string apiKey = ConfigurationManager.AppSettings["ThirdPartyApikey"];
        private static string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        private static string connectionStringRRStatic = ConfigurationManager.AppSettings["ConnectionStringStatic"];
        private static string connectionStringRRAdmin = ConfigurationManager.AppSettings["ConnectionStringAdmin"];

        static void Main(string[] args)
        {
             
            RegisterServices();

            GetEventResult().Wait();

            DisposeServices();
        }

        /// <summary>
        /// Register All Services Here For Use
        /// </summary>
        public static void RegisterServices()
        {
            var collection = new ServiceCollection();

            collection.AddDbContext<RankRideStaticContext>(options => options
             .UseSqlServer(connectionStringRRStatic));

            collection.AddDbContext<RankRideContext>(options => options
            .UseSqlServer(connectionString));

            collection.AddDbContext<RankRideAdminContext>(options => options
            //.UseSqlServer("Server=1106227-WEB1;Database=RankRideAdmin;User ID=usr_rankride;Password=jellodonkey39/42;Persist Security Info=true;"));
            .UseSqlServer(connectionStringRRAdmin));

            collection.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

            //Add services

            collection.AddScoped<RR.StaticService.IRiderService, RR.StaticService.RiderService>();
            collection.AddScoped<RR.StaticService.IEventService, RR.StaticService.EventService>();
            collection.AddScoped<RR.StaticService.IEmailSender, RR.StaticService.EmailSender>();
            collection.AddScoped<RR.StaticService.IBullService, RR.StaticService.BullService>();
            collection.AddScoped<RR.Service.ITeamService, RR.Service.TeamService>();
            collection.AddScoped<RR.Service.IBullService, RR.Service.BullService>();
            collection.AddScoped<RR.Service.IRiderService, RR.Service.RiderService>();
            collection.AddScoped<RR.AdminService.IContestService, RR.AdminService.ContestService>();
            collection.AddScoped<RR.Service.IContestUserWinnerService, RR.Service.ContestUserWinnerService>();
            collection.AddScoped<RR.Service.IBullRiderPicturesService, RR.Service.BullRiderPicturesService>();
            collection.AddScoped<RR.Service.User.IUserService, RR.Service.User.UserService>();
            collection.AddScoped<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            _serviceProvider = collection.BuildServiceProvider();
        }

        /// <summary>
        /// Get recently completed events and update the winners point and amount.
        /// </summary>
        /// <returns></returns>
        public static async Task GetEventResult()
        {
            //Get list of events which are recently completed
            var eventStaticService = _serviceProvider.GetService<RR.StaticService.IEventService>();
            var teamService = _serviceProvider.GetService<RR.Service.ITeamService>();
            var bullService = _serviceProvider.GetService<RR.Service.IBullService>();
            var riderService = _serviceProvider.GetService<RR.Service.IRiderService>();
            var contestAdminService = _serviceProvider.GetService<RR.AdminService.IContestService>();

            Console.WriteLine("Future Event API Call started. " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));

            #region Get Events By Future API

            var responseCurrent = await Core.WebProxy.APIResponse("events_future");
            var eventService = _serviceProvider.GetService<RR.StaticService.IEventService>();
            var returnCurrentEventsResult = JsonConvert.DeserializeObject<List<EventDto>>(responseCurrent);

            var completedEventsData = returnCurrentEventsResult.Where(x => x.PerfTime < DateTime.Now && x.result_count != "0").ToList();

            foreach (var item in completedEventsData)
            {
                try
                {
                    var eventStatus = await eventService.CheckEventComplete(item.PBRID);

                    if (eventStatus != null && !eventStatus.WinningDistributed)
                    {
                        try
                        {
                            //Get Event result response from PBS API
                            var resultResponse = await Core.WebProxy.APIResponse($"event_result&id={eventStatus.RId}");

                            //Check teams are avalible for this contest or not.
                            var teams = await teamService.GetJoinedTeamsByEventIdWinningService(eventStatus.Id);

                            if (!string.IsNullOrEmpty(resultResponse) && teams.Count > 0)
                            {
                                //Deserialize event result response.
                                var eventResult = JsonConvert.DeserializeObject<List<EventResultDto>>(resultResponse);
                                foreach (var team in teams)
                                {
                                    try
                                    {
                                        decimal bullTotal = 0M, riderTotal = 0M;
                                        //Calculate all team bulls total points
                                        foreach (var bull in team.TeamBull)
                                        {
                                            var bullData = await bullService.GetBullRecordById(bull.BullId);
                                            var bTotal = eventResult.Where(x => x.pbid == (bullData != null ? bullData.Id : bull.BullId)).Select(y => y.rr_bull_score).Sum();
                                            bullTotal = bullTotal + bTotal;
                                            //Update Team Bull Point in DB
                                            await teamService.UpdateTeamBullPoint(bull.TeamBullId, bTotal);
                                        }
                                        //Calculate all team riders total points
                                        foreach (var rider in team.TeamRider)
                                        {
                                            var riderData = await riderService.GetRiderById(rider.RiderId);
                                            var rTotal = eventResult.Where(x => x.rider_id == (riderData != null ? riderData.RiderId : rider.RiderId)).Select(y => y.rr_rider_score).Sum();
                                            riderTotal = riderTotal + rTotal;
                                            //Update Team rider Point in DB
                                            await teamService.UpdateTeamRiderPoint(rider.TeamRiderId, rTotal);
                                        }

                                        var TeamTotal = bullTotal + riderTotal;

                                        await teamService.UpdateTeamTotalPoint(team.Id, TeamTotal);
                                        Console.WriteLine($"got response for event:- {item}, Team Total Point is:- {TeamTotal}");
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }

                                //Distribute contest winning among all the users who joined the contest.
                                var teamsPoints = await teamService.GetJoinedTeamsByEventIdWinningService(eventStatus.Id);

                                //Distribute Event winnings
                                await DistributeWinning(eventStatus.Id, teamsPoints, contestAdminService, teamService);

                                //Update event result ressponse in database
                                await eventStaticService.UpdateEventResultResponse(eventStatus.Id, resultResponse);
                            }
                            else
                            {
                                Console.WriteLine($"Update the event status as distributed for Event {eventStatus.RId}");
                                await eventStaticService.UpdateEventWinningDistribution(eventStatus.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            #endregion


            #region Get Events By Future API

            var response = await Core.WebProxy.APIResponse("events_future");

            var returnFutureEventsResult = JsonConvert.DeserializeObject<List<EventDto>>(response);

            var completedEventsList = returnFutureEventsResult.Where(x => x.PerfTime < DateTime.Now && x.result_count != "0").ToList();

            foreach (var item in completedEventsList)
            {
                try
                {
                    var eventStatus = await eventService.CheckEventComplete(item.PBRID);

                    if (eventStatus != null && !eventStatus.WinningDistributed)
                    {
                        try
                        {
                            //Get Event result response from PBS API
                            var resultResponse = await Core.WebProxy.APIResponse($"event_result&id={eventStatus.RId}");

                            //Check teams are avalible for this contest or not.
                            var teams = await teamService.GetJoinedTeamsByEventIdWinningService(eventStatus.Id);

                            if (!string.IsNullOrEmpty(resultResponse) && teams.Count > 0)
                            {
                                //Deserialize event result response.
                                var eventResult = JsonConvert.DeserializeObject<List<EventResultDto>>(resultResponse);
                                foreach (var team in teams)
                                {
                                    try
                                    {
                                        decimal bullTotal = 0M, riderTotal = 0M;
                                        //Calculate all team bulls total points
                                        foreach (var bull in team.TeamBull)
                                        {
                                            var bullData = await bullService.GetBullRecordById(bull.BullId);
                                            var bTotal = eventResult.Where(x => x.pbid == (bullData != null ? bullData.Id : bull.BullId)).Select(y => y.rr_bull_score).Sum();
                                            bullTotal = bullTotal + bTotal;
                                            //Update Team Bull Point in DB
                                            await teamService.UpdateTeamBullPoint(bull.TeamBullId, bTotal);
                                        }
                                        //Calculate all team riders total points
                                        foreach (var rider in team.TeamRider)
                                        {
                                            var riderData = await riderService.GetRiderById(rider.RiderId);
                                            var rTotal = eventResult.Where(x => x.rider_id == (riderData != null ? riderData.RiderId : rider.RiderId)).Select(y => y.rr_rider_score).Sum();
                                            riderTotal = riderTotal + rTotal;
                                            //Update Team rider Point in DB
                                            await teamService.UpdateTeamRiderPoint(rider.TeamRiderId, rTotal);
                                        }

                                        var TeamTotal = bullTotal + riderTotal;

                                        await teamService.UpdateTeamTotalPoint(team.Id, TeamTotal);
                                        Console.WriteLine($"got response for event:- {item}, Team Total Point is:- {TeamTotal}");
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }

                                //Distribute contest winning among all the users who joined the contest.
                                var teamsPoints = await teamService.GetJoinedTeamsByEventIdWinningService(eventStatus.Id);

                                //Distribute Event winnings
                                await DistributeWinning(eventStatus.Id, teamsPoints, contestAdminService, teamService);

                                //Update event result ressponse in database
                                await eventStaticService.UpdateEventResultResponse(eventStatus.Id, resultResponse);
                            }
                            else
                            {
                                Console.WriteLine($"Update the event status as distributed for Event {eventStatus.RId}");
                                await eventStaticService.UpdateEventWinningDistribution(eventStatus.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            #endregion

            var completedEvents = eventStaticService.GetRecentlyCompletedEvent();
            foreach (var item in completedEvents)
            {
                try
                {
                    //Get Event result response from PBS API
                    var resultResponse = await Core.WebProxy.APIResponse($"event_result&id={item.EventId}",apiKey);

                    //Check teams are avalible for this contest or not.
                    var teams = await teamService.GetJoinedTeamsByEventIdWinningService(item.Id);

                    if (!string.IsNullOrEmpty(resultResponse) && teams.Count > 0)
                    {
                        //Deserialize event result response.
                        var eventResult = JsonConvert.DeserializeObject<List<EventResultDto>>(resultResponse);
                        foreach (var team in teams)
                        {
                            try
                            {
                                decimal bullTotal = 0M, riderTotal = 0M;
                                //Calculate all team bulls total points
                                foreach (var bull in team.TeamBull)
                                {
                                    var bullData = await bullService.GetBullRecordById(bull.BullId);
                                    var bTotal = eventResult.Where(x => x.pbid == (bullData != null ? bullData.Id : bull.BullId)).Select(y => y.rr_bull_score).Sum();
                                    bullTotal = bullTotal + bTotal;
                                    //Update Team Bull Point in DB
                                    await teamService.UpdateTeamBullPoint(bull.TeamBullId, bTotal);
                                }
                                //Calculate all team riders total points
                                foreach (var rider in team.TeamRider)
                                {
                                    var riderData = await riderService.GetRiderById(rider.RiderId);
                                    var rTotal = eventResult.Where(x => x.rider_id == (riderData != null ? riderData.RiderId : rider.RiderId)).Select(y => y.rr_rider_score).Sum();
                                    riderTotal = riderTotal + rTotal;
                                    //Update Team rider Point in DB
                                    await teamService.UpdateTeamRiderPoint(rider.TeamRiderId, rTotal);
                                }

                                var TeamTotal = bullTotal + riderTotal;

                                await teamService.UpdateTeamTotalPoint(team.Id, TeamTotal);
                                Console.WriteLine($"got response for event:- {item}, Team Total Point is:- {TeamTotal}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }

                        //Distribute contest winning among all the users who joined the contest.
                        var teamsPoints = await teamService.GetJoinedTeamsByEventIdWinningService(item.Id);

                        //Distribute Event winnings
                        await DistributeWinning(item.Id, teamsPoints, contestAdminService, teamService);

                        //Update event result ressponse in database
                        await eventStaticService.UpdateEventResultResponse(item.Id, resultResponse);
                    }
                    else
                    {
                        Console.WriteLine($"Update the event status as distributed for Event {item.EventId}");
                        await eventStaticService.UpdateEventWinningDistribution(item.Id);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("Success Future Event API");
        }

        /// <summary>
        /// Dispose All Services
        /// </summary>
        public static void DisposeServices()
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

        /// <summary>
        /// Distribute Winnings based on ranks
        /// </summary>
        /// <param name="EventId"></param>
        /// <param name="teams"></param>
        public static async Task DistributeWinning(int EventId, List<Team> teams, RR.AdminService.IContestService serviceObj, RR.Service.ITeamService teamServiceObj)
        {
            List<ContestUserWinnerDto> userWinnerDtos = new List<ContestUserWinnerDto>();
            var cuwService = _serviceProvider.GetService<RR.Service.IContestUserWinnerService>();
            var userService = _serviceProvider.GetService<RR.Service.User.IUserService>();
            //Get list of all contests by event Id
            var contests = await serviceObj.GetContestByEventId(EventId);

            //Get Contest joined teams
            foreach (var contest in contests)
            {
                //Get Winnings for contest
                var contestWinnings = await serviceObj.GetContestWinningByContestId(contest.Id);

                if (contestWinnings.Count > 0)
                {
                    //get joined team for contest
                    var joinedTeamForContests = teams.Where(x => x.JoinedContest.Where(xx => xx.ContestId == contest.Id).ToList().Count > 0).Select(d=>new
                    { IsTeamPaidMember = (userService.GetUserDetail(d.UserId).Result).IsPaidMember ?? false,
                    Team = d}).ToList();


                    var PaidMembers = joinedTeamForContests.Where(d => d.IsTeamPaidMember).Select(r => r.Team).ToList();
                    await DistributeManage(EventId, PaidMembers, contest, contestWinnings, cuwService);

                    var NonPaidMember = joinedTeamForContests.Where(d => !d.IsTeamPaidMember).Select(r => r.Team).ToList();
                    await DistributeManage(EventId, NonPaidMember, contest, contestWinnings, cuwService);
                    
                }
                else
                {
                    //Mark no winners added in DB
                    Console.WriteLine($"Winnings not added for contest:- {contest.WinnerTitle}");
                }
            }
        }

        private static async Task DistributeManage(int EventId, List<Team> Teams, ContestDto contest, List<ContestWinner> contestWinnings, RR.Service.IContestUserWinnerService cuwService)
        {
            List<ContestUserWinnerDto> userWinnerDtos = new List<ContestUserWinnerDto>();


            var rankResult = Teams.OrderByDescending(x => x.TeamPoint).Select(s => new TeamRankDto()
            {
                Rank = Teams.Count(x => x.TeamPoint > s.TeamPoint) + 1,
                ContestId = s.JoinedContest.FirstOrDefault() != null ? s.JoinedContest.FirstOrDefault().ContestId : 0,
                ContestType = s.ContestType,
                EventId = s.EventId,
                TeamPoint = s.TeamPoint,
                UserId = s.UserId,
                TeamID = s.Id
            });

            foreach (var item in rankResult)
            {
                var winnings = contestWinnings.Where(x => x.IsPaidMember == item.IsTeamPaidMember && item.Rank >= x.RankFrom && item.Rank <= x.RankTo).ToList();
                if (winnings.Count > 0)
                {
                    foreach (var winner in winnings)
                    {
                        ContestUserWinnerDto winnerDto = new ContestUserWinnerDto();
                        winnerDto.ContestId = contest.Id;
                        winnerDto.ContestWinnerId = winner.Id;
                        winnerDto.CreatedDate = DateTime.Now;
                        winnerDto.EventId = EventId;
                        winnerDto.TeamId = item.TeamID;
                        winnerDto.TeamRank = item.Rank;
                        winnerDto.UserId = item.UserId;
                        userWinnerDtos.Add(winnerDto);
                    }
                }
            }

            if (userWinnerDtos.Count > 0)
            {
                //save winning in DB;
                await cuwService.SaveContestWinner(userWinnerDtos);
                Console.WriteLine($"Winnings distributed for contest:- {contest.WinnerTitle}");
                userWinnerDtos = new List<ContestUserWinnerDto>();
            }
        }
    }
}
