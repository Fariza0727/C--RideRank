using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public class VideoSliderDto
    {
        public int Id { get; set; }
        public string VideoPath { get; set; }
        public string VideoUrl { get; set; }
        public string VideoThumb { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
        public bool IsUrl { get; set; }
    }
}
