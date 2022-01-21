using RR.Dto;
using RR.Dto.Event;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.StaticService
{
    public interface IEventService : IDisposable
    {
        /// <summary>
        /// Current Add Edit Events
        /// </summary>
        /// <param name="model">List of Curent EventDto</param>
        /// <returns></returns>
        Task CurrentAddEditEvents(List<CurrentEventDto> model);
        /// <summary>
        /// Current Add Edit Events Dropouts
        /// </summary>
        /// <param name="model">List of Curent EventDto</param>
        /// <returns></returns>
        Task CurrentAddEditEventsDropOut(List<CurrentEventDto> model);

        /// <summary>
        /// Add Edit Events
        /// </summary>
        /// <param name="eventDto">List Of EventDto</param>
        /// <returns></returns>
        Task AddEditEvents(List<EventDto> eventDto);

        /// <summary>
        /// returns the list of events which completed recently.
        /// </summary>
        /// <returns></returns>
        List<EventServiceDto> GetRecentlyCompletedEvent();

        /// <summary>
        /// update event winning distribution status
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task UpdateEventWinningDistribution(int eventId);

        /// <summary>
        /// Update event result response in DB
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task UpdateEventResultResponse(int eventId, string response);

        /// <summary>
        /// returns the list of events which upcoming/current recently.
        /// </summary>
        /// <returns></returns>
        Task<List<EventDto>> GetUpcomingCurrentEvent();

        /// <summary>
        /// returns draw is loaded or not
        /// </summary>
        /// <returns></returns>
        Task<bool> GetEventDrawDeclared(int eventId, string pbrId = "");

        /// <summary>
        /// Edit Event
        /// </summary>
        /// <param name="pbrid"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        Task EditEvent(string pbrid);

        /// <summary>
        /// Add Edit Events
        /// </summary>
        /// <param name="eventDto">List Of EventDto</param>
        /// <returns></returns>
        Task ManageEvents(List<EventDto> eventDto);

        /// <summary>
        /// Check Event is completed and winnig distributed.
        /// </summary>
        /// <param name="pbrID"></param>
        /// <returns></returns>
        Task<EventCompleteDto> CheckEventComplete(string pbrID);
    }
}
