using RR.AdminData;
using RR.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RR.AdminMapper
{
   public class PictureManagerMapper
    {
        /// <summary>
        /// Map PicturesManager To PictureManagerDto
        /// </summary>
        /// <param name="pictures">List Of PicuresManager</param>
        /// <returns>List of PictureManagerLiteDto</returns>
        public static IEnumerable<PictureManagerLiteDto> Map(IEnumerable<PicuresManager> pictures)
        {
            return pictures.Select(p => MapLiteDto(p));
        }

        /// <summary>
        /// MapDto
        /// </summary>
        /// <param name="picture">The PicuresManager</param>
        /// <returns>The PictureManagerLiteDto</returns>
        public static PictureManagerLiteDto MapLiteDto(PicuresManager picture)
        {
            if (picture == null)
                return null;

            return new PictureManagerLiteDto
            {
                Id = picture.Id,
                BullId = picture.BullId,
                BullPicture = picture.BullPicture,
                RiderId = picture.RiderId,
                RiderPicture = picture.RiderPicture,
                BullName = picture.BullName,
                RiderName = picture.RiderName
            };
        }

        /// <summary>
        /// MapDto
        /// </summary>
        /// <param name="picture">The PicuresManager</param>
        /// <returns>The PictureManagerLiteDto</returns>
        public static PictureManagerDto MapDto(PicuresManager picture)
        {
            if (picture == null)
                return null;

            return new PictureManagerDto
            {
                Id = picture.Id,
                BullId = picture.BullId,
                BullPicture = picture.BullPicture,
                RiderId = picture.RiderId,
                RiderPicture = picture.RiderPicture,

            };
        }
    }
}
