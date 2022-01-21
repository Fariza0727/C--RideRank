using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RR.Dto
{
    public class PointTableDto
    {
        /// <summary>
        /// Point ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Point For Category
        /// </summary>
        [Required, Range(1, int.MaxValue, ErrorMessage = "Required")]
        public int PointFor { get; set; }

        /// <summary>
        /// Point for category value
        /// </summary>
        public string PointForValue { get; set; }

        /// <summary>
        /// Point Key
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public string Key { get; set; }

        /// <summary>
        /// Point Value
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public string Value { get; set; }

        /// <summary>
        /// Point Type List
        /// </summary>
        public List<SelectListItem> PointType { get; set; }
    }
}
