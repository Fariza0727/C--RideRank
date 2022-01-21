using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RR.AdminData;
using RR.AdminMapper;
using RR.Core;
using RR.Dto;
using RR.Repo;
using RR.StaticData;

namespace RR.AdminService
{
    public class PictureManagerService : IPictureManagerService
    {

        #region Constructor 

        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;
        private readonly IRepository<RiderManager, RankRideAdminContext> _repoRiderManager;
        private readonly AppSettings _appSettings;
        private readonly IRepository<PicuresManager, RankRideAdminContext> _repoPictures;

        public PictureManagerService(
            IRepository<Rider, RankRideStaticContext> repoRider, 
            IRepository<Bull, RankRideStaticContext> repoBull,
            IRepository<RiderManager, RankRideAdminContext> repoRiderManager,
            IOptions<AppSettings> appSettings,
            IRepository<PicuresManager, RankRideAdminContext> repoPictures)
        {
            _repoRider = repoRider;
            _repoBull = repoBull;
            _appSettings = appSettings.Value;
            _repoPictures = repoPictures;
            _repoRiderManager = repoRiderManager;
        }

        #endregion


        /// <summary>
        /// Get All News
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Record </param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<PictureManagerLiteDto>, int>> GetAllPictures(int start, int length, int column, string searchStr = "", string sort = "", bool isBull = false)
        {
            int count = 0;
            var predicate = PredicateBuilder.True<PicuresManager>();

            if (isBull)
                predicate = predicate.And(d => d.BullId > 0 && (searchStr == "" || d.BullName.Contains(searchStr.ToLower())));
            else
                predicate = predicate.And(d => d.RiderId > 0 && (searchStr == "" || d.RiderName.Contains(searchStr.ToLower())));


            var pictures = _repoPictures
                .Query()
                .Filter(predicate);

            return await Task.FromResult(new Tuple<IEnumerable<PictureManagerLiteDto>, int>(pictures
                    .GetPage(start, length, out count).Select(y => new PictureManagerLiteDto
                    {
                        Id = y.Id,
                        BullId = y.BullId, 
                        BullName = y.BullName,
                        RiderId = y.RiderId,
                        RiderName = y.RiderName,
                        BullPicture = setFullPath(y.BullPicture),
                        RiderPicture = setFullPath(y.RiderPicture)

                    }), count));
        }

        public async Task<PictureManagerLiteDto> AddEditPicture(PictureManagerDto pictureManager)
        {

            var entity = new PicuresManager();
            if (pictureManager?.Id > 0)
            {
                entity = _repoPictures.Query().Filter(r => r.Id == pictureManager.Id).Get().SingleOrDefault();
                if (entity != null)
                {
                    entity.BullPicture = pictureManager.BullPicture;
                    entity.RiderPicture = pictureManager.RiderPicture;
                    await _repoPictures.UpdateAsync(entity);
                }
            }
            else
            {
                entity.BullId = pictureManager.BullId;
                entity.BullPicture = pictureManager.BullPicture;
                entity.RiderId = pictureManager.RiderId;
                entity.RiderPicture = pictureManager.RiderPicture;
                entity.BullName = _repoBull.Query().Filter(r => r.BullId == pictureManager.BullId).Get().SingleOrDefault()?.Name;
                entity.RiderName = _repoRider.Query().Filter(r => r.RiderId == pictureManager.RiderId).Get().SingleOrDefault()?.Name;
                await _repoPictures.InsertAsync(entity);

            }

            var model = PictureManagerMapper.MapLiteDto(entity);
            model.BullPicture = setFullPath(model.BullPicture);
            return await Task.FromResult(model);

        }

        public async Task DeletePicture(long id)
        {
            await _repoPictures.DeleteAsync(id);
        }

        public void Dispose()
        {
            if (_repoRider != null)
                _repoRider.Dispose();

            if (_repoBull != null)
                _repoBull.Dispose();

            if (_repoPictures != null)
                _repoPictures.Dispose();
        }

        public Task<IEnumerable<PictureManagerLiteDto>> GetPictures()
        {
            return Task.FromResult(PictureManagerMapper.Map(_repoPictures.Query().Get()));
        }

        public Task<PictureManagerDto> GetPicture(long id)
        {

            var model = PictureManagerMapper.MapDto(_repoPictures.Query().Filter(r => r.Id == id).Get().SingleOrDefault());
            model.RiderPicture = setFullPath(model.RiderPicture);
            model.BullPicture = setFullPath(model.BullPicture);
            return Task.FromResult(model);


            
        }

