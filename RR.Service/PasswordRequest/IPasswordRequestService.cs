using RR.Dto;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface IPasswordRequestService
     {
          Task<PasswordRequestDto> GetPasswordRequest(string code);

          Task AddEditPasswordRequest(PasswordRequestDto passwordRequestDto);
     }
}
