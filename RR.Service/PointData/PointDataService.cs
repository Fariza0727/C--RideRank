using RR.AdminData;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
    public class PointDataService: IPointDataService
    {
        #region Constructor

        private readonly IRepository<PointTable, RankRideAdminContext> _repoPointTable;

        public PointDataService(IRepository<PointTable, RankRideAdminContext> repoPointTable)
        {
            _repoPointTable = repoPointTable;
        }

        #endregion

        /// <summary>
        /// Returns List Of Point Table Data
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="column"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <param name="PointID"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<PointTableDto>, int>> GetAllPointTableData()
        {
            int count = 0;
            var data = _repoPointTable
                .Query().Get();
            var dataList = data.Select(y => new PointTableDto
            {
                Id = y.Id,
                PointFor = y.PointFor,
                PointForValue = y.PointFor == 1 ? "Rider" : y.PointFor == 2 ? "Bull" : "NA",
                Key = y.Text,
                Value = y.Value
            }).ToList();

            count = dataList.Count;

            return await Task.FromResult(new Tuple<IEnumerable<PointTableDto>, int>(dataList, count));
        }

        /// <summary>
        /// Dispose User Service
        /// </summary>
        public void Dispose()
        {
            if (_repoPointTable != null)
            {
                _repoPointTable.Dispose();
            }
        }
    }
}
