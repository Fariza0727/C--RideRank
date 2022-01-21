using RR.AdminData;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.AdminService
{
    public interface IPictureManagerService : IDisposable
    {
        /// <summary>
        /// Get all Pictures of bulls/riders
        /// </summary>
        /// <returns>IEnumerable<PictureManagerLiteDto></returns>
        Task<IEnumerable<PictureManagerLiteDto>> GetPictures();

        /// <summary>
        /// Get All Pictures
        /// </summary>
        /// <param name="start">Page Number</param>
        /// <param name="length">Number Of Record </param>
        /// <param name="searchStr"></param>
        /// <param name="sort"></param>
        /// <param name="isBull"></param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<PictureManagerLiteDto>, int>> GetAllPictures(int start, int length, int column, string searchStr = "", string sort = "",bool isBull = false);

        /// <summary>
        /// Add-Edit picture
        /// </summary>
        /// <param name="pictureManager"></param>
        /// <returns>PictureManagerLiteDto</returns>
        Task<PictureManagerLiteDto> AddEditPicture(PictureManagerDto pictureManager);

        /// <summary>
        /// get picture
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PictureManagerDto></returns>
        Task<PictureManagerDto> GetPicture(long id);

        /// <summary>
        /// get ids which are added pictures
        /// </summary>
        /// <returns>Item1: riderIds, Item2:bullIds</returns>
        Task<Tuple<List<int>, List<int>>> AddRiderBullPictursIds();

        /// <summary>
        /// Delete picture
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeletePicture(long id);

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
        Task<RidermanagerDto> GetRiderManager(int RiderId);
        Task<RidermanagerDto> AddEditRiderManager(RidermanagerDto rider, int riderid);
    }
}
