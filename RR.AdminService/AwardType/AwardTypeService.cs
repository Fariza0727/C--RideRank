using RR.AdminData;
using RR.AdminMapper;
using RR.Core;
using RR.Dto.AwardType;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class AwardTypeService : IAwardTypeService
    {
        #region constructor

        private readonly IRepository<AwardType, RankRideAdminContext> _repoAwardType;
        private readonly IRepository<Award, RankRideAdminContext> _repoAward;

        public AwardTypeService(IRepository<AwardType, RankRideAdminContext> repoAwardType,
            IRepository<Award, RankRideAdminContext> repoAward)
        {
            _repoAwardType = repoAwardType;
            _repoAward = repoAward;
        }

        #endregion

        /// <summary>
        /// Get all award types
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<AwardTypeDto>, int>> GetAllAwardType(int start, int length, string searchStr = "", string sort = "")
        {
            int count = 0;
            var predicate = PredicateBuilder.True<AwardType>()
           .And(x => (searchStr == "" || x.Name.ToLower().Contains(searchStr.ToLower())))
           .And(x => x.IsDelete == false);

            var awardTypes = _repoAwardType
                .Query()
                .Filter(predicate);

            if (sort == "desc")
            {
                awardTypes = awardTypes.OrderBy(x => x.OrderByDescending(xx => xx.Name));
            }
            else
            {
                awardTypes = awardTypes.OrderBy(x => x.OrderBy(xx => xx.Name));
            }

            return await Task.FromResult(new Tuple<IEnumerable<AwardTypeDto>, int>(awardTypes
                    .GetPage(start, length, out count).Select(y => new AwardTypeDto
                    {
                        Id = y.Id,
                        Name = y.Name
                    }), count));
        }

        /// <summary>
        /// Add/Edit award type.
        /// </summary>
        /// <param name="awardTypeDto"></param>
        /// <returns></returns>
        public async Task AddEditAwardType(AwardTypeDto awardTypeDto)
        {
            try
            {
                AwardType award = new AwardType();
                if (awardTypeDto.Id > 0)
                {
                    award = _repoAwardType.Query().Filter(x => x.Id == awardTypeDto.Id).Get().FirstOrDefault();
                    if (award != null)
                    {
                        award.Name = awardTypeDto.Name;
                        await _repoAwardType.UpdateAsync(award);
                    }
                }
                else
                {
                    award = _repoAwardType.Query().Filter(x => x.Name.ToLower() == awardTypeDto.Name.ToLower()).Get().FirstOrDefault();
                    if (award != null)
                    {
                        award.IsDelete = false;
                        await _repoAwardType.UpdateAsync(award);
                    }
                    else
                    {
                        award = new AwardType();
                        award.CreatedBy = awardTypeDto.CreatedBy;
                        award.Name = awardTypeDto.Name;
                        award.CreatedOn = DateTime.Now;
                        await _repoAwardType.InsertAsync(award);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Get award type detail by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AwardTypeDto> GetAwardType(int id)
        {
            var result = _repoAwardType
                .Query()
                    .Filter(x => x.Id == id)
                    .Get()
                    .SingleOrDefault();
            return await Task.FromResult(AwardTypeMapper.MapDto(result));
        }

        /// <summary>
        /// Delete award type by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAwardType(int id)
        {
            if (id > 3)
            {
                var award = _repoAwardType.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
                if (award != null)
                {
                    award.IsDelete = true;
                    await _repoAwardType.UpdateAsync(award);
                }
            }
        }

        /// <summary>
        /// Get all award types.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AwardTypeDto>> GetAwardTypes()
        {
            var awardTypes = _repoAwardType
                .Query()
                .Filter(x => x.IsDelete == false)
                .Get()
                .OrderBy(x => x.Name);

            return await Task.FromResult(AwardTypeMapper.Map(awardTypes));
        }

        /// <summary>
        /// check award type associate with award
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> CheckAwardTypeAssociateWithAward(int Id)
        {
            var isAssociate = true;

            try
            {
                var awards = _repoAward.Query().Filter(x => x.AwardTypeId == Id).Get().ToList();
                if (awards.Count == 0)
                    isAssociate = false;
            }
            catch (Exception ex)
            {
            }

            return await Task.FromResult(isAssociate);
        }

        public void Dispose()
        {
            if (_repoAwardType != null)
            {
                _repoAwardType.Dispose();
            }
        }
    }
}
