namespace RR.Dto
{
     public class UserContestLiteDto
     {
          public int Rank { get; set; }

          /// <summary>
          /// User Name
          /// </summary>
          public string Username { get; set; }

          /// <summary>
          /// JoiningDate
          /// </summary>
          public System.DateTime JoiningDate { get; set; }

          /// <summary>
          /// Team Point
          /// </summary>
          public decimal TeamPoint { get; set; }

          /// <summary>
          /// Contest Name
          /// </summary>
          public string ContestName { get; set; }

          /// <summary>
          /// Joining Fee
          /// </summary>
          public decimal JoiningFee { get; set; }

          /// <summary>
          /// EventStatus
          /// </summary>
          public string EventStatus { get; set; }

          /// <summary>
          /// User Id
          /// </summary>
          public string UserId { get; set; }

          /// <summary>
          /// Contest Id
          /// </summary>
          public int ContestId { get; set; }

          /// <summary>
          /// TeamId
          /// </summary>
          public int TeamId { get; set; }

          public int EventId { get; set; }

          public string Avtar { get; set; }
          public string TeamName { get; set; }
          public string JoiningDateUTCString { get; set; }
          public bool IsAward { get; set; }
    }
}
