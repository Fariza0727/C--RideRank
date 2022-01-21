using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class VideoSlider
    {
        public int Id { get; set; }
        public string VideoPath { get; set; }
        public string VideoUrl { get; set; }
        public string VideoThumb { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
