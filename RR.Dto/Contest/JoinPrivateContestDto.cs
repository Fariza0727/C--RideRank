using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RR.Dto.Contest
{
    public class JoinPrivateContestDto
    {
        [Required(ErrorMessage = "Please enter code.")]
        public string Code { get; set; }
    }
}
