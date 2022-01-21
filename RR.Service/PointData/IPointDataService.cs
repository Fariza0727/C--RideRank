using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface IPointDataService:IDisposable
    {
        /// <summary>
        /// Returns List Of Point Table Data
        /// </summary>
        /// <returns></returns>
        Task<Tuple<IEnumerable<PointTableDto>, int>> GetAllPointTableData();
    }
}
