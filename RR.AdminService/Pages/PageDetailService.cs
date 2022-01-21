using RR.AdminData;
using RR.AdminMapper;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class PageDetailService : IPageDetailService
    {
        #region Constructor
        private readonly IRepository<PageDetail, RankRideAdminContext> _repoPageDetail;
        private readonly IRepository<PagePermission, RankRideAdminContext> _repoPagePermission;
        public PageDetailService(IRepository<PageDetail, RankRideAdminContext> repoPageDetail, IRepository<PagePermission, RankRideAdminContext> pagePermission)
        {
            _repoPageDetail = repoPageDetail;
            _repoPagePermission = pagePermission;
        }
        #endregion

        /// <summary>
        /// Get All Pages Details
        /// </summary>
        /// <returns></returns>
        public async Task<List<PageDto>> GetAllPagesDetail()
        {
            var page = _repoPageDetail.Query().Get().OrderBy(x => x.PageName);

            return await Task.FromResult(PageDetailMapper.Map(page.ToList()));
        }

        /// <summary>
        /// Save Page Permissions for user
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task AddEditUserPagePermissionDetail(List<PageDto> pages, string userid)
        {
            var permissionPages = _repoPagePermission.Query().Get();
            if (permissionPages.Any(x => x.UserId == userid))
            {
                try
                {
                    foreach (var item in pages)
                    {

                        if (item.Selected && permissionPages.Any(x => x.PageId.ToString() == item.Id && x.UserId == userid))
                        {
                            //Already in Database 
                        }
                        else if (item.Selected)
                        {
                            //Added new In Database
                            await _repoPagePermission.InsertAsync(new PagePermission { UserId = userid, PageId = Convert.ToInt16(item.Id), CreatedDate = DateTime.Now });
                        }
                        else if (item.Selected == false && permissionPages.Any(x => x.PageId.ToString() == item.Id && x.UserId == userid))
                        {
                            //Delete if not selected and in Database
                            var pagePermission = _repoPagePermission.Query()
                                .Filter(x => x.PageId == Convert.ToInt32(item.Id) && x.UserId == userid)
                                .Get().SingleOrDefault();
                            await _repoPagePermission.DeleteAsync(pagePermission.Id);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            else
            {
                try
                {
                    foreach (var item in pages)
                    {
                        if (item.Selected)
                            await _repoPagePermission.InsertAsync(new PagePermission { UserId = userid, PageId = Convert.ToInt16(item.Id), CreatedDate = DateTime.Now });
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }



        }
        /// <summary>
        /// Get All User Permitted Pages
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<PageDto>> GetPermitedPages(string userId)
        {
            var pages = _repoPageDetail.Query().Get().OrderBy(x => x.PageName);
            var permitedPages = _repoPagePermission.Query().Get().Where(x => x.UserId == userId);
            List<PageDto> permittedPageDetail = PageDetailMapper.Map(pages.ToList());

            foreach (var item in permittedPageDetail)
            {
                try
                {
                    if (permitedPages.Any(x => x.PageId == Convert.ToInt32(item.Id)))
                    {
                        item.Selected = true;
                    }
                }
                catch (Exception ex)
                {
                    string exe = ex.ToString();
                    throw;
                }

            }
            return await Task.FromResult(permittedPageDetail);
        }

        public void DeletePagePermission(string userId)
        {
            try
            {
                var pages = _repoPagePermission.Query().Get().Where(x => x.UserId == userId);

                _repoPagePermission.DeleteCollection(pages.ToList());

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public void Dispose()
        {
            if (_repoPageDetail != null)
            {
                _repoPageDetail.Dispose();
            }
            if (_repoPagePermission != null)
            {
                _repoPagePermission.Dispose();
            }
        }
    }
}
