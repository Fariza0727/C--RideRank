using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IEventService : IDisposable
    {
        /// <summary>
        /// Get All Events
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Records on each page</param>
        /// <param name="searchStr">Search String</param>
        /// <param name="sort">Order</param>
        /// <returns>List Of EventRecords</returns>
        Task<Tuple<IEnumerable<EventLiteDto>, int>> GetAllEvents(int start, int length, int column, string searchStr = "", string sort = "");

        /// <summary>
        /// Get Event By Id
        /// </summary>
        /// <param name="eventId">An Event Id</param>
        /// <returns>The Event detail</returns>
        Task<EventDto> GetEventById(int eventId);

        /// <summary>
        /// Update Event Detail
        /// </summary>
        /// <param name="eventDto">An Event Dto</param>
        /// <returns></returns>
        Task UpdateEventDetail(EventDto eventDto);

        /// <summary>
        /// Update Event Status
        /// </summary>
        /// <param name="eventId">An Event Id</param>
        /// <returns></returns>
        Task UpdateStatus(int eventId);

        /// <summary>
        /// Delete Event By Id
        /// </summary>
        /// <param name="eventId">An Event Id</param>
        /// <returns></returns>
        Task DeleteEventById(int eventId);

        /// <summary>
        /// Get upcoming events
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<EventDto>> GetUpcomingEvents();
        
        /// <summary>
        /// Get upcoming events which have not added any contest yet
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<IEnumerable<EventDto>> GetUpcomingEventsFiltered(long eventId = 0);

        Task<IEnumerable<EventLiteDto>> GetAllEvents();

        /// <summary>
        /// Get events as card
        /// </summary>
        /// <returns>Tuple<int, int> item1:total, item2:active</returns>
        Task<Tuple<int, int>> GetEventsAsCard();
    }
}
