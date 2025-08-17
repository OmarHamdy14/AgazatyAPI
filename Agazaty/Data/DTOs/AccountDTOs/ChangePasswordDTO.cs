using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.AccountDTOs
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "معرف المستخدم مطلوب.")]
        public string UseId { get; set; }
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "يجب أن تكون كلمة المرور على الأقل 8 أحرف.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب.")]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور غير متطابقة.")]
        public string ConfirmNewPassword { get; set; }
    }
}