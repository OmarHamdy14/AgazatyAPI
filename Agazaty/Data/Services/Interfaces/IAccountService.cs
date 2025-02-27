using Agazaty.Data.DTOs.AccountDTOs;
using Agazaty.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Agazaty.Data.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ApplicationUser> FindById(string id);
        ApplicationUser FindByNationalId(string NationalId);
        Task<ApplicationUser> FindByName(string UserName);
        Task<ApplicationUser> FindByEmail(string email);
        IEnumerable<ApplicationUser> GetAllUsers();
        Task<IEnumerable<ApplicationUser>> GetAllUsersInRole(string RoleName);
        IEnumerable<ApplicationUser> GetAllUsersByDepartmentId(int DepartmentId);

        Task<bool> IsInRoleAsync(ApplicationUser user, string RoleName);
        Task<IdentityResult> RemoveUserFromRole(ApplicationUser user, string RoleName);
        Task<IEnumerable<string>> GetAllRolesOfUser(ApplicationUser user);
        Task<string> AddUserToRole(ApplicationUser user, string role);
        Task<AuthModel> Create(string RoleName, CreateUserDTO model);
        Task<string> GetDeanORSupervisor(string RoleName);
        Task<AuthModel> GetTokenAsync(LogInUserDTO model);
        Task<IdentityResult> Update(ApplicationUser user);
        Task<IdentityResult> Delete(ApplicationUser user);
        Task InitalizeLeavesCountOfUser(string userid);
    }
}
