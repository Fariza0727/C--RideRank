using Microsoft.AspNetCore.Mvc.Rendering;
using RR.AdminData;
using RR.Core;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class PointTableService : IPointTableService
    {
        #region Constructor

        private readonly IRepository<PointTable, RankRideAdminContext> _repoPointTable;

        public PointTableService(IRepository<PointTable, RankRideAdminContext> repoPointTable)
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
        /// Add/Edit PointTable Data
        /// </summary>
        /// <param name="pointTableDto"></param>
        /// <returns></returns>
        public async Task<long> AddEditPointTableData(PointTableDto pointTableDto)
        {
            long ID = 0;
            try
            {
                PointTable data = new PointTable();
                data = _repoPointTable.Query().Filter(x => x.Id == pointTableDto.Id).Get().FirstOrDefault();
                if (data == null)
                {
                    data = new PointTable
                    {
                        CreatedDate = DateTime.Now,
                        PointFor = pointTableDto.PointFor,
                        Text = pointTableDto.Key,
                        UpdatedDate = DateTime.Now,
                        Value = pointTableDto.Value.Replace("\r\n", "<br>")
                    };
                    await _repoPointTable.InsertAsync(data);
                    ID = data.Id;
                }
                else
                {
                    data.PointFor = pointTableDto.PointFor;
                    data.Text = pointTableDto.Key;
                    data.UpdatedDate = DateTime.Now;
                    data.Value = pointTableDto.Value.Replace("\r\n", "<br>");
                    await _repoPointTable.UpdateAsync(data);
                    ID = data.Id;
                }
            }
            catch (Exception ex)
            {
            }

            return await Task.FromResult(ID);

        }

        /// <summary>
        /// Returns Point Table Data By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PointTableDto> GetPointDataByID(long id)
        {
            PointTableDto tableDto = new PointTableDto();
            tableDto.PointType = PointTypeList();
            try
            {
                var data = _repoPointTable.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
                if (data != null)
                {
                    tableDto.Id = data.Id;
                    tableDto.Key = data.Text;
                    tableDto.PointFor = data.PointFor;
                    tableDto.Value = data.Value.Replace("<br>", "\r\n");
                }
            }
            catch (Exception)
            {
            }
            return await Task.FromResult(tableDto);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<long> DeletePointDataByID(long id)
        {
            long ID = 0;

            try
            {
                var data = _repoPointTable.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
                if (data != null)
                {
                    await _repoPointTable.DeleteAsync(data);
                    ID = 1;
                }
            }
            catch (Exception ex)
            {
            }
            return await Task.FromResult(ID);
        }

        /// <summary>
        /// Return List Of Point Category
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> PointTypeList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Insert(0, new SelectListItem { Text = "Select Type", Value = "0" });
            selectListItems.Insert(0, new SelectListItem { Text = "Rider", Value = "1" });
            selectListItems.Insert(0, new SelectListItem { Text = "Bull", Value = "2" });
            return selectListItems;
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
