namespace RR.Dto
{
     public class EventContestDto
     {
          public string UserId { get; set; }
          public int EventId { get; set; }
          public string EventName { get; set; }
          public int priceFrom { get; set; }
          public int priceTo { get; set; }
          public int priceFilter { get; set; }
          public long contestId { get; set; }
     }
}
