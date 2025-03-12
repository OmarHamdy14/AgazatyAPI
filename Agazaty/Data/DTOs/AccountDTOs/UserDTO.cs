using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.AccountDTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? DepartmentName { get; set; }
        public int SickLeavesCount { get; set; }
        public int NormalLeavesCount { get; set; }
        public int CasualLeavesCount { get; set; }
        public int NormalLeavesCount_47 { get; set; }
        public int NormalLeavesCount_81Before3Years { get; set; }
        public int NormalLeavesCount_81Before2Years { get; set; }
        public int NormalLeavesCount_81Before1Years { get; set; }
        public int HowManyDaysFrom81And47 { get; set; }
        public int YearsOfWork { get; set; }
    }
}