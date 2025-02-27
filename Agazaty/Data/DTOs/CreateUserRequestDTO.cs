using Agazaty.Models;
using Microsoft.AspNetCore.Identity;

namespace Agazaty.Data.DTOs
{
    public class CreateUserRequestDTO
    {
        public IEnumerable<IdentityRole> Roles { get; set; }
        public List<Department> Depts { get; set; }
    }
}
