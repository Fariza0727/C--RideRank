using RR.AdminData;
using RR.Dto.Award;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RR.AdminMapper
{
    public static class AwardMapper
    {
        /// <summary>
        /// Map List Of awards To awardDto
        /// </summary>
        /// <param name="awards"></param>
        /// <returns></returns>
        public static IEnumerable<AwardDto> Map(IEnumerable<Award> awards)
        {
            return awards.Select(p => MapDto(p));
        }

        /// <summary>
        /// MapDto
        /// </summary>
        /// <param name="eventDetail">An award Detail</param>
        /// <returns>An awardDto</returns>
        public static AwardDto MapDto(Award award)
        {
            return new AwardDto
            {
                Id = award.Id,
                AwardTypeId = award.AwardTypeId,
                CreatedBy = award.CreatedBy,
                CreatedDate = award.CreatedOn,
                Image = award.Image,
                Message = award.Message,
                Token = award.Token
            };
        }
    }
}
