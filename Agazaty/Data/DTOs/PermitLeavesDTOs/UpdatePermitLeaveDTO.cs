using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.PermitLeavesDTOs
{
    public class UpdatePermitLeaveDTO
    {
        //public int Id { get; set; }
        [Required]
        public string EmployeeNationalNumber { get; set; }
        public double Hours { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
