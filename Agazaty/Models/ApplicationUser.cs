using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agazaty.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FName { get; set; }
        [Required]
        public string SName { get; set; }
        [Required]
        public string TName { get; set; }
        [Required]
        public string LName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public int Position { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime HireDate { get; set; }
        [Required]
        public string NationalID { get; set; }
        public double SickLeavesCount { get; set; }
        [Range(0, double.MaxValue)]
        public double NormalLeavesCount { get; set; }
        [Range(0, double.MaxValue)]
        public double CasualLeavesCount { get; set; }
        public double PermitLeavesCount { get; set; }
        [DefaultValue(true)]
        public bool IntializationCheck { get; set; }
        [Required]
        [ForeignKey("Department")]
        public int? Departement_ID { get; set; }
        public Department? Department { get; set; }
        [Required]
        public bool IsDepartmentManager { get; set; }
    }
}
