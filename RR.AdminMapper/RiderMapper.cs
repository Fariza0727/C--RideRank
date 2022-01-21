using RR.Dto;
using RR.StaticData;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
    public static class RiderMapper
    {
        /// <summary>
        /// Map List of Rider To RiderDto
        /// </summary>
        /// <param name="riders"></param>
        /// <returns></returns>
        public static IEnumerable<RiderDto> Map(IEnumerable<Rider> riders)
        {
            return riders.Select(p => MapDto(p));
        }

        /// <summary>
        /// MapDto
        /// </summary>
        /// <param name="rider">The Rider</param>
        /// <returns>The RiderDto</returns>
        public static RiderDto MapDto(Rider rider)
        {
            return new RiderDto
            {
                Id = rider.RiderId,
                Hand = rider.Hand,
                Mounted = rider.Mounted,
                MountedCurrent = rider.MountedCurrent,
                Name = rider.Name,
                RidePerc = rider.RidePerc,
                RidePrecCurent = rider.RidePrecCurent,
                RiderPower = rider.RiderPower,
                RiderPowerCurrent = rider.RiderPowerCurrent,
                Rode = rider.Rode,
                Streak = rider.Streak,
                IsActive = rider.IsActive,
                CWRP = rider.Cwrp ?? 9999
            };
        }
    }
}
