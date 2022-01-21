namespace RR.Dto
{
     public class PlayStandingLiteDto
     {
          /// <summary>
          /// Team Rank
          /// </summary>
          public int Rank { get; set; }

          /// <summary>
          /// UserName
          /// </summary>
          public string UserName { get; set; }
          /// <summary>
          /// UserPic
          /// </summary>
          public string UserPic { get; set; }

          /// <summary>
          /// Team points
          /// </summary>
          public decimal TeamPoint { get; set; }

          /// <summary>
          /// ContestTitle
          /// </summary>
          public string ContestTitle { get; set; }

          /// <summary>
          /// ContestId
          /// </summary>
          public long ContestId { get; set; }
        
          /// <summary>
          /// ContestId
          /// </summary>
          public int NumberOfContests { get; set; }
    }
}
