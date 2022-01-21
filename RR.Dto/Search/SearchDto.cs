using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class SearchDto
     {
          /// <summary>
          /// List Of Events
          /// </summary>
          public IEnumerable<EventDto> EventsList { get; set; }

          /// <summary>
          /// List Of Riders
          /// </summary>
          public IEnumerable<RiderDto> RidersList { get; set; }

          /// <summary>
          /// List Of Bulls
          /// </summary>
          public IEnumerable<BullDto> BullsList { get; set; }

          /// <summary>
          /// List Of News
          /// </summary>
          public IEnumerable<NewsDto> NewsList { get; set; }

          /// <summary>
          /// PageCount
          /// </summary>
          public int PageCount { get; set; }

          /// <summary>
          /// PageNumber
          /// </summary>
          public int PageNumber { get; set; }

          /// <summary>
          /// Keyword
          /// </summary>
          [Required(ErrorMessage = "Enter Any Keyword")]
          public string Keyword { get; set; }
     }
}
