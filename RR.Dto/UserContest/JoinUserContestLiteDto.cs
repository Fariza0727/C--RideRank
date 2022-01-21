namespace RR.Dto
{
     public class JoinUserContestLiteDto
     {
          /// <summary>
          /// UserName
          /// </summary>
          public string UserName { get; set; }

          /// <summary>
          /// Email 
          /// </summary>
          public string Email { get; set; }

          /// <summary>
          /// ContestName
          /// </summary>
          public string ContestName { get; set; }

          /// <summary>
          /// Points
          /// </summary>
          public decimal TeamPoint { get; set; }

          /// <summary>
          /// Ranks
          /// </summary>
          public int TeamRank { get; set; }

          /// <summary>
          /// Price
          /// </summary>
          public decimal Price { get; set; }

          /// <summary>
          /// Token
          /// </summary>
          public decimal Token { get; set; }

          /// <summary>
          /// Merchendise
          /// </summary>
          public string Merchendise { get; set; }

          /// <summary>
          /// Other Award
          /// </summary>
          public string OtherAward { get; set; }

          public string UserId { get; set; }

          public int ContestId { get; set; }

          public int TeamId { get; set; }
          public string TeamName { get; set; }
          public string Avatar { get; set; }
          public bool CanUpdateTeam { get; set; }
          public int NumberOfContest { get; set; }
          
     }
}
