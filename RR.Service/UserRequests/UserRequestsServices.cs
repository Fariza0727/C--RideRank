using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;

namespace RR.Service
{
    public class UserRequestsServices : IUserRequestsServices
    {
        private readonly IRepository<UserRequests, RankRideContext> _RepoRequest;
        public UserRequestsServices(IRepository<UserRequests,RankRideContext> repoRequest)
        {
            _RepoRequest = repoRequest;
        }


        public void Dispose()
        {
            if (_RepoRequest != null)
                _RepoRequest.Dispose();
        }

        public Task<IEnumerable<UserRequestsDto>> GetRequests(string AspUserId)
        {
            var requests_ = _RepoRequest.Query().Filter(r => r.UserId == AspUserId).Get();
            return Task.FromResult(UserRequestsMapper.Map(requests_));
        }

        public async Task<UserRequestsDto> SentRequest(UserRequestsDto requestsDto)
        {
            if(requestsDto!=null && !string.IsNullOrEmpty(requestsDto.UserId))
            {
                UserRequests Entity = _RepoRequest.Query().Filter(r => r.LongTermTeamId == requestsDto.LongTermTeamId && r.IsApproved != true).Get().SingleOrDefault();


                if(Entity!=null)
                {
                  
                    Entity.RequestMessage = requestsDto.RequestMessage;
                    Entity.Message = requestsDto.Message;
                    await _RepoRequest.UpdateAsync(Entity);

                    return await Task.FromResult(UserRequestsMapper.MapDto(Entity));
                }
                else
                {
                    Entity = Entity??new UserRequests();

                    Entity.CreatedBy = requestsDto.CreatedBy;
                    Entity.CreatedDate = DateTime.Now;
                    Entity.RequestMessage = requestsDto.RequestMessage;
                    Entity.RequestNo = RandomString(6);
                    Entity.Title = requestsDto.Title;
                    Entity.ReturlUrl = requestsDto.ReturlUrl;
                    Entity.UserId = requestsDto.UserId;
                    Entity.LongTermTeamId = requestsDto.LongTermTeamId;
                    Entity.Message = requestsDto.Message;

                    await _RepoRequest.InsertAsync(Entity);

                    return await Task.FromResult(UserRequestsMapper.MapDto(Entity));
                }
            }

            return await Task.FromResult(requestsDto);
        }

        // Generate a random string with a given size    
        private string RandomString(int size, bool lowerCase = false)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }
}
