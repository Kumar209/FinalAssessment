using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Models.Dto
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Please provide new password")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Please provide confirm password")]
        [Compare("Password", ErrorMessage = "Confirm password does not password")]
        public string ConfirmPassword { get; set; }
    }
}
