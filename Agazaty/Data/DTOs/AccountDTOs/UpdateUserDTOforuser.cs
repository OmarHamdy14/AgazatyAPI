using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.AccountDTOs
{
    public class UpdateUserDTOforuser
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "تنسيق البريد الإلكتروني غير صحيح.")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "يجب أن يحتوي حقل رقم الهاتف على 11 رقماً بالضبط دون مسافات أو رموز أخرى.")]
        public string PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? Governorate { get; set; }
        public string? State { get; set; }
    }
}