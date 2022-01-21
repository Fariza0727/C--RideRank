using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
    public class UserFavoriteBullsRidersDto
    {
        public UserFavoriteBullsRidersDto()
        {
            this.FavoriteBulls = new List<UserFavoriteBullsDto>();
            this.FavoriteRiders = new List<UserFavoriteRidersDto>();
        }
       public List<UserFavoriteBullsDto> FavoriteBulls { get; set; }
       public List<UserFavoriteRidersDto> FavoriteRiders { get; set; }
        
    }

    public class UserFavoriteBullsDto
    {
        public int Id { get; set; }
        public int BullId { get; set; }
        public string BullName { get; set; }
        public string OwnerName { get; set; }
        public string Avatar { get; set; }
        public decimal PowerRating { get; set; }
        public decimal AverageMark { get; set; }
        public decimal RankRideScore { get; set; }
    }
    public class UserFavoriteRidersDto
    {
        public int Id { get; set; }
        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public string Avatar { get; set; }
        public int WorldRank { get; set; }
        public decimal PowerRanking { get; set; }
        public decimal RRTotalPoint { get; set; }

    }
}

