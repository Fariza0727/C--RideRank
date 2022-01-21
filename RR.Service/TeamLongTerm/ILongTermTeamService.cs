using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RR.Data;
using RR.Dto;
using RR.Dto.Team;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface ILongTermTeamService : IDisposable
    {
        Task DeleteTeam(int teamId);

        Task UpdateBrandIcon(LongTermTeam team, string brand, IFormFile File);

        Task<LongTermTeamInfoDto> GetTeam(int teamId);

        Task<LongTermTeamInfoDto> GetTeam(string userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="pageno"></param>
        /// <param name="pageSize"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<LongTermTeamFormationDto> LongTermTeamById(int teamId, int pageno, int pageSize, bool isBull = false, string search = "", int column = 0, string sort = "", string userId ="" );

       /// <summary>
       /// Get User long term team info
       /// </summary>
       /// <param name="userId"></param>
       /// <param name="onlyBull"></param>
       /// <param name="onlyRider"></param>
       /// <returns></returns>
        Task<LongTermTeamFormationLiteDto> LongTermTeamById(string userId);



        Task<bool> IsTeamExist(string userId);

        /// <summary>
        /// Create Team
        /// </summary>
        /// <param name="teamDto">The TeamDto</param>
        /// <returns></returns>
        Task<int> CreateTeam(IEnumerable<LongTermTeamDto> teamDto, CreateLongTermTeamRequestDto requestDto);

        /// <summary>
        /// Create long term Team Api
        /// </summary>
        /// <param name="requestDto">The TeamDto</param>
        /// <returns></returns>
        Task<int> CreateTeamApi(LongTermTeamFormationDto requestDto);

        /// <summary>
        /// Create long term Team Api
        /// </summary>
        /// <param name="requestDto">The TeamDto</param>
        /// <returns></returns>
        Task<int> CreateTeamApi(LongTermTeamFormationApiDto requestDto);
    }
}