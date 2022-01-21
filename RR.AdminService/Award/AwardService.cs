using log4net;
using Microsoft.EntityFrameworkCore;
using RR.AdminData;
using RR.AdminMapper;
using RR.Core;
using RR.Dto.Award;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class AwardService : IAwardService
    {
        #region constructor

        private readonly IRepository<Award, RankRideAdminContext> _repoAward;
        private readonly IRepository<AwardType, RankRideAdminContext> _repoAwardType;
        private readonly IRepository<ContestWinner, RankRideAdminContext> _repoContestWinner;
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AwardService(IRepository<Award, RankRideAdminContext> repoAward,
            IRepository<AwardType, RankRideAdminContext> repoAwardType,
            IRepository<ContestWinner, RankRideAdminContext> repoContestWinner)
        {
            _repoAward = repoAward;
            _repoAwardType = repoAwardType;
            _repoContestWinner = repoContestWinner;
        }

        #endregion

        /// <summary>
        /// Get all awards.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="column"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<AwardDto>, int>> GetAllAward(int start, int length, int column, string searchStr = "", string sort = "")
        {
            try
            {
                int count = 0;
                var predicate = PredicateBuilder.True<Award>()
               .And(x => (searchStr == "" || x.AwardType.Name.ToLower().Contains(searchStr.ToLower()) || x.Message.ToLower().Contains(searchStr.ToLower()) || x.Token.ToLower().Contains(searchStr.ToLower())))
               .And(x => x.Message != "Not Required")
               .And(x => x.IsDelete == false);

                var awards = _repoAward
                    .Query()
                    .Includes(u => u.Include(uu => uu.AwardType))
                    .Filter(predicate);

                switch (column)
                {
                    case 0:
                        awards = (sort == "desc" ? awards.OrderBy(x => x.OrderByDescending(xx => xx.AwardType.Name)) : awards.OrderBy(x => x.OrderBy(xx => xx.AwardType.Name)));
                        break;
                    case 1:
                        awards = (sort == "desc" ? awards.OrderBy(x => x.OrderByDescending(xx => xx.Message)) : awards.OrderBy(x => x.OrderBy(xx => xx.Message)));
                        break;
                    default:
                        awards = (sort == "desc" ? awards.OrderBy(x => x.OrderByDescending(xx => xx.CreatedOn)) : awards.OrderBy(x => x.OrderBy(xx => xx.CreatedOn)));
                        break;
                }

                return await Task.FromResult(new Tuple<IEnumerable<AwardDto>, int>(awards
                        .GetPage(start, length, out count).Select(y => new AwardDto
                        {
                            Id = y.Id,
                            AwardTypeId = y.AwardTypeId,
                            AwardTypeName = y.AwardType.Name,
                            Token = y.Token,
                            Message = y.Message,
                            CreatedBy = y.CreatedBy,
                            CreatedDate = y.CreatedOn,
                            Image = y.Image
                        }), count));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Add/Edit awards.
        /// </summary>
        /// <param name="awardDto"></param>
        /// <returns></returns>
        public async Task AddEditAward(AwardDto awardDto)
        {
            try
            {
                Award award = new Award();
                if (awardDto.Id > 0)
                {
                    award = _repoAward.Query().Filter(x => x.Id == awardDto.Id).Get().FirstOrDefault();
                    if (award != null)
                    {
                        award.AwardTypeId = awardDto.AwardTypeId;
                        award.Image = awardDto.Image;
                        award.Message = awardDto.Message;
                        award.Token = awardDto.Token;
                        await _repoAward.UpdateAsync(award);
                    }
                }
                else
                {
                    award.AwardTypeId = awardDto.AwardTypeId;
                    award.Image = awardDto.Image;
                    award.Message = awardDto.Message;
                    award.Token = awardDto.Token;
                    award.CreatedBy = awardDto.CreatedBy;
                    award.CreatedOn = DateTime.Now;
                    award.IsDelete = false;
                    await _repoAward.InsertAsync(award);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Get award detail by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AwardDto> GetAward(long id)
        {
            try
            {
                var result = _repoAward
                .Query()
                    .Filter(x => x.Id == id)
                    .Get()
                    .SingleOrDefault();
                return await Task.FromResult(AwardMapper.MapDto(result));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Awards for Dropdown 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AwardDto>> GetAllAwardsdropdown()
        {
            try
            {
                var result = _repoAward
                .Query()
                    //.Filter(x => x.AwardTypeId == id)
                    .Get().Where(x => x.AwardTypeId != 2 && x.AwardTypeId != 3 && x.IsDelete == false)
                    ;
                return await Task.FromResult(AwardMapper.Map(result));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Awards by Awardtypeid for Dropdown 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AwardDto>> GetAwards(long id)
        {
            try
            {
                var result = _repoAward
                .Query()
                    .Filter(x => x.AwardTypeId == id && x.AwardTypeId != 2 && x.AwardTypeId != 3)
                    .Get()
                    ;
                return await Task.FromResult(AwardMapper.Map(result));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
        }
        public int GetAwardtypeid(long awardId)
        {

            var result = _repoAward
            .Query()
                .Filter(x => x.Id == awardId)
                .Get()
                .Select(x => x.AwardTypeId)
                .SingleOrDefault()
                ;
            return result;

        }

        /// <summary>
        /// Delete award by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAward(long id)
        {
            try
            {
                var award = _repoAward.Query().Filter(x => x.Id == id).Get().FirstOrDefault();
                if (award != null)
                {
                    award.IsDelete = true;
                    await _repoAward.UpdateAsync(award);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

        }
        /// <summary>
        /// Get AwardType id by awardid 
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetAwardid(int awardtypeid)
        {
            try
            {
                var awardTypes = _repoAward
                    .Query()
                    //.Filter(x => x.AwardTypeId == awardtypeid)
                    .Get();

                var result = awardTypes
                .Where(x => x.AwardTypeId == awardtypeid).Select(x => x.Id
                    ).SingleOrDefault();
                return await Task.FromResult(result);

            }
            catch (Exception ex)
            {

            }
            return await Task.FromResult(5);
        }

        public async Task<IEnumerable<AwardDto>> GetAwards(int awardTypeId)
        {
            try
            {
                var awards = _repoAward
                    .Query()
                    .Filter(x => x.AwardTypeId == awardTypeId && x.IsDelete == false)
                    .Get();

                return await Task.FromResult(AwardMapper.Map(awards));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// returns list of marchendise type awards.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AwardDto>> GetMerchandiseAward()
        {
            try
            {
                var awardsList = from awards in _repoAward
                    .Query()
                    .Filter(x => x.IsDelete == false)
                    .Get()
                                 join awardType in _repoAwardType.Query().Get()
                                  on awards.AwardTypeId equals awardType.Id
                                 where awardType.Name == "Merchandise" && awardType.IsDelete == false
                                 select awards;

                return await Task.FromResult(AwardMapper.Map(awardsList));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// returns list of other type awards.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AwardDto>> GetOtherAward()
        {
            try
            {
                var awardsList = from awards in _repoAward
                    .Query()
                    .Filter(x => x.IsDelete == false)
                    .Get()
                                 join awardType in _repoAwardType.Query().Get()
                                  on awards.AwardTypeId equals awardType.Id
                                 where awardType.Name != "Merchandise" && awardType.Name != "Cash" && awardType.Name != "Token" && awardType.IsDelete == false
                                 select awards;

                return await Task.FromResult(AwardMapper.Map(awardsList));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// check award associate with Contest
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> CheckAwardAssociateWithContest(int Id)
        {
            bool isAssociate = true;
            try
            {
                var contests = _repoContestWinner.Query().Filter(x => x.Marchendise.Value == Id || x.OtherReward.Value == Id).Get().ToList();
                if (contests.Count == 0)
                    isAssociate = false;
            }
            catch (Exception ex)
            {
            }
            return await Task.FromResult(isAssociate);
        }

        public void Dispose()
        {
            if (_repoAward != null)
            {
                _repoAward.Dispose();
            }
        }
    }
}
