using Microsoft.Extensions.Options;
using RR.AdminData;
using RR.Core;
using RR.Repo;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
    public class BullRiderPicturesService : IBullRiderPicturesService
    {
        private readonly IRepository<PicuresManager, RankRideAdminContext> _repoPictures;
        private readonly IRepository<Bull, RankRideStaticContext> _repoBull;
        private readonly IRepository<Rider, RankRideStaticContext> _repoRider;
        private readonly AppSettings _appSettings;

        public BullRiderPicturesService(
            IRepository<PicuresManager, RankRideAdminContext> repoPictures,
            IRepository<Bull, RankRideStaticContext> repoBull,
            IRepository<Rider, RankRideStaticContext> repoRider,
            IOptions<AppSettings> appSettings)
        {
            _repoPictures = repoPictures;
            _repoBull = repoBull;
            _repoRider = repoRider;
            _appSettings = appSettings.Value;
        }

        public void Dispose()
        {
            if (_repoPictures != null)
                _repoPictures.Dispose();
        }

        public async Task<string> GetBullPic(int BullId, string baseUrl = "")
        {
            var bull_ = _repoBull.Query().Filter(r => r.Id == BullId || r.BullId == BullId).Get().FirstOrDefault();
            string PicPath = "images/home/Bull-logo.png";
            if (bull_ != null && _appSettings.ShowDynamicBullRiderImg)
            {
                var bullPic = _repoPictures.Query().Filter(r => r.BullId == bull_.BullId || r.BullId == bull_.Id).Get().FirstOrDefault();
                if (bullPic != null && !string.IsNullOrEmpty(bullPic.BullPicture))
                    PicPath = string.Concat("images/riderbulls/", bullPic.BullPicture);
            }
            if (!string.IsNullOrEmpty(baseUrl))
            {
                PicPath = string.Concat(baseUrl, PicPath);
            }

            return await Task.FromResult(PicPath);
        }


        public async Task<string> GetRiderPic(int RiderId, string baseUrl = "")
        {
            var rider_ = _repoRider.Query().Filter(r => r.Id == RiderId || r.RiderId == RiderId).Get().FirstOrDefault();
            string PicPath = "images/home/Rider-logo.png";

            if (rider_ != null && _appSettings.ShowDynamicBullRiderImg)
            {
                var riderPic = _repoPictures.Query().Filter(r => r.RiderId == rider_.RiderId || r.RiderId == rider_.Id).Get().FirstOrDefault();
                if (riderPic != null && !string.IsNullOrEmpty(riderPic.RiderPicture))
                    PicPath = string.Concat("images/riderbulls/", riderPic.RiderPicture);

            }
            if (!string.IsNullOrEmpty(baseUrl))
            {
                PicPath = string.Concat(baseUrl, PicPath);
            }
            return await Task.FromResult(PicPath);
        }
    }
}
