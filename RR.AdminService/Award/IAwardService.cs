using RR.Dto.Award;
using RR.Dto.AwardType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IAwardService : IDisposable
    {
        /// <summary>
        /// Get all awards
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns>Lust of created awards.</returns>
        Task<Tuple<IEnumerable<AwardDto>, int>> GetAllAward(int start, int length, int column, string searchStr = "", string sort = "");

        /// <summary>
        /// Add/Edit award
        /// </summary>
        /// <param name="awardDto"></param>
        /// <returns></returns>
        Task AddEditAward(AwardDto awardDto);

        /// <summary>
        /// Get award by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AwardDto> GetAward(long id);

        Task<IEnumerable<AwardDto>> GetAwards(long id);

        Task<IEnumerable<AwardDto>> GetAllAwardsdropdown();

        Task<long> GetAwardid(int awardtypeid);
        int GetAwardtypeid(long awardId);
        /// <summary>
        /// delete award by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAward(long id);

        Task<IEnumerable<AwardDto>> GetAwards(int awardTypeId);

        /// <summary>
        /// returns list of marchendise type awards.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AwardDto>> GetMerchandiseAward();

        /// <summary>
        /// returns list of other type awards.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AwardDto>> GetOtherAward();

        /// <summary>
        /// check award associate with Contest
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> CheckAwardAssociateWithContest(int Id);
    }
}
