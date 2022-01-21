using System.ComponentModel.DataAnnotations;

namespace RR.Dto.AwardType
{
    public class AwardTypeDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        /// <summary>
        /// CreatedOn
        /// </summary>
        public string CreatedOn { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        public string CreatedBy { get; set; }
    }
}
