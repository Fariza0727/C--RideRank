using RR.Dto;
using RR.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RR.StaticMapper
{
     public class EventMapper
     {
          /// <summary>
          /// Map List Of Events To List Of EventDto
          /// </summary>
          /// <typeparam name="T">Generic Type</typeparam>
          /// <param name="events">List Of Events</param>
          /// <returns>List Of EventDto</returns>
          public static IEnumerable<T> Map<T>(IEnumerable<Event> events)
          {
               return events.Select(p => Map<T>(p));
          }

          /// <summary>
          /// Map Event to EventDto
          /// </summary>
          /// <typeparam name="T">Generic Type</typeparam>
          /// <param name="events">The Event</param>
          /// <returns>The eventDto</returns>
          public static T Map<T>(Event events)
          {
               return (T)Convert.ChangeType(Map(events), typeof(T));
          }

          /// <summary>
          /// MapDto
          /// </summary>
          /// <typeparam name="T">Generic Type</typeparam>
          /// <param name="events">The Event</param>
          /// <returns>The eventDto</returns>
          public static EventDto Map(Event events)
          {
               return new EventDto
               {
                    //Average = events.Average,
                    City = events.City,
                    EventId = events.EventId,
                    Id = events.Id,
                    IsActive = events.IsActive,
                    Location = events.Location,
                    PerfTime = events.PerfTime,
                    Sanction = events.Sanction,
                    //Saves  = events.Saves,
                    Season = events.Season,
                    StartDate = events.StartDate,
                    State = events.State,
                    //Time = events.Time,
                    Title = events.Title
               };
          }

          /// <summary>
          /// Map EventDraw To EventDrawDto
          /// </summary>
          /// <param name="eventDraws">List Of EventDraw</param>
          /// <returns>List Of EventDrawDto</returns>
          public static IEnumerable<EventDrawDto> Map(IEnumerable<EventDraw> eventDraws)
          {
               return eventDraws.Select(p => Map(p));
          }

          /// <summary>
          /// Map
          /// </summary>
          /// <param name="eventDraws">The Event Draw</param>
          /// <returns>The EventDrawDto</returns>
          public static EventDrawDto Map(EventDraw eventDraws)
          {
               return new EventDrawDto
               {
                    EventName = eventDraws.Event.Title,
                    BullId = eventDraws.BullId,
                    BullName = eventDraws.BullName,
                    EventId = eventDraws.Event.EventId,
                    RiderId = eventDraws.RiderId,
                    RiderName = eventDraws.RiderName,
                    Round = Convert.ToString(eventDraws.Round),
                    RemainingDate = eventDraws.Event.PerfTime,
                    StartDate = eventDraws.Event.StartDate
               };
          }
     }
}
