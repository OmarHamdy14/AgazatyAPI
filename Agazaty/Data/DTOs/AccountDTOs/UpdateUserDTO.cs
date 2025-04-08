using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.AccountDTOs
{
    public class UpdateUserDTO
    {
        [Required]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "The field must contain only letters (English or Arabic), with no numbers or spaces.")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "The field must contain only letters (English or Arabic), with no numbers or spaces.")]
        public string SecondName { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "The field must contain only letters (English or Arabic), with no numbers or spaces.")]
        public string ThirdName { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "The field must contain only letters (English or Arabic), with no numbers or spaces.")]
        public string ForthName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "The National Number field must contain exactly 14 digits with no spaces or other characters.")]
        public string NationalID { get; set; }
        [Required]
        public DateTime HireDate { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "The Phone Number field must contain exactly 11 digits with no spaces or other characters.")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public int position { get; set; }
        public int NormalLeavesCount { get; set; }
        public int CasualLeavesCount { get; set; }
        public int NonChronicSickLeavesCount { get; set; }
        public int HistoryNormalLeavesCount { get; set; }
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
        [DefaultValue(0)]
        public string? Street { get; set; }
        public string? governorate { get; set; }
        public string? State { get; set; }
        public bool Disability { get; set; }

    }
}