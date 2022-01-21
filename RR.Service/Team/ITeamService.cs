using RR.Data;
using RR.Dto;
using RR.Dto.Team;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface ITeamService : IDisposable
    {
        Task DeleteTeam(int teamId);

        Task<string> UpdateRedisCache();
        /// <summary>
        /// Event Players Of Event By Id
        /// </summary>
        /// <param name="eventId">Event Id</param>
        /// <param name="contestId">Contest Id</param>
        /// <returns>returns team formation object</returns>
        Task<TeamFormationDto> EventPlayersById(int eventId, int contestId, int teamId);

        /// <summary>
        /// Event Players Of Event By Id
        /// </summary>
        /// <param name="eventId">Event Id</param>
        /// <param name="contestId">Contest Id</param>
        /// <returns>returns team formation object</returns>
        Task<TeamFormationDto> EventPlayersByIdApi(int eventId, int contestId, int teamId);

        /// <summary>
        /// Create Team
        /// </summary>
        /// <param name="teamDto">The TeamDto</param>
        /// <returns></returns>
        Task<int> CreateTeam(IEnumerable<TeamDto> teamDto, int eventId, string userId);

        /// <summary>
        /// Create Team For api
        /// </summary>
        /// <param name="teamFormationDto">The TeamFormationDto</param>
        /// <returns></returns>
        Task<int> CreateTeamApi(TeamFormationDto teamFormationDto);

        /// <summary>
        ///  TeamList
        /// </summary>
        /// <param name="contestId">Contest Id</param>
        /// <param name="teamId">Team Id</param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<TeamLiteDto>, int>> TeamList(int eventId, int contestId, string user, int start, int length, int column,
             string searchStr = "", string sort = "");

        /// <summary>
        /// Add join contest wit team.
        /// </summary>
        /// <param name="joinedContestDto"></param>
        /// <returns></returns>
        Task AddJoinTeamContest(JoinedContestDto joinedContestDto);

        /// <summary>
        /// Get joined teams by eventId
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<List<Team>> GetJoinedTeamsByEventId(int eventId);

        /// <summary>
        /// Update team bull point
        /// </summary>
        /// <param name="teamBullId"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        Task UpdateTeamBullPoint(int teamBullId, decimal point = 0);

        /// <summary>
        /// update team rider point
        /// </summary>
        /// <param name="teamRiderId"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        Task UpdateTeamRiderPoint(int teamRiderId, decimal point = 0);

        /// <summary>
        /// update 
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        Task UpdateTeamTotalPoint(int teamId, decimal point = 0);

        /// <summary>
        /// Get list of joined teams by contest Id
        /// </summary>
        /// <param name="contestId"></param>
        /// <returns></returns>
        Task<List<JoinedContestDto>> GetJoinedTeamsByContestId(long contestId);

        /// <summary>
        /// Get Points and rank of player Of Last Event   
        /// Home Page
        /// </summary>
        /// <param name="getAll"></param>
        /// <returns></returns>
        Task<IEnumerable<PlayStandingLiteDto>> GetPlayerPointsOfEvent(bool getAll = false);

        /// <summary>
        /// Get points of player according to its contest selected
        /// Home Page
        /// </summary>
        /// <returns></returns>
        Task<List<PlayStandingLiteDto>> GetAllPlayerPointsOfEventContest();

        /// <summary>
        /// Get All Players Contest Joined Points
        /// Account
        /// </summary>
        /// <returns></returns>
        Task<List<StandingDto>> GetStandings(bool isPlayer = false, int start = 0, int length = 10, int column = 0, string searchStr = "", string sort = "");

        /// <summary>
        /// Get All Year Standings Of CurrentUser
        /// Account
        /// </summary>
        /// <returns></returns>
        Task<List<StandingDto>> GetYearStandingsOfCurrentUser(string userId, int start = 0, int length = 10, int column = 0, string searchStr = "", string sort = "");

        Task<Tuple<List<StandingDto>, List<StandingDto>, List<StandingDto>>> GetStandingsApi(bool isPlayer = false);


        /// <summary>
        /// Get joined teams by eventId
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<List<Team>> GetJoinedTeamsByEventIdWinningService(int eventId);

        /// <summary>
        /// Get Last event standing 
        /// </summary>
        /// <param name="count"></param>
        /// <returns>List<PlayStandingLiteDto></returns>
        Task<List<PlayStandingLiteDto>> GetLastEventStatndingPlayerPoints(int count = 0);
        Task<List<PlayStandingLiteDto>> GetLastEventStatndingFreePlayerPoints(int count = 0);
        Task<List<PlayStandingLiteDto>> GetTopReferred(int count = 0);

        Task<int> GetTeamIdByEventIdUserId(int eventId, string userId);
        Task<TeamFormationDetailDto> EventPlayerDataById(int eventId, int contestId, int teamId);
        Task<string> GetLastEventName();
    }
}