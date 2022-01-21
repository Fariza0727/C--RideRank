using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface IEventService : IDisposable
     {
          /// <summary>
          /// Get EventDraw By Id
          /// </summary>
          /// <param name="id">An EventDraw Id</param>
          /// <returns>List of EventDrawDto</returns>
          Task<IEnumerable<EventDrawDto>> GetEventDrawById(int id);

          /// <summary>
          /// Get All Events
          /// </summary>
          /// <returns></returns>
          Task<Tuple<IEnumerable<EventDto>, IEnumerable<EventDto>, IEnumerable<EventDto>>> GetAllEvents();

          /// <summary>
          /// Get Latest Upcoming Event
          /// </summary>
          /// <returns>EventDto</returns>
          Task<EventDto> GetUpcomingEvent();

          /// <summary>
          /// Get upcoming events
          /// </summary>
          /// <returns></returns>
          Task<IEnumerable<EventDto>> GetUpcomingEvents(int count=0);

          /// <summary>
          /// Get event detail by Id
          /// </summary>
          /// <param name="eventId"></param>
          /// <returns></returns>
          Task<EventDto> GetEventById(int eventId);

        /// <summary>
        /// Get completed events
        /// </summary>
        /// <returns>IEnumerable<EventDto></returns>
        Task<IEnumerable<EventDto>> GetCompletedEvents();

        /// <summary>
        /// Get All Completed events
        /// </summary>
        /// <param name="start">The Start Page</param>
        /// <param name="length">The Page Size</param>
        /// <returns>List Of 10 Events Along</returns>
        Task<Tuple<IEnumerable<EventDto>, int>> GetAllCompletedEvents(int start, int length);
    }
}
