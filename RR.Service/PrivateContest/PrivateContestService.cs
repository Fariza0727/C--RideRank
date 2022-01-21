using RR.AdminData;
using RR.Dto;
using RR.Repo;
using RR.StaticData;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
    public class PrivateContestService : IPrivateContestService
    {
        #region Constructor

        private readonly IRepository<Contest, RankRideAdminContext> _repoContest;
        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IRepository<ContestWinner, RankRideAdminContext> _repoContestWinner;

        public PrivateContestService(IRepository<Contest, RankRideAdminContext> repoContest,
            IRepository<Event, RankRideStaticContext> repoEvent,
            IRepository<ContestWinner, RankRideAdminContext> repoContestWinner)
        {
            _repoContest = repoContest;
            _repoEvent = repoEvent;
            _repoContestWinner = repoContestWinner;
        }

        #endregion

        public async Task<PrivateContestDto> AddEditContest(PrivateContestDto privateContestDto)
        {
            long Id = privateContestDto.Id;
            string contestCode = GetPrivateContestCode();
            try
            {
                Contest contest = new Contest();
                if (privateContestDto.Id > 0)
                {
                    contest = _repoContest.Query().Filter(x => x.Id == privateContestDto.Id).Get().FirstOrDefault();
                    if (contest != null)
                    {
                        contest.Title = privateContestDto.Title;
                        contest.EventId = privateContestDto.EventId;
                        contest.IsActive = privateContestDto.IsActive;
                        contest.IsRefunded = false;
                        contest.JoiningFee = privateContestDto.JoiningFee;
                        contest.Members = privateContestDto.Members;
                        contest.Winners = privateContestDto.Winners;
                        contest.WinningTitle = privateContestDto.WinnerTitle;
                        contest.IsPrivate = true;
                        contest.EntryFeeTypeId = privateContestDto.EntryFeeType;
                        contest.WinningPrice = privateContestDto.WinningPrice;
                        contest.EntryFeeTypeId = privateContestDto.EntryFeeType;

                        await _repoContest.UpdateAsync(contest);
                    }
                }
                else
                {
                    decimal Price = (privateContestDto.JoiningFee * privateContestDto.Members) - (((privateContestDto.JoiningFee * privateContestDto.Members) * 10) / 100);
                    contest.Title = string.Empty;
                    contest.CreatdDate = DateTime.Now;
                    contest.UpdatedDate = DateTime.Now;
                    contest.CreatedBy = privateContestDto.CreatedBy;
                    contest.UpdatedBy = privateContestDto.CreatedBy;
                    contest.EventId = privateContestDto.EventId;
                    contest.IsActive = true;
                    contest.IsRefunded = false;
                    contest.JoiningFee = privateContestDto.JoiningFee;
                    contest.Members = privateContestDto.Members;
                    contest.Winners = privateContestDto.Winners;
                    contest.WinningTitle = privateContestDto.WinnerTitle;
                    contest.UniqueCode = contestCode;
                    contest.IsPrivate = true;
                    contest.WinningPrice = privateContestDto.WinningPrice > 0 ? privateContestDto.WinningPrice : Price;
                    contest.ContestCategoryId = 8;
                    contest.EntryFeeTypeId = 1;
                    await _repoContest.InsertAsync(contest);
                    privateContestDto.Id = contest.Id;
                    privateContestDto.UniqueCode = contestCode;
                }
            }
            catch (Exception)
            {
            }
            return privateContestDto;
        }

        public void Dispose()
        {
            if (_repoContest != null)
            {
                _repoContest.Dispose();
            }
        }

        public string GetPrivateContestCode()
        {
            string Code = "PRC-" + RandomString(6, true) + RandomString(3, false);
            return Code;
        }
        private static Random random = new Random();
        public static string RandomString(int length, bool IsAlphanumeric)
        {
            if (IsAlphanumeric)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            else
            {
                const string chars = "0123456789";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }
    }
}
