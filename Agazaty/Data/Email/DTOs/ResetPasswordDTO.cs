using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.Email.DTOs
{
    public class ResetPasswordDTO
    {
        //from front
        [Required, EmailAddress]
        public string email { get; set; }
        [Required]
        public string newPassword { get; set; }
    }
}