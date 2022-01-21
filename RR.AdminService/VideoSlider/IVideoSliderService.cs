using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface IVideoSliderService : IDisposable
     {
         /// <summary>
         /// 
         /// </summary>
         /// <param name="sliderDto"></param>
         /// <returns></returns>
          Task AddEditVSlider(VideoSliderDto sliderDto);

         /// <summary>
         /// 
         /// </summary>
         /// <param name="start"></param>
         /// <param name="length"></param>
         /// <param name="column"></param>
         /// <param name="searchStr"></param>
         /// <param name="sort"></param>
         /// <returns></returns>
          Task<Tuple<IEnumerable<VideoSliderDto>, int>> GetAllVSlider(int start, int length, int column, string searchStr = "", string sort = "");

          /// <summary>
          /// 
          /// </summary>
          /// <param name="Id"></param>
          /// <returns></returns>
          Task<VideoSliderDto> GetVSliderById(int Id);

         /// <summary>
         /// 
         /// </summary>
         /// <param name="Id"></param>
          void DeleteVSlider(int Id);
        
        Task UpdateStatus(int Id,bool status);
    }
}
