using RR.AdminData;
using RR.Dto.AwardType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RR.AdminMapper
{
    public static class AwardTypeMapper
    {
        /// <summary>
        /// Map List Of events To EventDto
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public static IEnumerable<AwardTypeDto> Map(IEnumerable<AwardType> awards)
        {
            return awards.Select(p => MapDto(p));
        }

        /// <summary>
        /// MapDto
        /// </summary>
        /// <param name="eventDetail">An Event Detail</param>
        /// <returns>An EventDto</returns>
        public static AwardTypeDto MapDto(AwardType award)
        {
            return new AwardTypeDto
            {
                Id = award.Id,
                Name = award.Name
            };
        }
    }
}
