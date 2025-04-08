using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.AccountDTOs
{
    public class CreateUserDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "The Phone Number field must contain exactly 11 digits with no spaces or other characters.")]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        // ErrorMessage = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        //[DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "The First Name field must contain only letters (English or Arabic), with no numbers or spaces.")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "The Second Name field must contain only letters (English or Arabic), with no numbers or spaces.")]
        public string SecondName { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "The Third Name field must contain only letters (English or Arabic), with no numbers or spaces.")]
        public string ThirdName { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "The Forth Name field must contain only letters (English or Arabic), with no numbers or spaces.")]
        public string ForthName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime HireDate { get; set; }
        [Required]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "The National Number field must contain exactly 14 digits with no spaces or other characters.")]
        public string NationalID { get; set; }
        [Required]
        public int position { get; set; }
        public int NormalLeavesCount { get; set; }
        public int CasualLeavesCount { get; set; }
        public int NonChronicSickLeavesCount { get; set; }
        public int? Departement_ID { get; set; }
        [DefaultValue(0)]
        public int NormalLeavesCount_47 { get; set; }
        [DefaultValue(0)]
        public int NormalLeavesCount_81Before3Years { get; set; }
        [DefaultValue(0)]
        public int NormalLeavesCount_81Before2Years { get; set; }
        [DefaultValue(0)]
        public int NormalLeavesCount_81Before1Years { get; set; }
        [DefaultValue(0)]
        public int HowManyDaysFrom81And47 { get; set; }
        public string? Street { get; set; }
        public string? governorate { get; set; }
        public string? State { get; set; }
        public bool Disability { get; set; }

    }
}