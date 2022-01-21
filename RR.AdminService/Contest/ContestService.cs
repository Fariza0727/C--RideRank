using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RR.AdminData;
using RR.AdminMapper;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class ContestService : IContestService
    {
        #region Constructor

        private readonly IRepository<Contest, RankRideAdminContext> _repoContest;
        private readonly IRepository<JoinedContest, RankRideContext> _repoJoinedContest;
        private readonly IRepository<Event, RankRideStaticContext> _repoEvent;
        private readonly IRepository<ContestWinner, RankRideAdminContext> _repoContestWinner;

        public ContestService(IRepository<Contest, RankRideAdminContext> repoContest,
            IRepository<Event, RankRideStaticContext> repoEvent,
            IRepository<ContestWinner, RankRideAdminContext> repoContestWinner,
          IRepository<JoinedContest, RankRideContext> repoJoinedContest)
        {
            _repoContest = repoContest;
            _repoEvent = repoEvent;
            _repoContestWinner = repoContestWinner;
            _repoJoinedContest = repoJoinedContest;
        }

        #endregion

        /// <summary>
        /// Get all contests by event id.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <param name="EventId"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<ContestDto>, int>> GetAllContest(int start, int length, int column, string searchStr = "", string sort = "", long eventId = 0)
        {
            int count = 0;
            var predicate = PredicateBuilder.True<Contest>()
           .And(x => (searchStr == "" || x.Title.ToLower().Contains(searchStr.ToLower())))
           .And(x => x.IsActive == true)
           .And(x => (eventId == 0 || x.EventId == eventId));

            var contests = _repoContest
                .Query()
                .Filter(predicate).Get();
            var joinedcontest = _repoJoinedContest.Query().Get();
            var contestList = contests.Select(y => new ContestDto
            {
                Id = y.Id,
                Title = y.Title,
                EventName = _repoEvent.FindById(y.EventId)?.Title,
                PerfTime = _repoEvent.FindById(y.EventId).PerfTime,
                JoiningFee = y.JoiningFee,
                JoiningFeeDisplay = y.EntryFeeTypeId == 1 ? ("$" + y.JoiningFee.ToString()) : ("T" + y.JoiningFee.ToString()),
                Members = y.Members,
                Winners = y.Winners,
                IsActive = y.IsActive,
                JoinedMembers = joinedcontest.Where(x => x.ContestId == y.Id).Count()
            }).ToList();

            count = contestList.Count;

            switch (column)
            {
                case 0:
                    contestList = (sort == "desc" ? contestList.OrderByDescending(xx => xx.Title).ToList() : contestList.OrderBy(xx => xx.Title).ToList());
                    break;
                case 1:
                    contestList = (sort == "desc" ? contestList.OrderByDescending(xx => xx.EventName).ToList() : contestList.OrderBy(xx => xx.EventName).ToList());
                    break;
                case 2:
                    contestList = (sort == "desc" ? contestList.OrderByDescending(xx => xx.PerfTime).ToList() : contestList.OrderBy(xx => xx.PerfTime).ToList());
                    break;
                case 3:
                    contestList = (sort == "desc" ? contestList.OrderByDescending(xx => xx.Winners).ToList() : contestList.OrderBy(xx => xx.Winners).ToList());
                    break;
                case 4:
                    contestList = (sort == "desc" ? contestList.OrderByDescending(xx => xx.JoiningFee).ToList() : contestList.OrderBy(xx => xx.JoiningFee).ToList());
                    break;
                case 5:
                    contestList = (sort == "desc" ? contestList.OrderByDescending(xx => xx.JoinedMembers).ToList() : contestList.OrderBy(xx => xx.JoinedMembers).ToList());
                    break;
                default:
                    contestList = (sort == "desc" ? contestList.OrderByDescending(xx => xx.CreatdDate).ToList() : contestList.OrderBy(xx => xx.CreatdDate).ToList());
                    break;
            }
            int skip = start * length;
            return await Task.FromResult(new Tuple<IEnumerable<ContestDto>, int>(contestList.Skip(skip).Take(length), count));
            //return await Task.FromResult(new Tuple<IEnumerable<ContestDto>, int>(contests
            //        .GetPage(start, length, out count).Select(y => new ContestDto
            //        {
            //            Id = y.Id,
            //            Title = y.Title,
            //            EventName = _repoEvent.FindById(y.EventId).Title,
            //            PerfTime = _repoEvent.FindById(y.EventId).PerfTime,
            //            JoiningFee = y.JoiningFee,
            //            JoiningFeeDisplay = y.EntryFeeTypeId == 1 ? ("$" + y.JoiningFee.ToString()) : ("T" + y.JoiningFee.ToString()),
            //            Members = y.Members,
            //            Winners = y.Winners,
            //            IsActive = y.IsActive,
            //            JoinedMembers = joinedcontest.Where(x => x.ContestId == y.Id).Count()
            //        }), count));
        }

        /// <summary>
        /// Add/Edit contest. 
        /// </summary>
        /// <param name="contestDto"></param>
        /// <returns></returns>
        public async Task<long> AddEditContest(ContestDto contestDto, string userId)
        {
            long Id = contestDto.Id;
            try
            {
                Contest contest = new Contest();
                if (contestDto.Id > 0)
                {
                    contest = _repoContest.Query().Filter(x => x.Id == contestDto.Id).Get().FirstOrDefault();
                    if (contest != null)
                    {
                        contest.Title = contestDto.Title;
                        contest.EventId = contestDto.EventId;
                        contest.IsActive = contestDto.IsActive;
                        contest.IsRefunded = false;
                        contest.JoiningFee = contestDto.JoiningFee;
                        contest.Members = contestDto.Members;
                        contest.Winners = contestDto.Winners;
                        contest.WinningTitle = contestDto.WinnerTitle;
                        contest.IsPrivate = false;
                        //contest.EntryFeeTypeId = contestDto.EntryFeeType;
                        contest.WinningPrice = contestDto.WinningPrice;
                        contest.WinningToken = contestDto.WinningToken;
                        //contest.ContestCategoryId = contestDto.ContestCategoryId;
                        contest.UpdatedDate = DateTime.Now;
                        contest.UpdatedBy = userId;
                        await _repoContest.UpdateAsync(contest);
                    }
                }
                else
                {
                    contest.Title = contestDto.Title;
                    contest.CreatdDate = DateTime.Now;
                    contest.CreatedBy = userId;
                    contest.EventId = contestDto.EventId;
                    contest.IsActive = contestDto.IsActive;
                    contest.IsRefunded = false;
                    contest.JoiningFee = contestDto.JoiningFee;
                    contest.Members = contestDto.Members;
                    contest.Winners = contestDto.Winners;
                    contest.WinningTitle = contestDto.WinnerTitle;
                    contest.UniqueCode = Guid.NewGuid().ToString();
                    contest.IsPrivate = false;
                    contest.WinningPrice = contestDto.WinningPrice;
                    contest.WinningToken = contestDto.WinningToken;
                    contest.ContestCategoryId = contestDto.ContestCategoryId;
                    contest.EntryFeeTypeId = contestDto.EntryFeeType;
                    contest.CreatdDate = DateTime.Now;
                    contest.CreatedBy = userId;
                    contest.UpdatedDate = DateTime.Now;
                    contest.UpdatedBy = userId;
                    await _repoContest.InsertAsync(contest);
                    Id = contest.Id;
                }
            }
            catch (Exception)
            {
            }
            return Id;
        }
        /// <summary>
        /// Check for Title Already Exists
        /// </summary>
        /// <param name="findTitle"></param>
        /// <returns></returns>
        public async Task<bool> IsTitleAvailable(string findTitle)
        {
            var title = _repoContest.Query().Get();
            bool isAvailable = title.Where(x => x.Title == findTitle).Count() > 0 ? true : false;

            return await Task.FromResult(isAvailable);
        }

        /// <summary>
        /// Check contest available for event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> IsContestAvailable(int eventId, long Id)
        {
            var contests = _repoContest.Query().Get();
            bool isAvailable = contests.Where(x => x.Id != Id && x.EventId == eventId).Count() > 0 ? true : false;

            return await Task.FromResult(isAvailable);
        }

        /// <summary>
        /// Get contest detail by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ContestDto> GetContest(long id)
        {
            var result = _repoContest
                .Query()
                    .Filter(x => x.Id == id)
                    .Get()
                    .SingleOrDefault();
            return await Task.FromResult(ContestMapper.MapDto(result));
        }

        /// <summary>
        /// Delete contest by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteContest(int id)
        {
            var contest = _repoContest.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
            if (contest != null)
            {
                contest.IsActive = false;
                await _repoContest.UpdateAsync(contest);
            }
        }
        /// <summary>
        /// returns list of contests of particular event.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<List<ContestDto>> GetContestByEventId(int eventId)
        {
            var predicate = PredicateBuilder.True<Contest>()
           .And(x => x.IsActive == true)
           .And(x => (x.EventId == eventId));

            var contests = _repoContest
                .Query()
                .Filter(predicate);

            return await Task.FromResult((contests
                    .Get()).Select(y => new ContestDto
                    {
                        Id = y.Id,
                        EventName = _repoEvent.FindById(y.EventId).Title,
                        JoiningFee = y.JoiningFee,
                        Members = y.Members,
                        Winners = y.Winners,
                        IsActive = y.IsActive,
                        WinnerTitle = y.WinningTitle
                    }).ToList());
        }

        /// <summary>
        /// returns list of winnings of contest.
        /// </summary>
        /// <param name="contestId"></param>
        /// <returns></returns>
        public async Task<List<ContestWinner>> GetContestWinningByContestId(long contestId)
        {
            var predicate = PredicateBuilder.True<ContestWinner>()
           .And(x => (x.ContestId == contestId));

            var contestWinnings = _repoContestWinner
                .Query()
                .Filter(predicate);
            return await Task.FromResult(contestWinnings
                    .Get().ToList());
        }

        /// <summary>
        /// returns list of contests of particular event.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>        
        public async Task<List<SelectListItem>> GetContestDropdownByEventId(int eventId)
        {

            var contests = _repoContest
                   .Query()
                   .Filter(x => x.EventId == eventId && x.IsPrivate == false)
                   .Get();

            return await Task.FromResult(contests.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = c.Title }).ToList());
        }

        /// <summary>
        /// Dispose User Service
        /// </summary>
        public void Dispose()
        {
            if (_repoContest != null)
            {
                _repoContest.Dispose();
            }
            if (_repoEvent != null)
            {
                _repoEvent.Dispose();
            }
            if (_repoContestWinner != null)
            {
                _repoContestWinner.Dispose();
            }
            if (_repoJoinedContest != null)
            {
                _repoJoinedContest.Dispose();
            }


        }


    }
}
