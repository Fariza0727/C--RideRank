using RR.Dto;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RR.StaticMapper
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
                Id = riders.Id,
                Name = riders.Name,
                Hand = riders.Hand,
                RidePerc = Math.Round(riders.RidePerc, 2) * 100,
                RidePrecCurent = Math.Round(riders.RidePrecCurent, 2) * 100,
                Streak = Math.Round(riders.Streak, 2),
                RiderPowerCurrent = Math.Round(riders.RiderPowerCurrent, 3),
                RiderPower = Math.Round(riders.RiderPower, 3),                
                Rode = riders.Rode,
                Mounted = riders.Mounted,
                RiderId = riders.RiderId,
                //CWRP = riders.Cwrp.HasValue ? riders.Cwrp.Value : 0
                CWRP = riders.Cwrp ?? 0
            };
        }
    }
}
