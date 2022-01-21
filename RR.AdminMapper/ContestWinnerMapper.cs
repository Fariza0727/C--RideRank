using RR.AdminData;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RR.AdminMapper
{
    public class ContestWinnerMapper
    {
        public static IEnumerable<ContestWinnerDto> Map(IEnumerable<ContestWinner> winners)
        {
            return winners.Select(p => MapDto(p));
        }

        public static ContestWinnerDto MapDto(ContestWinner winner)
        {
            return new ContestWinnerDto
            {
                Id = winner.Id,
                CreatedBy = winner.CreatedBy,
                //AwardId = winner.AwardId,
                //AwardValue = winner.AwardValue,
                ContestId = winner.ContestId,
                RankFrom = winner.RankFrom,
                RankTo = winner.RankTo
            };
        }
    }
}
