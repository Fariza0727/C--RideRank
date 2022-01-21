using RR.AdminData;
using RR.Core;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
    public class ContestWinnerService : IContestWinnerService
    {
        #region Constructor

        private readonly IRepository<Contest, RankRideAdminContext> _repoContest;
        private readonly IRepository<ContestWinner, RankRideAdminContext> _repoContestWinner;

        public ContestWinnerService(IRepository<Contest, RankRideAdminContext> repoContest,
             IRepository<ContestWinner, RankRideAdminContext> repoContestWinner)
        {
            _repoContest = repoContest;
            _repoContestWinner = repoContestWinner;
        }

        #endregion

        public async Task AddEditWinners(ContestAwardDto contestDto)
        {
            #region Delete Previous entry

            await DeleteContestWinnerByContestId(contestDto.ContestId);

            #endregion

            #region Add new entry

            List<ContestWinner> contestWinners = new List<ContestWinner>();
            int length = contestDto.RankFrom.Length == contestDto.RankTo.Length ? contestDto.RankTo.Length :
                contestDto.RankFrom.Length > contestDto.RankTo.Length ? contestDto.RankFrom.Length : contestDto.RankTo.Length;
            length = length == contestDto.Value.Length ? length :
                length > contestDto.Value.Length ? length : contestDto.Value.Length;

            int from, to, value = 0;
            long awardid = 0;
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                {
                    from = contestDto.RankFrom[i].HasValue ? contestDto.RankFrom[i].Value : 0;
                    to = contestDto.RankTo[i].HasValue ? contestDto.RankTo[i].Value : 0;
                    value = contestDto.Value[i].HasValue ? contestDto.Value[i].Value : 0;
                    awardid = 12;

                    if (value > 0 || awardid > 0)
                    {
                        if (from > 0 && to > 0)
                        {
                            ContestWinner winner = new ContestWinner();
                            //winner.AwardValue = Convert.ToDecimal(value);
                            winner.ContestId = contestDto.ContestId;
                            winner.CreatedBy = contestDto.CreatedBy;
                            winner.UpdatedBy = contestDto.CreatedBy;
                            winner.CreatedDate = DateTime.Now;
                            winner.UpdatedDate = DateTime.Now;
                            winner.RankFrom = from;
                            winner.RankTo = to;
                            winner.PricePercentage = value;
                            //winner.AwardId = 12;
                            contestWinners.Add(winner);
                        }
                    }
                }
            }
            int index = 1, rankFrom = 1, rankTo = 1;
            foreach (var item in contestWinners.Skip(0).Take(10))
            {
                if (item.RankFrom < item.RankFrom + item.RankTo)
                    rankFrom = 0;
                if (item.RankTo < item.RankFrom)
                    rankTo = 0;

                if (rankFrom == 0 && rankTo == 0)
                {
                    contestWinners.RemoveAt(index);
                }
                index++;
            }

            if (contestWinners.Count > 0)
            {
                _repoContestWinner.InsertCollection(contestWinners);
            }
            #endregion
        }

        public async Task DeleteContestWinnerByContestId(long id)
        {
            var predicate = PredicateBuilder.True<ContestWinner>()
           .And(x => (x.ContestId == id));

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
