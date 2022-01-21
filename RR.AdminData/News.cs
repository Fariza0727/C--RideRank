using System;
using System.Collections.Generic;

namespace RR.AdminData
{
    public partial class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PicPath { get; set; }
        public string VideoUrl { get; set; }
        public string VideoPath { get; set; }
        public DateTime NewsDate { get; set; }
        public string NewsContent { get; set; }
        public string NewsTag { get; set; }
        public bool IsPopular { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
