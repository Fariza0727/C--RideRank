using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RR.AdminMapper;
using RR.Core;
using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;

namespace RR.AdminService
{
    public class UserRequestsServices : IUserRequestsServices
    {
        private readonly IRepository<UserRequests, RankRideContext> _RepoRequest;
        private readonly AppSettings _appsettings;

        public UserRequestsServices(IRepository<UserRequests,RankRideContext> repoRequest, IOptions<AppSettings> appsettings)
        {
            _RepoRequest = repoRequest;
            _appsettings = appsettings.Value;
        }

        public void Dispose()
        {
            if (_RepoRequest != null)
                _RepoRequest.Dispose();
        }

        /// <summary>
        /// Get All Bulls
        /// </summary>
        /// <param name="start">The Start Page</param>
        /// <param name="length">The Page Size</param>
        /// <param name="searchStr">The Search Keyword</param>
        /// <param name="sort">The Order of page</param>
        /// <returns>List Of 10 Bulls Along</returns>
        public async Task<Tuple<IEnumerable<UserRequestsLiteDto>, int>> GetAllRequests(int start, int length, int column, string searchStr = "", string sort = "")
        {
            int count = 0;

            var predicate = PredicateBuilder.True<UserRequests>()
           .And(x => x.IsDelete == false && (searchStr == ""
              || x.Title.ToLower().Contains(searchStr.ToLower())
              || x.Message.Contains(searchStr.ToLower())
              || x.RequestNo.ToString().Contains(searchStr.ToLower())));

            var requests = _RepoRequest
                .Query().Includes(r=>r.Include(u=>u.User))
                .Includes(r => r.Include(u => u.LongTermTeam))
              .Filter(predicate);

            if (FilterSortingVariable.USERREQUEST_NO == column)
            {
                requests = (sort == "desc" ? requests.OrderBy(x => x.OrderByDescending(xx => xx.RequestNo)) : requests.OrderBy(x => x.OrderBy(xx => xx.RequestNo)));
            }
            if (FilterSortingVariable.USERREQUEST_USERNAME == column)
            {
                requests = (sort == "desc" ? requests.OrderBy(x => x.OrderByDescending(xx => xx.User.UserName)) : requests.OrderBy(x => x.OrderBy(xx => xx.User.UserName)));
            }
            if (FilterSortingVariable.USERREQUEST_TITLE == column)
            {
                requests = (sort == "desc" ? requests.OrderBy(x => x.OrderByDescending(xx => xx.Title)) : requests.OrderBy(x => x.OrderBy(xx => xx.Title)));
            }
            if (FilterSortingVariable.USERREQUEST_MESSAGE == column)
            {
                requests = (sort == "desc" ? requests.OrderBy(x => x.OrderByDescending(xx => xx.Message)) : requests.OrderBy(x => x.OrderBy(xx => xx.Message)));
            }
            if (FilterSortingVariable.USERREQUEST_MESSAGE == column)
            {
                requests = (sort == "desc" ? requests.OrderBy(x => x.OrderByDescending(xx => xx.Message)) : requests.OrderBy(x => x.OrderBy(xx => xx.Message)));
            }
            if (FilterSortingVariable.USERREQUEST_STATUS == column)
            {
                requests = (sort == "desc" ? requests.OrderBy(x => x.OrderByDescending(xx => xx.IsApproved)) : requests.OrderBy(x => x.OrderBy(xx => xx.IsApproved)));
            }


            return await Task.FromResult(new Tuple<IEnumerable<UserRequestsLiteDto>, int>(requests
                 .GetPage(start, length, out count).Select(y => new UserRequestsLiteDto
                 {
                     Id = y.Id,
                     IsApproved = y.IsApproved,
                     LongTermTeamId = Convert.ToInt32(y.LongTermTeamId),
                     Message = y.Message,
                     RequestNo = y.RequestNo,
                     ReturlUrl = y.ReturlUrl,
                     TeamBrand = y.LongTermTeam.TeamBrand,
                     TeamIcon = string.Concat(_appsettings.MainSiteURL,y.LongTermTeam.TeamIcon),
                     Title = y.Title,
                     UserId = y.UserId,
                     UserName = y.User?.UserName,
                     RequestMessage = y.RequestMessage
                     
                 }), count));
        }

        public Task<UserRequestsLiteDto> GetRequest(long Id)
        {
            var request_ = _RepoRequest.Query().Filter(r => r.Id == Id).Get().SingleOrDefault();
            if(request_ != null)
            {
                return Task.FromResult(UserRequestsMapper.MapLiteDto(request_));
            }

            return Task.FromResult(default(UserRequestsLiteDto));
        }

        public async Task<UserRequestsLiteDto> UpdateRequest(long Id)
        {
            var request_ = _RepoRequest.Query().Filter(r => r.Id == Id).Includes(r=>r.Include(m=>m.LongTermTeam)).Get().SingleOrDefault();
            if (request_ != null)
            {
                request_.IsApproved = request_.IsApproved ? false : true;
                request_.Message =string.Concat($"Your <b>{request_.LongTermTeam.TeamBrand}</b> Team updation request has been ", request_.IsApproved ? "Approved" : "Under Reviewed");
                await _RepoRequest.UpdateAsync(request_);
            }

            return await Task.FromResult(UserRequestsMapper.MapLiteDto(request_));
        }
    }
}
