using RR.AdminData;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
    public class BannerMapper
    {
        /// <summary>
        /// Map Banner To BannerDto
        /// </summary>
        /// <param name="news">List Of Banners</param>
        /// <returns>List of Bannerdto</returns>
        public static IEnumerable<BannerDto> Map(IEnumerable<Banner> banners)
        {
            return banners.Select(p => MapDto(p));
        }

        /// <summary>
        /// MapDto
        /// </summary>
        /// <param name="banner">The Banner</param>
        /// <returns>The BannerDto</returns>
        public static BannerDto MapDto(Banner banner)
        {
            return new BannerDto
            {
                Id = banner.Id,
                PicPath = banner.PicPath,
                Title = banner.Title,
                Url = banner.Url,
                CreatedBy = banner.CreatedBy,
                UpdatedBy=banner.UpdatedBy,
                CreatedDate = banner.CreatedDate,
                UpdatedDate = banner.UpdatedDate
            };
        }
    }
}
