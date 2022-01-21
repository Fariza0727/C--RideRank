using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
   public interface IBullRiderPicturesService : IDisposable
    {
        /// <summary>
        /// Get Bull Picture
        /// </summary>
        /// <param name="BullId"></param>
        /// <param name="basUrl"></param>
        /// <returns></returns>
        Task<string> GetBullPic(int BullId, string basUrl = "");

        /// <summary>
        /// Get Rider Picture
        /// </summary>
        /// <param name="RiderId"></param>
        /// <param name="basUrl"></param>
        /// <returns></returns>
        Task<string> GetRiderPic(int RiderId, string basUrl = "");
    }
}
