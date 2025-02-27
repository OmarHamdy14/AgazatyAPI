using Agazaty.Data.DTOs.RoleDTOs;
using Microsoft.AspNetCore.Identity;

namespace Agazaty.Data.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IdentityRole> FindById(string id);
        Task<IEnumerable<IdentityRole>> GetAllRoles();
        Task<IdentityResult> CreateRole(CreateRoleDTO model);
        Task<IdentityResult> UpdateRole(IdentityRole role);
        Task<IdentityResult> DeleteRole(IdentityRole role);
    }
}
