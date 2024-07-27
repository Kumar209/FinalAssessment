using FinalAssessment_Backend.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Models.Dto
{
    public class PrashantDbUserDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(30, ErrorMessage = "First name cannot be longer than 30 characters.")]
        public string FirstName { get; set; } = null!;


        [Required(ErrorMessage = "MiddleName is required")]
        [StringLength(30, ErrorMessage = "Middle name cannot be longer than 30 characters.")]
        public string MiddleName { get; set; } = null!;


        [StringLength(30, ErrorMessage = "Last name cannot be longer than 30 characters.")]
        public string? LastName { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email should be valid")]
        [StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
        public string Email { get; set; }



        [Required(ErrorMessage = "Gender is required")]
        public byte Gender { get; set; }


        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format")]
        [Required(ErrorMessage = "DateOfJoining is required")]
        public DateOnly DateOfJoining { get; set; }



        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateOnly DateOfBirth { get; set; }



        [Required(ErrorMessage = "Phone is required")]
        [Phone]
        [StringLength(10, ErrorMessage = "Phone number cannot be longer than 10 characters.")]
        public string Phone { get; set; }


        [Phone]
        [StringLength(10, ErrorMessage = "Alternate phone number cannot be longer than 10 characters.")]
        public string? AlternatePhone { get; set; }


        [Required(ErrorMessage = "Email is required")]
        public IFormFile ImageFile { get; set; }


        public string? ImageUrl { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public DateOnly? CreatedDate { get; set; }

        public DateOnly? ModifiedDate { get; set; }

        public string? CreatedBy { get; set; }

        public string? ModifiedBy { get; set; }




        [Required(ErrorMessage = "Address is required")]
        public virtual ICollection<PrashantDbAddressDto> PrashantDbAddresses { get; set; } = new List<PrashantDbAddressDto>();
    }
}