        private string setFullPath(string pic)
        {
            if(!string.IsNullOrEmpty(pic))
                return string.Concat(_appSettings.MainSiteURL, "images/riderbulls/", pic);

            return "";
        }

        public Task<Tuple<List<int>, List<int>>> AddRiderBullPictursIds()
        {
            var ridersIds = _repoPictures.Query().Filter(r => r.RiderId > 0).Get().Select(r=>r.RiderId??0).ToList();
            var bullIds = _repoPictures.Query().Filter(r => r.BullId > 0).Get().Select(r => r.BullId??0).ToList();
            return Task.FromResult(new Tuple<List<int>, List<int>>(ridersIds, bullIds));

        }

        public async Task<string> GetBullPic(int BullId, string baseUrl = "")
        {
            var bull_ = _repoBull.Query().Filter(r => r.Id == BullId || r.BullId == BullId).Get().FirstOrDefault();
            string PicPath = "images/RR/New-logo.png";
            if (bull_ != null)
            {
                var bullPic = _repoPictures.Query().Filter(r => r.BullId == bull_.BullId).Get().FirstOrDefault();
                if (bullPic != null && !string.IsNullOrEmpty(bullPic.BullPicture))
                    PicPath = string.Concat("images/riderbulls/", bullPic.BullPicture);
            }
            if (!string.IsNullOrEmpty(baseUrl))
            {
                PicPath = string.Concat(baseUrl, PicPath);
            }

            return await Task.FromResult(string.Concat(_appSettings.MainSiteURL,PicPath));
        }

        public async Task<string> GetRiderPic(int RiderId, string baseUrl = "")
        {
            var rider_ = _repoRider.Query().Filter(r => r.Id == RiderId || r.RiderId == RiderId).Get().FirstOrDefault();
            string PicPath = "images/RR/New-logo.png";

            if (rider_ != null)
            {
                var riderPic = _repoPictures.Query().Filter(r => r.RiderId == rider_.RiderId).Get().FirstOrDefault();
                if (riderPic != null && !string.IsNullOrEmpty(riderPic.RiderPicture))
                    PicPath = string.Concat("images/riderbulls/", riderPic.RiderPicture);

            }
            if (!string.IsNullOrEmpty(baseUrl))
            {
                PicPath = string.Concat(baseUrl, PicPath);
            }
            return await Task.FromResult(string.Concat(_appSettings.MainSiteURL, PicPath));
        }

        public Task<RidermanagerDto> GetRiderManager(int RiderId)
        {
            RidermanagerDto model = new RidermanagerDto();
            if (RiderId > 0)
            {

                try
                {
                    model.RiderId = RiderId;
                    model.SocialLinks = _repoRiderManager.Query().Filter(d => d.RiderId == RiderId).Get().Select(r => new RiderSocialLinksDto
                    {
                        Id = r.Id,
                        Icon = r.Icon,
                        Sociallink = r.Sociallink,
                        Type = (SocialType)Enum.Parse(typeof(SocialType), r.Type)
                    }).ToList();
                }
                catch (Exception ed)
                {

                    
                }
            }
            return Task.FromResult(model);
        }

        public Task<RidermanagerDto> AddEditRiderManager(RidermanagerDto rider, int riderid)
        {
            var ids = rider.SocialLinks.Select(d => d.Id);
            var deleteItems = _repoRiderManager.Query().Filter(d => d.RiderId == riderid && !ids.Contains(d.Id)).Get();
            
            var newlinks = rider.SocialLinks.Where(d=>!string.IsNullOrEmpty(d.Sociallink) && d.Id == 0).Select(d => new RiderManager
            {
                Type = d.Type.ToString(),
                Sociallink = new UriBuilder(d.Sociallink).Uri.AbsoluteUri,
                RiderId = riderid,
                Icon = string.Concat($"fa fa-{d.Type.ToString().ToLower()}")
            }).ToList();
            var exist = rider.SocialLinks.Where(d => !string.IsNullOrEmpty(d.Sociallink) && d.Id > 0).Select(d => new RiderManager
            {
                Id = d.Id,
                Type = d.Type.ToString(),
                Sociallink = new UriBuilder(d.Sociallink).Uri.AbsoluteUri,
                RiderId = riderid,
                Icon = string.Concat($"fa fa-{d.Type.ToString().ToLower()}")
            }).ToList();

            _repoRiderManager.DeleteCollection(deleteItems.ToList());
            Thread.Sleep(500);
            _repoRiderManager.InsertCollection(newlinks);
            Thread.Sleep(500);
            _repoRiderManager.UpdateCollection(exist);

            return Task.FromResult(rider);
        }
    }
}
