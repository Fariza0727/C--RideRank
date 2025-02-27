﻿using Microsoft.AspNetCore.Mvc.Rendering;
using RR.AdminData;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IContestService : IDisposable
    {
        /// <summary>
        /// Get all contests by event id.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <param name="eventId"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<ContestDto>, int>> GetAllContest(int start, int length, int column, string searchStr = "", string sort = "", long eventId = 0);

        /// <summary>
        /// Add/Edit contest. 
        /// </summary>
        /// <param name="contestDto"></param>
        /// <returns></returns>
        Task<long> AddEditContest(ContestDto contestDto, string userId);

        /// <summary>
        /// Get contest detail by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ContestDto> GetContest(long id);

        /// <summary>
        /// Delete contest by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteContest(int id);

        /// <summary>
        /// Check For Title Available
        /// </summary>
        /// <param name="findTitle"></param>
        /// <returns></returns>
        Task<bool> IsTitleAvailable(string findTitle);
        /// <summary>
        /// Check contest available for event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> IsContestAvailable(int eventId, long Id);

        /// <summary>
        /// returns list of contests of particular event.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<List<ContestDto>> GetContestByEventId(int eventId);

        /// <summary>
        /// returns list of winnings of contest.
        /// </summary>
        /// <param name="contestId"></param>
        /// <returns></returns>
        Task<List<ContestWinner>> GetContestWinningByContestId(long contestId);

        /// <summary>
        /// returns list of contest for dropdown by eventid
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task<List<SelectListItem>> GetContestDropdownByEventId(int eventId);
    }
}
