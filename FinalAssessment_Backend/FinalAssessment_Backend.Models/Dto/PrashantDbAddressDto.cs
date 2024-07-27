using FinalAssessment_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Models.Dto
{
    public class PrashantDbAddressDto
    {
        public int? Id { get; set; }


        [Required(ErrorMessage = "City is required")]
        [StringLength(60, ErrorMessage = "City cannot be longer than 60 characters.")]
        public string City { get; set; } = null!;


        [Required(ErrorMessage = "State is required")]
        [StringLength(50, ErrorMessage = "State cannot be longer than 50 characters.")]
        public string State { get; set; } = null!;


        [Required(ErrorMessage = "Country is required")]
        [StringLength(50, ErrorMessage = "Country cannot be longer than 50 characters.")]
        public string Country { get; set; } = null!;


        [Required(ErrorMessage = "ZipCode is required")]
        [StringLength(6, ErrorMessage = "ZipCode cannot be longer than 6 characters.")]
        public string ZipCode { get; set; } = null!;

        [Required(ErrorMessage = "Address Type is required")]
        public int AddressTypeId { get; set; }


        public int? UserId { get; set; }
    }
}
