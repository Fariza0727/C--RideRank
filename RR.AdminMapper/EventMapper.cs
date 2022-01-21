using RR.Dto;
using RR.StaticData;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
     public static class EventMapper
     {
          /// <summary>
          /// Map List Of events To EventDto
          /// </summary>
          /// <param name="events"></param>
          /// <returns></returns>
          public static IEnumerable<EventDto> Map(IEnumerable<Event> events)
          {
               return events.Select(p => MapDto(p));
          }

          /// <summary>
          /// MapDto
          /// </summary>
          /// <param name="eventDetail">An Event Detail</param>
          /// <returns>An EventDto</returns>
          public static EventDto MapDto(Event eventDetail)
          {
               return new EventDto
               {
                    Id = eventDetail.Id,
                    EventId = eventDetail.EventId,
                    IsActive = eventDetail.IsActive,
                    City = eventDetail.City,
                    Location = eventDetail.Location,
                    PerfTime = eventDetail.PerfTime,
                    Sanction = eventDetail.Sanction,
                    Season = eventDetail.Season,
                    StartDate = eventDetail.StartDate,
                    State = eventDetail.State,
                    Title = eventDetail.Title,
                    Type = eventDetail.Type
               };
          }
     }
}
