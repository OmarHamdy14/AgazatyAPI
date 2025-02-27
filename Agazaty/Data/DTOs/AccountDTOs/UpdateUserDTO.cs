using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.AccountDTOs
{
    public class UpdateUserDTO
    {
        //public string FName { get; set; }
        //public string SName { get; set; }
        //public string TName { get; set; }
        //public string LName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string NationalID { get; set; }
        [Required]
        public string HireDate { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int Position { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        public int Departement_ID { get; set; }
    }
}