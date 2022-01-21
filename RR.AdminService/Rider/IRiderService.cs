using Microsoft.AspNetCore.Mvc.Rendering;
using RR.Dto;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IRiderService : IDisposable
    {
        /// <summary>
        /// Get All Riders
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Record </param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<RiderDto>, int>> GetAllRiders(int start, int length, int column, string searchStr = "", string sort = "", int rank = 1, bool getCleanRider = false);

        /// <summary>
        /// Get riders as select list item
        /// </summary>
        /// <param name="selectedid">selected Id</param>
        /// <param name="predict">selected Id</param>
        /// <returns>IEnumerable<SelectListItem></returns>
        Task<IEnumerable<SelectListItem>> GetRiders(int selectedid = 0);

        /// <summary>
        /// Get Rider By Id
        /// </summary>
        /// <param name="riderId">Rider Id</param>
        /// <returns>The RiderDto</returns>
        Task<RiderDto> GetRiderById(int riderId);

        /// <summary>
        /// Update Rider Detail
        /// </summary>
        /// <param name="riderDto">The RiderDto</param>
        /// <returns></returns>
        Task UpdateRiderDetail(RiderDto riderDto);

        /// <summary>
        /// Update Rider Status
        /// </summary>
        /// <param name="riderId">Rider Id</param>
        /// <returns></returns>
        Task UpdateStatus(int riderId);

        /// <summary>
        /// Delete Rider By Id
        /// </summary>
        /// <param name="riderId">Rider Id</param>
        /// <returns></returns>
        Task DeleteRider(int riderId);

        Task DeleteNotSeenRider(bool isParmanentdelete = false);

        /// <summary>
        /// Get riders as card
        /// </summary>
        /// <returns>Tuple<int, int> item1:total, item2:active</returns>
        Task<Tuple<int, int>> GetRidersAsCard();
    }
}
