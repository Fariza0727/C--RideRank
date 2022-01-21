using RR.Dto;
using RR.StaticData;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
    public class RiderMapper
    {
        /// <summary>
        /// Map Rider To RiderDto
        /// </summary>
        /// <param name="riders">List Of Riders</param>
        /// <returns>List Of RiderDto</returns>
        public static IEnumerable<RiderDto> Map(IEnumerable<Rider> riders)
        {
            return riders.Select(p => Map(p));
        }

        /// <summary>
        /// Map Rider To RiderDto
        /// </summary>
        /// <param name="riders">The Rider</param>
        /// <returns>The RiderDto</returns>
        public static RiderDto Map(Rider riders)
        {
            return new RiderDto
            {
                RiderId = riders.RiderId,
                Id = riders.Id,
                Name = riders.Name,
                Hand = riders.Hand,
                RidePerc = riders.RidePerc,
                RidePrecCurent = riders.RidePrecCurent,
                Streak = riders.Streak,
                RiderPowerCurrent = riders.RiderPowerCurrent,
                Rode = riders.Rode,
                CreatedDate = riders.CreatedDate
                
            };
        }
    }
}
