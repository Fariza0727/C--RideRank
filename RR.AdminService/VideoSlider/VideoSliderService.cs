using Microsoft.Extensions.Options;
using RR.AdminData;
using RR.AdminMapper;
using RR.Core;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public class VideoSliderService : IVideoSliderService
    {
        #region Constructor 

        private readonly IRepository<VideoSlider, RankRideAdminContext> _repoVSlider;
        private readonly AppSettings _appSettings;

        public VideoSliderService(IRepository<VideoSlider, RankRideAdminContext> repoVSlider,
             IOptions<AppSettings> appSettings)
        {
            _repoVSlider = repoVSlider;
            _appSettings = appSettings.Value;
        }

        #endregion


        public async Task AddEditVSlider(VideoSliderDto sliderDto)
        {
            
                VideoSlider newsEntity = new VideoSlider();
                string createdFileName = "";
            if (!sliderDto.IsUrl)
            {
                if (sliderDto.File?.Length > 0)
                {
                    TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                    createdFileName = span.TotalSeconds + sliderDto.File.FileName.Substring(sliderDto.File.FileName.LastIndexOf("."));

                    var path1 = Path.Combine(_appSettings.SaveasBannersWeb, createdFileName);
                    using (var stream1 = new FileStream(path1, FileMode.Create))
                    {
                        await sliderDto.File.CopyToAsync(stream1);
                    }
                    newsEntity.VideoPath = createdFileName;

                }
            }

                if (sliderDto.Id > 0)
                {
                    newsEntity = _repoVSlider.Query().Filter(x => x.Id == sliderDto.Id).Get().FirstOrDefault();
                
                    if (!string.IsNullOrEmpty(createdFileName))
                    {
                        newsEntity.VideoPath = createdFileName;
                    }
                    if (!string.IsNullOrEmpty(sliderDto.VideoUrl))
                    {
                        newsEntity.VideoUrl = sliderDto.VideoUrl?.Replace("watch?v=", "embed/");
                    }
                    if (!string.IsNullOrEmpty(sliderDto.VideoThumb))
                    {
                        newsEntity.VideoThumb = sliderDto.VideoThumb;
                    }

                if (!sliderDto.IsUrl)
                    newsEntity.VideoUrl = "";
                else
                    newsEntity.VideoPath = "";


                newsEntity.IsActive = sliderDto.IsActive;
                    
                }
                else
                {
                    newsEntity.VideoPath = createdFileName;
                    newsEntity.VideoThumb = sliderDto.VideoThumb;
                    newsEntity.VideoUrl = sliderDto.VideoUrl?.Replace("watch?v=", "embed/");
            }

                if (sliderDto.Id > 0)
                    await _repoVSlider.UpdateAsync(newsEntity);
                else
                    await _repoVSlider.InsertGraphAsync(newsEntity);
            
        }


        public async Task<Tuple<IEnumerable<VideoSliderDto>, int>> GetAllVSlider(int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;
            var predicate = PredicateBuilder.True<VideoSlider>()
           .And(x => (searchStr == "" || x.VideoPath.ToLower().Contains(searchStr.ToLower())
           || x.Id.ToString().Contains(searchStr.ToLower()) || x.VideoUrl.ToLower().Contains(searchStr.ToLower())));

            var news = _repoVSlider
                .Query()
                .Filter(predicate);

            if (1 == column)
            {
                news = (sort == "desc" ? news.OrderBy(x => x.OrderByDescending(xx => xx.IsActive)) : news.OrderBy(x => x.OrderBy(xx => xx.IsActive)));
            }
            return await Task.FromResult(new Tuple<IEnumerable<VideoSliderDto>, int>(news
                    .GetPage(start, length, out count).Select(y => new VideoSliderDto
                    {
                        Id = y.Id,
                        Title = y.Title,
                        VideoPath = string.Concat(_appSettings.MainSiteURL,_appSettings.BannerSharedPath, y.VideoPath),
                        VideoThumb = y.VideoThumb,
                        VideoUrl = y.VideoUrl,
                        IsActive = y.IsActive,
                        Description = y.Description,
                        IsUrl = !string.IsNullOrEmpty(y.VideoUrl)
                        
                    }), count));
        }


        public async Task<VideoSliderDto> GetVSliderById(int Id)
        {
            var newsData = _repoVSlider.Query().Filter(x => x.Id == Id).Get().FirstOrDefault();
            return await Task.FromResult(VideoSliderMapper.Map(newsData));
        }


        public void DeleteVSlider(int Id)
        {
            _repoVSlider.Delete(Id);
        }


        public void Dispose()
        {
            if (_repoVSlider != null)
            {
                _repoVSlider.Dispose();
            }
        }

        public async Task UpdateStatus(int Id, bool status)
        {
            var vslider = _repoVSlider.Query().Filter(r => r.Id == Id).Get().FirstOrDefault();
            vslider.IsActive = status;
            await _repoVSlider.UpdateAsync(vslider);
        }
    }
}
