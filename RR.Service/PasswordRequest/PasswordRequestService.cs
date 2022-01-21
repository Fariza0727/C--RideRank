using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
     public class PasswordRequestService : IPasswordRequestService
     {
          #region Constructor

          private readonly IRepository<PasswordRequest, RankRideContext> _repoPasswordRequest;

          public PasswordRequestService(IRepository<PasswordRequest, RankRideContext> repoPasswordRequest)
          {
               _repoPasswordRequest = repoPasswordRequest;
          }

          #endregion

          /// <summary>
          /// Get News Detail
          /// </summary>
          /// <param name="title">The Title</param>
          /// <param name="id">An Id</param>
          /// <returns>The NewsDto</returns>
          public async Task<PasswordRequestDto> GetPasswordRequest(string code)
          {
               var pass = _repoPasswordRequest.Query()
                    .Filter(x => x.Code == code && x.IsUsed == false)
                    .Get()
                    .SingleOrDefault();
               return await Task.FromResult(PasswordRequestMapper.MapDto(pass));
          }

          /// <summary>
          /// Get News Detail
          /// </summary>
          /// <param name="title">The Title</param>
          /// <param name="id">An Id</param>
          /// <returns>The NewsDto</returns>
          public async Task AddEditPasswordRequest(PasswordRequestDto passwordRequestDto)
          {
               var pass = _repoPasswordRequest.Query()
                   .Filter(x => x.Code == passwordRequestDto.Code && x.UserEmail == passwordRequestDto.Email && !x.IsUsed)
                   .Get()
                   .SingleOrDefault();

               if (pass == null)
               {
                    pass = new PasswordRequest
                    {
                         Code = passwordRequestDto.Code,
                         IsUsed = false,
                         UserEmail = passwordRequestDto.Email
                    };
                    await _repoPasswordRequest.InsertGraphAsync(pass);
               }
               else
               {
                    pass.Code = passwordRequestDto.Code;
                    pass.UserEmail = passwordRequestDto.Email;
                    pass.IsUsed = passwordRequestDto.IsUsed;
                    await _repoPasswordRequest.UpdateAsync(pass);
               }
          }

          /// <summary>
          /// Dispose News Service
          /// </summary>
          public void Dispose()
          {
               if (_repoPasswordRequest != null)
               {
                    _repoPasswordRequest.Dispose();
               }
          }
     }
}
