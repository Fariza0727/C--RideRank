using RR.Data;
using RR.Dto;

namespace RR.Mapper
{
     public static class PasswordRequestMapper
     {
          public static PasswordRequestDto MapDto(PasswordRequest passwordRequest)
          {
               return new PasswordRequestDto
               {
                    Code = passwordRequest.Code,
                    Email = passwordRequest.UserEmail,
                    IsUsed = passwordRequest.IsUsed
               };
          }
     }
}
