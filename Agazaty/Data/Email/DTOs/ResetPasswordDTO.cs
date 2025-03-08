using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.Email.DTOs
{
    public class ResetPasswordDTO
    {
        [Required, EmailAddress]
        public string email { get; set; }
        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "OTP is 6 digits ")]

        public string token { get; set; }
        [Required]

        public string newPasswod { get; set; }
    }
}
