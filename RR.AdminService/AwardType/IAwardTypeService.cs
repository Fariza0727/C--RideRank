using RR.Dto.AwardType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IAwardTypeService : IDisposable
    {
        /// <summary>
        /// Get all award types.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<AwardTypeDto>, int>> GetAllAwardType(int start, int length, string searchStr = "", string sort = "");

        /// <summary>
        /// Add/Edit award type.
        /// </summary>
        /// <param name="awardTypeDto"></param>
        /// <returns></returns>
        Task AddEditAwardType(AwardTypeDto awardTypeDto);

        /// <summary>
        /// Get award type detail by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AwardTypeDto> GetAwardType(int id);

        /// <summary>
        /// Delete award type by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAwardType(int id);

        /// <summary>
        /// Get all award types.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AwardTypeDto>> GetAwardTypes();

        /// <summary>
        /// check award type associate with award
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> CheckAwardTypeAssociateWithAward(int Id);
    }
}
