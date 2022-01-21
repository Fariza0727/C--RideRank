using RR.AdminData;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{

    public static class VideoSliderMapper
    {
        /// <summary>
        /// Map Dto
        /// </summary>
        /// <param name="sliders"></param>
        /// <returns></returns>
        public static IEnumerable<VideoSliderDto> Map(IEnumerable<VideoSlider> sliders)
        {
            return sliders.Select(p => Map(p));
        }

        
        public static VideoSliderDto Map(VideoSlider slider)
        {
            return new VideoSliderDto
            {
                Id = slider.Id,
                Description = slider.Description,
                IsActive = slider.IsActive,
                Title = slider.Title,
                VideoPath = slider.VideoPath,
                VideoThumb = slider.VideoThumb,
                VideoUrl = slider.VideoUrl,
                IsUrl = !string.IsNullOrEmpty(slider.VideoUrl)
            };
        }
    }
}
