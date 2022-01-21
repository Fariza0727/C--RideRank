using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface IContestService : IDisposable
    {
        /// <summary>
        /// GetContestOfEvent
        /// </summary>
        /// <param name="eventId">EventId</param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<ContestLiteDto>, decimal, string>> GetContestOfEvent(int eventId, string eventName);

        /// <summary>
        /// Get Filtered Contest From Price Range
        /// </summary>
        /// <param name="eventId">Event Id</param>
        /// <param name="priceFrom">Price From</param>
        /// <param name="priceTo">Price To</param>
        /// <param name="priceFilter">Price Filter</param>
        /// <returns></returns>
        Task<IEnumerable<ContestLiteDto>> FilterContest(int eventId, int priceFrom, int priceTo, int priceFilter);

        /// <summary>
        /// GetContestAwards
        /// </summary>
        /// <param name="contestId">Contest Id</param>
        /// <returns></returns>
        Task<Tuple<List<AwardLiteDto>, AwardLiteDescriptionDto>> GetContestAwards(int contestId, int eventId);

        /// <summary>
        ///  JoinedUserContest
        /// </summary>
        /// <param name="contestId">Contest Id</param>
        /// <param name="eventId">Event id</param>
        /// <returns></returns>
        Task<Tuple<List<JoinUserContestLiteDto>, int, string>> JoinedUserContest(long contestId, int eventId, int start = 0, int length = 10, int column = 0, string searchStr = "", string sort = "");

        Task<Tuple<List<JoinUserContestLiteDto>, int>> JoinedUserContestAjax(long contestId, int eventId, int start = 0, int length = 10);

        /// <summary>
        /// GetContestById
        /// </summary>
        /// <param name="contestId">Contest Id</param>
        /// <returns></returns>
        Task<ContestLiteDto> GetContestById(int contestId);

        /// <summary>
        ///  Get Joined ContestList By EventId
        /// </summary>
        /// <param name="eventId">EventId</param>
        /// <param name="UserId">UserId</param>
        /// <returns></returns>
        Task<List<long>> GetJoinedContestListByEventId(int eventId, string UserId);

        /// <summary>
        /// Get Contest By UniqueCode
        /// </summary>
        /// <param name="uniqueCode">Unique Code</param>
        /// <returns></returns>
        Task<ContestLiteDto> GetContestByUniqueCode(string uniqueCode);

        /// <summary>
        /// Get Contest Of CurrentUser
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<UserContestLiteDto>, int>> GetContestOfCurrentUser(string userId, int start=0, int length =10);

        /// <summary>
        /// Get Contest Detail Of Current User 
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="contestId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<RiderContestLiteDto>, IEnumerable<BullContestLiteDto>, int, int>> GetContestDetailOfCurrentUser(int eventId,
                                                                                                                     int contestId,
                                                                                                                     int teamId);
        /// <summary>
        /// Get Contest Standings of current user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<UserContestLiteDto>> GetContestStandingsApi(string userId);

    }
}
