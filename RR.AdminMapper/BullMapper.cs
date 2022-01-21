using RR.Dto;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
     public static class BullMapper
     {
          /// <summary>
          /// Map Bull To BullDto
          /// </summary>
          /// <param name="bulls"></param>
          /// <returns></returns>
          public static IEnumerable<BullDto> Map(IEnumerable<Bull> bulls)
          {
               return bulls.Select(p => MapDto(p));
          }

          /// <summary>
          /// MapDto
          /// </summary>
          /// <param name="bull">The Bull</param>
          /// <returns>The BullDto</returns>
          public static BullDto MapDto(Bull bull)
          {
               return new BullDto
               {
                    Id = bull.BullId,
                    Brand = bull.Brand,
                    ActiveRank = bull.ActiveRank.ToString(),
                    AverageMark = bull.AverageMark,
                    BuckOffPerc = bull.BuckOffPerc,
                    BuckOffPercVsLeftHandRider = bull.BuckOffPercVsLeftHandRider,
                    BuckOffPercVsRightHandRider = bull.BuckOffPercVsRightHandRider,
                    BuckOffPercVsTopRider = bull.BuckOffPercVsTopRider,
                    HistoricalRank = bull.HistoricalRank.ToString(),
                    IsRegistered = Convert.ToString(bull.IsRegistered),
                    Mounted = bull.Mounted,
                    Name = bull.Name,
                    OutsVsTopRiders = bull.OutsVsTopRiders,
                    OutVsLeftHandRider = bull.OutVsLeftHandRider,
                    OutVsRightHandRider = bull.OutVsRightHandRider,
                    Owner = bull.Owner,
                    PowerRating = bull.PowerRating,
                    Rode = bull.Rode,
                    isActive = bull.IsActive,
                    
               };
          }
     }
}
