namespace RR.Dto
{
     public class TeamLiteDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public int Id { get; set; }

          /// <summary>
          /// Team Id
          /// </summary>
          public int TeamId { get; set; }

          /// <summary>
          /// UserName
          /// </summary>
          public string UserEmail { get; set; }

          /// <summary>
          /// Event Name
          /// </summary>
          public string EventName { get; set; }

          /// <summary>
          /// Contest Type
          /// </summary>
          public string ContestType { get; set; }
     }
}
