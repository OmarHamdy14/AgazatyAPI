using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agazaty.Models
{
    public class PermitLeaveImage
    {
        public int Id { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [ForeignKey("PermitLeave")]
        public int LeaveId { get; set; }
        public PermitLeave PermitLeave { get; set; }
    }
}
