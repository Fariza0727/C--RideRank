namespace RR.Dto
{
     public class TeamDto
     {
          /// <summary>
          /// User id
          /// </summary>
          public string UserId { get; set; }

          /// <summary>
          /// Event Id
          /// </summary>
          public int EventId { get; set; }

          /// <summary>
          /// Contest Type
          /// </summary>
          public byte ContestType { get; set; }

          /// <summary>
          /// Team Number
          /// </summary>
          public int TeamNumber { get; set; }

          /// <summary>
          /// IsDelete
          /// </summary>
          public bool IsDelete = true;

          /// <summary>
          /// Bull Id
          /// </summary>
          public int BullId { get; set; }

          /// <summary>
          /// Is Substitute
          /// </summary>
          public bool IsSubstitute { get; set; }

          /// <summary>
          /// RiderId
          /// </summary>
          public int RiderId { get; set; }

          /// <summary>
          /// TeamId
          /// </summary>
          public int TeamId { get; set; }
     }
}
