using RR.AdminData;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
    public static class ContestMapper
    {
        public static IEnumerable<ContestDto> Map(IEnumerable<Contest> contests)
        {
            return contests.Select(p => MapDto(p));
        }

        public static ContestDto MapDto(Contest contest)
        {
            return new ContestDto
            {
                Id = contest.Id,
                OldTitle = contest.Title,
                Title = contest.Title,
                CreatdDate = contest.CreatdDate,
                CreatedBy = contest.CreatedBy,
                EventId = contest.EventId,
                IsActive = contest.IsActive,
                JoiningFee = contest.JoiningFee,
                JoiningFeeDisplay = contest.EntryFeeTypeId == 1 ? string.Concat("$", contest.JoiningFee) : string.Concat("T", contest.JoiningFee),
                IsRefunded = contest.IsRefunded,
                Members = contest.Members,
                Winners = contest.Winners,
                IsPrivate = contest.IsPrivate,
                UniqueCode = contest.UniqueCode,
                WinnerTitle = contest.WinningTitle,
                WinningPrice = contest.WinningPrice,
                WinningToken = contest.WinningToken,
                EntryFeeType = contest.EntryFeeTypeId
            };
        }
    }
}
