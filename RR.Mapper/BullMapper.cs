using RR.Dto;
using RR.StaticData;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
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
                //Age=bulls.Age,  
                AverageMark = bulls.AverageMark,
                BuckOffPerc = bulls.BuckOffPerc,
                //Breeding = bulls.Breeding,
                //BuckingStatistics = bulls.BuckingStatistics,
                //BuckOffStreak =bulls.BuckOffStreak,
                Owner = bulls.Owner,
                PowerRating = bulls.PowerRating,
                Name = bulls.Name,
                CreatedDate = bulls.CreatedDate,
                BullId = bulls.Id
            };
        }
    }
}
