using System.ComponentModel.DataAnnotations;

namespace Agazaty.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        [Required]
        public string ManagerNationalNumber { get; set; }
    }
}
