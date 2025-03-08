using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.AccountDTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? DepartmentName { get; set; }
        public double SickLeavesCount { get; set; }
        public double NormalLeavesCount { get; set; }
        public double CasualLeavesCount { get; set; }
    }
}