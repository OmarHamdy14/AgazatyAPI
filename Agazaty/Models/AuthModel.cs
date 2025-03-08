using System.ComponentModel.DataAnnotations;

namespace Agazaty.Models
{
    public class AuthModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
        public double SickLeavesCount { get; set; }
        public double NormalLeavesCount { get; set; }
        public double CasualLeavesCount { get; set; }
        public double PermitLeavesCount { get; set; }
        public List<string> Roles { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        //public DateTime ExpiresOn { get; set; }
    }
}
