using RR.Dto;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RR.StaticMapper
{
     public class BullMapper
     {
          /// <summary>
          /// Map Bull To BullDto
          /// </summary>
          /// <param name="bulls">List Of Bulls</param>
          /// <returns>List Of All BullDto</returns>
          public static IEnumerable<BullDto> Map(IEnumerable<Bull> bulls)
          {
               return bulls.Select(p => Map(p));
          }

          /// <summary>
          /// Map Bull To BullDto
          /// </summary>
          /// <param name="bulls">The Bull Record</param>
          /// <returns>The BullDto Record</returns>
          public static BullDto Map(Bull bulls)
          {
               return new BullDto
               {
                    Id = bulls.BullId,
                    BullId = bulls.Id,
                    //Age=bulls.Age,  
                    AverageMark = Math.Round(bulls.AverageMark, 2),
                    BuckOffPerc = Math.Round(bulls.BuckOffPerc, 2),
                    //Breeding = bulls.Breeding,
                    //BuckingStatistics = bulls.BuckingStatistics,
                    //BuckOffStreak =bulls.BuckOffStreak,
                    Owner = bulls.Owner,
                    PowerRating = Math.Round(bulls.PowerRating),
                    Name = bulls.Name,
                    CreatedDate = bulls.CreatedDate
               };
          }
     }
}
