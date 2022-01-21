using Newtonsoft.Json;
using System;

namespace RR.Dto
{
    public class BullDto
    {
        /// <summary>
        /// Bull Id
        /// </summary>
        [JsonProperty("pbid")]
        public int Id { get; set; }

        /// <summary>
        /// Bull Brand
        /// </summary>
        [JsonProperty("bull_brand")]
        public string Brand { get; set; }

        /// <summary>
        /// Bull Name
        /// </summary>
        [JsonProperty("bull_name")]
        public string Name { get; set; }

        /// <summary>
        /// Bull Avatar
        /// </summary>
        [JsonProperty("bull_avatar")]
        public string Avatar { get; set; }

        /// <summary>
        /// Owner Name of Bull
        /// </summary>
        [JsonProperty("owner")]
        public string Owner { get; set; }

        /// <summary>
        /// Is Registered
        /// </summary>
        [JsonProperty("abbi_registered")]
        public string IsRegistered { get; set; }

        /// <summary>
        ///  Historical Rank
        /// </summary>
        [JsonProperty("historical_rank")]
        public string HistoricalRank { get; set; }

        /// <summary>
        ///  Active Rank
        /// </summary>
        [JsonProperty("active_rank")]
        public string ActiveRank { get; set; }

        /// <summary>
        /// Mounted
        /// </summary>
        [JsonProperty("mounted")]
        public int Mounted { get; set; }

        /// <summary>
        /// Rode
        /// </summary>
        [JsonProperty("rode")]
        public int Rode { get; set; }

        /// <summary>
        /// Average Mark
        /// </summary>
        [JsonProperty("avgmark")]
        public decimal AverageMark { get; set; }

        /// <summary>
        /// Power Rating
        /// </summary>
        [JsonProperty("power_rating")]
        public decimal PowerRating { get; set; }

        /// <summary>
        /// BuckOff Percentage
        /// </summary>
        [JsonProperty("buckoff_perc")]
        public decimal BuckOffPerc { get; set; }

        /// <summary>
        /// Out VS Top Riders
        /// </summary>
        [JsonProperty("outs_vs_top_riders")]
        public int OutsVsTopRiders { get; set; }

        /// <summary>
        /// BuckOff Percentage VS Top Riders
        /// </summary>
        [JsonProperty("buckoff_perc_vs_top_riders")]
        public decimal BuckOffPercVsTopRider { get; set; }

        /// <summary>
        /// Out VS Left Hand Rider
        /// </summary>
        [JsonProperty("outs_vs_lh_riders")]
        public int OutVsLeftHandRider { get; set; }

        /// <summary>
        /// BuckOff Percentage VS Left Hand Rider
        /// </summary>
        [JsonProperty("buckoff_perc_vs_lh_riders")]
        public decimal BuckOffPercVsLeftHandRider { get; set; }

        /// <summary>
        /// Out VS Right Hand Rider
        /// </summary>
        [JsonProperty("outs_vs_rh_riders")]
        public int OutVsRightHandRider { get; set; }

        /// <summary>
        /// BuckOff Percentage VS Right Hand Rider
        /// </summary>
        [JsonProperty("buckoff_perc_vs_rh_riders")]
        public decimal BuckOffPercVsRightHandRider { get; set; }

        /// <summary>
        /// Age
        /// </summary>
        public int Age { get; set; } = 0;

        /// <summary>
        /// BuckOff Streak
        /// </summary>
        public int BuckOffStreak { get; set; } = 0;

        /// <summary>
        /// Breeding
        /// </summary>
        public string Breeding { get; set; }

        /// <summary>
        /// Bucking Statistics
        /// </summary>
        public string BuckingStatistics { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <summary>
        /// Is Active
        /// </summary>
        public bool isActive { get; set; }

        /// <summary>
        /// Is Delete
        /// </summary>
        public bool isDelete { get; set; }

        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Bull Id
        /// </summary>
        public int BullId { get; set; }
        /// <summary>
        /// Is Added Favorite
        /// </summary>
        [JsonProperty("isAddedFavorite")]
        public bool IsAddedFavorite { get; set; }

        public bool isAddedInLongTermTeam { get; set; }
        public string LTTeamIcon { get; set; }
        public string LTTeamName { get; set; }
        public string WorldStanding { get; set; }
        public decimal RankRideScore { get; set; }
    }

    public class BullEventDto
    { 
        public int Season { get; set; }
        public string Series { get; set; }
        public string Date { get; set; }
        public string Location { get; set; }
        public string EventTpe { get; set; }
        public string EventName { get; set; }
        public string Rider { get; set; }
        public string BullScore { get; set; }
        public string RiderScore { get; set; }
        public string Buckoftime { get; set; }
        
    }
}
