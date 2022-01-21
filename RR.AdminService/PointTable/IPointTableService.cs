using RR.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IPointTableService : IDisposable
    {
        /// <summary>
        /// Returns List Of Point Table Data
        /// </summary>
        /// <returns></returns>
        Task<Tuple<IEnumerable<PointTableDto>, int>> GetAllPointTableData();

        /// <summary>
        /// Add/Edit PointTable Data
        /// </summary>
        /// <param name="pointTableDto"></param>
        /// <returns></returns>
        Task<long> AddEditPointTableData(PointTableDto pointTableDto);

        /// <summary>
        /// Returns Point Table Data By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PointTableDto> GetPointDataByID(long id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<long> DeletePointDataByID(long id);
    }
}
