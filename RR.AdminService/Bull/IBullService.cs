using Microsoft.AspNetCore.Mvc.Rendering;
using RR.Dto;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IBullService : IDisposable
    {
        /// <summary>
        /// Get All Bulls
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Record </param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<BullLiteDto>, int>> GetAllBulls(int start, int length, int column, string searchStr = "", string sort = "", int rank = 1, bool getCleanbull = false);

        /// <summary>
        /// Get bull as select list item
        /// </summary>
        /// <param name="selectedid">selected Id</param>
        /// <param name="predict"></param>
        /// <returns>IEnumerable<SelectListItem></returns>
        Task<IEnumerable<SelectListItem>> GetBulls(int selectedid = 0);

        /// <summary>
        /// Get Bull By Id
        /// </summary>
        /// <param name="bullId">Bull Id</param>
        /// <returns>The BullDto</returns>
        Task<BullDto> GetBullById(int bullId);

        /// <summary>
        /// Update Bull Detail
        /// </summary>
        /// <param name="bullDto">The BullDto</param>
        /// <returns></returns>
        Task UpdateBullDetail(BullDto bullDto);

        /// <summary>
        /// Update Bull Status
        /// </summary>
        /// <param name="bullId">Bull Id</param>
        /// <returns></returns>
        Task UpdateStatus(int bullId);

        /// <summary>
        /// Delete Bull By Id
        /// </summary>
        /// <param name="bullId">Bull Id</param>
        /// <returns></returns>
        Task DeleteBullById(int bullId);
        Task DeleteNotSeenBulls(bool isParmanentdelete = false);
        /// <summary>
        /// Get bulls as card
        /// </summary>
        /// <returns>Tuple<int, int> item1:total, item2:active</returns>
        Task<Tuple<int, int>> GetBullsAsCard();
    }
}
