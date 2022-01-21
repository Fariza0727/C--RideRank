using RR.AdminData;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IPageDetailService : IDisposable
    {
        /// <summary>
        /// Get All Pages Details
        /// </summary>
        /// <returns></returns>
        Task<List<PageDto>> GetAllPagesDetail();

        /// <summary>
        /// Save Page Permissions for user
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task AddEditUserPagePermissionDetail(List<PageDto> pages, string userid);
        /// <summary>
        /// Get All User Permitted Pages
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<PageDto>> GetPermitedPages(string userId);
        /// <summary>
        /// Delete Page Permission
        /// </summary>
        /// <param name="userId"></param>
        void DeletePagePermission(string userId);
    }
}
