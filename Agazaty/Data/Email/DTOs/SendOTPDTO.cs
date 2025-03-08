using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.Email.DTOs
{
    public class SendOTPDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
