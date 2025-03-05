using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.CasualLeaveDTOs
{
    public class CreateCasualLeaveDTO
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
