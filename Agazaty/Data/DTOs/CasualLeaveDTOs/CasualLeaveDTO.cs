using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.CasualLeaveDTOs
{
    public class CasualLeaveDTO
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Year { get; set; }
        public string UserName { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
