using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.PermitLeavesDTOs
{
    public class PermitLeaveImageDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int LeaveId { get; set; }
    }
}
