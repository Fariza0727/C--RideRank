using RR.Dto;
using RR.Dto.Calcutta;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface ICalcuttaService : IDisposable
    {
        /// <summary>
        Task<CalcuttaDetailDto> GetEventDetail(int eventId, string userId);
        Task<CalcuttaCheckoutDetailDto> GetCheckoutDetail(int eventId, string userId);
        Task<bool> AddCart(AddCartDto cartDto, string userId);
        Task<bool> RemoveCart(int cartId, string userId);
        Task<string> CreatePayment(int eventId, string userId, bool isRiderComp);

        Task<decimal> CapturePayment(int eventId, string token, string payerId, bool isRiderComp);

        Task<CalcuttaCheckoutRCDetailDto> GetRCCheckoutDetail(int eventId, string userId);
        Task<CalcuttRCDetailDto> GetRCEventDetail(int eventId, string userId);

        Task<Tuple<List<CalcuttaHistoryLiteDto>, int>> GetHistoryOfCurrentUser(string userId, int start = 0, int length = 10);
        Task<Tuple<List<CalcuttaHistoryLiteDto>, int>> GetAwardHistoryOfCurrentUser(string userId, int start = 0, int length = 10);

        // simple pick a team game
        Task<PickTeamDetailDto> GetSimplePickTeamDetail(int eventId, string userId);

        Task<CalcuttaEvent> GetEventById(int eventId);
        Task DeleteTeam(int teamId);
        Task<int> CreateTeam(IEnumerable<SimpleTeamDto> teamDto, int eventId, string userId);

        Task<Tuple<List<JoinUserContestLiteDto>, int>> JoinedUserContestAjax(int eventId, int start = 0, int length = 10);
        Task<Tuple<IEnumerable<PickEntryLiteDto>, int, int, decimal, DateTime>> GetTeamDetailOfCurrentUser(int eventId, int teamId);

        Task<Tuple<List<JoinUserContestLiteDto>, int>> JoinedCurrentUserContestAjax(string userId, int start = 0, int length = 10);
    }
}
