using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class ComponentRequestDto
    {
        public ComponentRequestDto(int teamid,int size = 15, int page=1 )
        {
            TeamId = teamid;
            Page = page;
            PageSize = size;
        }
        public int TeamId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        
    }

    public class CreateLongTermTeamRequestDto
    {
        public string UserId  { get; set; }
        public string TeamData { get; set; }
        public int TeamId  { get; set; }
        public string BrandName  { get; set; }
        public IFormFile IconFile { get; set; }

    }
}
