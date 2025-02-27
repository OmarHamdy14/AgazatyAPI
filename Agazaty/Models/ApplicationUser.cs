using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agazaty.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FName { get; set; }
        public string SName { get; set; }
        public string TName { get; set; }
        public string LName { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        public int Position { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string HireDate { get; set; }
        [Required]
        public string NationalID { get; set; }
        public double SickLeavesCount { get; set; }
        [Range(0, double.MaxValue)]
        public double NormalLeavesCount { get; set; }
        [Range(0, double.MaxValue)]
        public double CasualLeavesCount { get; set; }
        [Required]
        [ForeignKey("Department")]
        public int? Departement_ID { get; set; }
        public Department? Department { get; set; }
        //public double PermitLeavesCount { get; set; }
    }
}
