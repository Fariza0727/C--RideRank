using RR.AdminData;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
     public class ContestMapper
     {
          /// <summary>
          /// Map Contest To ContestDto
          /// </summary>
          /// <param name="contests">List Of Contest</param>
          /// <returns></returns>
          public static IEnumerable<T> Map<T>(IEnumerable<Contest> contests)
          {
               return contests.Select(p => Map<T>(p));
          }

          public static T Map<T>(Contest contest)
          {
               if (typeof(T) == typeof(ContestLiteDto))
               {
                    return (T)Convert.ChangeType(MapContestLiteDto(contest), typeof(T));
               }
               if (typeof(T) == typeof(AwardLiteDto))
               {
                    return (T)Convert.ChangeType(MapAwardLiteDto(contest), typeof(T));
               }
               return (T)Convert.ChangeType(Map(contest), typeof(T));
          }

          /// <summary>
          /// Map Contest To ContestDto
          /// </summary>
          /// <param name="contest">The Contest</param>
          /// <returns>The ContestDto</returns>
          public static ContestDto Map(Contest contest)
          {
               return new ContestDto
               {

               };
          }

          /// <summary>
          /// Map Contest To ContestDto
          /// </summary>
          /// <param name="contest">The Contest</param>
          /// <returns>The ContestLiteDto</returns>
          public static ContestLiteDto MapContestLiteDto(Contest contest)
          {
               return new ContestLiteDto
               {
                    ContestId = contest.Id,
                    EventId = contest.EventId,
                    JoiningFee = contest.JoiningFee,
                    WinnerTitle = contest.WinningTitle,
                    Title = contest.Title,
                    ContestCategoryName = contest.ContestCategory.CategoryName,  
                    EntryFeeType = contest.EntryFeeTypeId == 1 ? "Cash" : "Token"
                    //AwardTypeName = (contest.ContestWinner.AwardType != null ? contest.AwardType.Name : ""),
                    //AwardTypeId = contest.AwardTypeId,
               };
          }

          public static AwardLiteDto MapAwardLiteDto(Contest contest)
          {
               return new AwardLiteDto
               {
                    //AwardName = contest.AwardType.Name,
                    //ContestId = contest.Id,
                    //RankFrom = contest.ContestWinner.Where(x => x.ContestId == contest.Id).Select(y => y.RankFrom),
                    //RankTo = contest.ContestWinner.Where(x => x.ContestId == contest.Id).Select(y => y.RankTo),
                    //Price = contest.ContestWinner.Where(x => x.ContestId == contest.Id).Select(y => y.AwardValue > 0 ?
                    //y.AwardValue.ToString() : y.Award.Message)
               };
          }
     }
}
