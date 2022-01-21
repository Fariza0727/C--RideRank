using RR.AdminData;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
    public class PageDetailMapper
    {
        /// <summary>
        /// Map Page Detail To PageDetailDto
        /// </summary>
        /// <param name="pages">List Of PageDetail</param>
        /// <returns>List Of PageDetailDto</returns>
        public static List<PageDto> Map(List<PageDetail> pages)
        {
            return pages.Select(p => MapDto(p)).ToList();
        }
        /// <summary>
        /// Map Dto
        /// </summary>
        /// <param name="user">The Page Detail</param>
        /// <returns>The PageDetailDto</returns>
        public static PageDto MapDto(PageDetail page)
        {
            return new PageDto
            {
                Id = page.Id.ToString(),
                PageName = page.PageName,
                PageUrl = page.PageBaseUrl + page.PageUrl
            };
        }

    }
}

