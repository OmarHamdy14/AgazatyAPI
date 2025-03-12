using Agazaty.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agazaty.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "The First Namefield must contain only letters (English or Arabic), with no numbers or spaces.")]
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
        public int position { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int NormalLeavesCount { get; set; }
        [Range(0, int.MaxValue)]
        public int CasualLeavesCount { get; set; }
        public int SickLeavesCount { get; set; }
        [ForeignKey("Department")]
        public int? Departement_ID { get; set; }
        public Department? Department { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPExpiry { get; set; }
        [DefaultValue(true)]
        public bool Active { get; set; }
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
        public int YearsOfWork { get; set; }
        public NormalLeaveSection? LeaveSection { get; set; }
        public CountsFromNormalLeaveTypes? Counts { get; set; }
        [DefaultValue(0)]
        public int TakenNormalLeavesCount_47 { get; set; }
        [DefaultValue(0)]
        public int TakenNormalLeavesCount_81Before3Years { get; set; }
        [DefaultValue(0)]
        public int TakenNormalLeavesCount_81Before2Years { get; set; }
        [DefaultValue(0)]
        public int TakenNormalLeavesCount_81Before1Years { get; set; }
        [DefaultValue(0)]
        public int TakenNormalLeavesCount { get; set; }

        //[DefaultValue(true)]
        //public bool IntializationCheck { get; set; }
    }
}