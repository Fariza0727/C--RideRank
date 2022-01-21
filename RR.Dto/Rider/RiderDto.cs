using Newtonsoft.Json;
using RR.Dto.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RR.Dto
{
     public class RiderDto
     {
          [JsonProperty("guyid")]
          public int Id { get; set; }
          [JsonProperty("riderid")]
          public int RiderId { get; set; }
          [JsonProperty("cwrp")]
          public int CWRP { get; set; }

          [Required(ErrorMessage = "Name is Required")]
          [JsonProperty("rider_name")]
          public string Name { get; set; }

          [JsonProperty("rider_avatar")]
          public string Avatar { get; set; }

          [Required(ErrorMessage = "Mounted is Required")]
          [JsonProperty("mounted")]
          public int Mounted { get; set; }

          [Required(ErrorMessage = "Rode is Required")]
          [JsonProperty("rode")]
          public int Rode { get; set; }

          [Required(ErrorMessage = "Streak is Required")]
          [JsonProperty("streak")]
          public decimal Streak { get; set; }

          [Required(ErrorMessage = "This Field is Required")]
          [JsonProperty("hand")]
          public string Hand { get; set; }

          [Required(ErrorMessage = "Ride Perc is Required")]
          [JsonProperty("rideperc")]
          public decimal? RidePerc { get; set; }

          [Required(ErrorMessage = "RidePrec Curent is Required")]
          [JsonProperty("rideperc_current")]
          public decimal? RidePrecCurent { get; set; }

          [Required(ErrorMessage = "Mounted Current is Required")]
          [JsonProperty("mounted_current")]
          public int? MountedCurrent { get; set; }

          [Required(ErrorMessage = "Rider Power is Required")]
          [JsonProperty("riderpower")]
          public decimal? RiderPower { get; set; }

          [Required(ErrorMessage = "Rider Power Current is Required")]
          [JsonProperty("riderpower_current")]
          public decimal? RiderPowerCurrent { get; set; }

          public int HeadShot { get; set; }

          public int WorldRanking { get; set; }

          public string CountryName { get; set; }

          public int Age { get; set; }

          public string Height { get; set; }

          public int Outs { get; set; }

          public int SeasonPoints { get; set; }

          public int SeasonMoney { get; set; }

          public string Background { get; set; }

          public string Statistics { get; set; }

          public string Video { get; set; }

          [JsonProperty("error")]
          public string Error { get; set; }

          public bool IsActive { get; set; }

          public bool IsDelete { get; set; }

          public int WorldRank { get; set; }
            public int WorldRankPoint { get; set; }
            public decimal RRTotalpoint { get; set; }

        public DateTime CreatedDate { get; set; }
          [JsonProperty("isAddedFavorite")]
          public bool IsAddedFavorite { get; set; }

         public bool isAddedInLongTermTeam { get; set; }
         public string LTTeamIcon { get; set; }
         public string LTTeamName { get; set; }
         public List<RiderEventDto> RiderEvents { get; set; }
         public RidermanagerDto RiderManager { get; set; }
    }

    public class RiderEventDto
    {
        public int Season { get; set; }
        public string Series { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public string EventTpe { get; set; }
        public string EventName { get; set; }
        public string Place { get; set; }
        public string Point {get; set; }
        public string Country { get; set; }
        public List<EventResultDto> EventReult { get; set; }
    }
    
}
