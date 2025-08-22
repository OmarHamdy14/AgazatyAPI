using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.CasualLeaveDTOs
{
    public class UpdateCasualLeaveDTO
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}