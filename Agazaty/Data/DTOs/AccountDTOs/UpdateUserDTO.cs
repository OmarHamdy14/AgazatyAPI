﻿using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.AccountDTOs
{
    public class UpdateUserDTO
    {
        [Required]
        public string FName { get; set; }
        [Required]
        public string SName { get; set; }
        [Required]
        public string TName { get; set; }
        [Required]
        public string LName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string NationalID { get; set; }
        [Required]
        public DateTime HireDate { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        //[Required]
       // public string Password { get; set; }
        [Required]
        public int Position { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public int? Departement_ID { get; set; }
        [Required]
        public bool IsDepartmentManager { get; set; }
    }
}