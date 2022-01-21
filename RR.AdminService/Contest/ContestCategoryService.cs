using RR.AdminData;
using RR.AdminMapper;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class ContestCategoryService : IContestCategoryService
    {
        #region Constructor

        private readonly IRepository<ContestCategory, RankRideAdminContext> _repoContestCategory;
        public ContestCategoryService(IRepository<ContestCategory, RankRideAdminContext> repoContestCategory)
        {
            _repoContestCategory = repoContestCategory;
        }

        #endregion

        /// <summary>
        /// Get Contest Categories
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DropDownDto>> GetContestCategories()
        {
            var contestCategories = _repoContestCategory.Query().Filter(x => x.CategoryName != "Private Contest").Get();
            return await Task.FromResult(ContestCategoryMapper.Map(contestCategories));
        }

        /// <summary>
        /// Dipose Contest Category Service
        /// </summary>
        public void Dispose()
        {
            if (_repoContestCategory != null)
            {
                _repoContestCategory.Dispose();
            }
        }
    }
}
