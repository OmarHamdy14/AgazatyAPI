using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.PermitLeavesDTOs
{
    public class CreatePermitLeaveDTO
    {
        [Required]
        public string EmployeeNationalNumber { get; set; }
        public double Hours { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }
}
