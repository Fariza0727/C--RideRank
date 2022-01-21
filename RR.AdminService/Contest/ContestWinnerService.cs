using Microsoft.AspNetCore.Mvc.Rendering;
using RR.AdminData;
using RR.Core;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class ContestWinnerService : IContestWinnerService
    {
        #region Constructor

        private readonly IRepository<Contest, RankRideAdminContext> _repoContest;
        private readonly IAwardService _awardService;
        private readonly IAwardTypeService _awardtypeService;
        private readonly IRepository<ContestWinner, RankRideAdminContext> _repoContestWinner;

        public ContestWinnerService(IRepository<Contest, RankRideAdminContext> repoContest, IRepository<ContestWinner, RankRideAdminContext> repoContestWinner,
            IAwardService awardService, IAwardTypeService awardtypeService)
        {
            _repoContest = repoContest;
            _repoContestWinner = repoContestWinner;
            _awardService = awardService;
            _awardtypeService = awardtypeService;
        }

        #endregion

        /// <summary>
        /// Get all contest Winners by contest id.
        /// </summary>
        /// <param name="ContestId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ContestWinnerDto>> GetContestWinner(long ContestId = 0)
        {
            var predicate = PredicateBuilder.True<ContestWinner>()
           .And(x => (x.ContestId == ContestId));

            var winnerdata = _repoContestWinner
                .Query()
                .Get().Where(x => x.ContestId == ContestId);
            //  .Filter(predicate).Get();

            var merchandiseAwards = await _awardService.GetMerchandiseAward();
            var otherAwards = await _awardService.GetOtherAward();

            return await Task.FromResult(new List<ContestWinnerDto>(winnerdata.Select(item => new ContestWinnerDto
            {
                Id = item.Id,
                RankFrom = item.RankFrom,
                RankTo = item.RankTo,
                PricePercentage = item.PricePercentage,
                TokenPercentage = item.TokenPercentage,
                Merchandise = item.Marchendise.HasValue ? item.Marchendise.Value : 0,
                OtherReward = item.OtherReward.HasValue ? item.OtherReward.Value : 0,
                IsPaidMember = item.IsPaidMember,
                MerchandiseList = merchandiseAwards.Select(x => new SelectListItem()
                {
                    Text = x.Message,
                    Value = x.Id.ToString(),
                    Selected = (x.Id == item.Id)
                }).ToList(),
                OtherRewardList = otherAwards.Select(x => new SelectListItem()
                {
                    Text = x.Message,
                    Value = x.Id.ToString(),
                    Selected = (x.Id == item.Id)
                }).ToList(),
            })));

            // return await Task.FromResult(ContestWinnerMapper.Map(winners));
        }

        public async Task AddEditWinners(ContestAwardDto contestDto, string userId)
        {
            #region Delete Previous entry

            var isNonPaidMember = (contestDto.IsPaidMember[0].HasValue && contestDto.IsPaidMember[0].Value == false);
            await DeleteContestWinnerByContestId(contestDto.ContestId, isNonPaidMember);

            #endregion

            #region Add new entry

            List<ContestWinner> contestWinners = new List<ContestWinner>();
            int length = contestDto.RankFrom.Length == contestDto.RankTo.Length ? contestDto.RankTo.Length :
                contestDto.RankFrom.Length > contestDto.RankTo.Length ? contestDto.RankFrom.Length : contestDto.RankTo.Length;
            length = length == contestDto.Merchandise.Length ? length :
                length > contestDto.Merchandise.Length ? length : contestDto.Merchandise.Length;

            int from, to, pricePercentage, tokenPercentage = 0;
            long merchandise, otherAward = 0;
            bool? isPaidMember;
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                {
                    from = contestDto.RankFrom[i].HasValue ? contestDto.RankFrom[i].Value : 0;
                    to = contestDto.RankTo[i].HasValue ? contestDto.RankTo[i].Value : 0;
                    pricePercentage = contestDto.PricePercentage[i].HasValue ? contestDto.PricePercentage[i].Value : 0;
                    tokenPercentage = contestDto.TokenPercentage[i].HasValue ? contestDto.TokenPercentage[i].Value : 0;
                    merchandise = contestDto.Merchandise[i].HasValue ? contestDto.Merchandise[i].Value : 0;
                    otherAward = contestDto.OtherReward[i].HasValue ? contestDto.OtherReward[i].Value : 0;
                    isPaidMember = contestDto.IsPaidMember[i];

                    if (pricePercentage >= 0 || tokenPercentage >= 0 || merchandise >= 0 || otherAward >= 0)
                    {
                        if (from > 0 && to > 0)
                        {
                            ContestWinner winner = new ContestWinner();
                            // winner.AwardValue = Convert.ToDecimal(value);
                            winner.ContestId = contestDto.ContestId;
                            winner.CreatedBy = contestDto.CreatedBy;
                            winner.RankFrom = from;
                            winner.RankTo = to;
                            winner.PricePercentage = pricePercentage;
                            winner.TokenPercentage = tokenPercentage;
                            if (merchandise > 0)
                                winner.Marchendise = merchandise;
                            if (otherAward > 0)
                                winner.OtherReward = otherAward;
                            winner.CreatedBy = userId;
                            winner.UpdatedBy = userId;
                            winner.UpdatedDate = DateTime.Now;
                            winner.CreatedDate = DateTime.Now;
                            winner.UpdatedDate = DateTime.Now;
                            winner.IsPaidMember = isPaidMember;
                            contestWinners.Add(winner);
                        }
                    }
                }
            }

            if (contestWinners.Count > 0)
            {
                _repoContestWinner.InsertCollection(contestWinners);
            }
            #endregion
        }

        public async Task AddEditWinnerMTT(ContestAwardDto contestDto, string userId)
        {
            #region Delete Previous entry

            await DeleteContestWinnerByContestId(contestDto.ContestId);

            #endregion

            #region Add new entry

            List<ContestWinner> contestWinners = new List<ContestWinner>();
            int length = contestDto.RankFrom.Length == contestDto.RankTo.Length ? contestDto.RankTo.Length :
                contestDto.RankFrom.Length > contestDto.RankTo.Length ? contestDto.RankFrom.Length : contestDto.RankTo.Length;
            length = length == contestDto.AwardId.Length ? length :
                length > contestDto.AwardId.Length ? length : contestDto.AwardId.Length;

            int from, to, awardId = 0;
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                {
                    from = contestDto.RankFrom[i].HasValue ? contestDto.RankFrom[i].Value : 0;
                    to = contestDto.RankTo[i].HasValue ? contestDto.RankTo[i].Value : 0;
                    awardId = contestDto.AwardId[i].HasValue ? contestDto.AwardId[i].Value : 0;

                    if (from > 0 && to > 0 && awardId > 0)
                    {
                        ContestWinner winner = new ContestWinner();
                        //winner.AwardValue = 0;
                        winner.ContestId = contestDto.ContestId;
                        winner.CreatedBy = userId;
                        winner.RankFrom = from;
                        winner.RankTo = to;
                        //winner.AwardId = awardId;
                        contestWinners.Add(winner);
                    }
                }
            }

            if (contestWinners.Count > 0)
            {
                _repoContestWinner.InsertCollection(contestWinners);
            }
            #endregion
        }

        public async Task DeleteContestWinner(long id)
        {
            await _repoContestWinner.DeleteAsync(id);
        }

        public async Task DeleteContestWinnerByContestId(long id, bool isNonPaidMember = false)
        {
            var predicate = PredicateBuilder.True<ContestWinner>()
           .And(x => (x.ContestId == id));

            if (isNonPaidMember)
                predicate = predicate.And(r => r.IsPaidMember == false);
            else
                predicate = predicate.And(r => r.IsPaidMember != false);
            
            var winners = _repoContestWinner
                .Query()
                .Filter(predicate).Get();
            foreach (var item in winners.ToList())
            {
                try
                {
                    await _repoContestWinner.DeleteAsync(item);
                }
                catch (Exception)
                {
                }
            }
        }

        public void Dispose()
        {
            if (_repoContest != null)
            {
                _repoContest.Dispose();
            }
        }
    }
}
