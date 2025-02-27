using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agazaty.Models
{
    public class SickLeave
    {
        public int Id { get; set; }
        public string Disease { get; set; }
        public string EmployeeAddress { get; set; }
        public DateTime RequestDate { get; set; }
        public string? MedicalCommitteAddress { get; set; }
        public bool RespononseDone { get; set; }
        public int Year { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
    }
}
