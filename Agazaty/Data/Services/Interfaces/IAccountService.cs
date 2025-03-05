using Agazaty.Data.DTOs.AccountDTOs;
using Agazaty.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Agazaty.Data.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> ChangePassword(ApplicationUser user, ChangePasswordDTO model);
        Task<ApplicationUser> FindById(string id);
        Task<ApplicationUser> FindByNationalId(string NationalId);
        Task<ApplicationUser> FindByName(string UserName);
        Task<ApplicationUser> FindByEmail(string email);
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
        Task<IEnumerable<ApplicationUser>> GetAllUsersInRole(string RoleName);
        Task<IEnumerable<ApplicationUser>> GetAllUsersByDepartmentId(int DepartmentId);
        Task<IEnumerable<string>> GetAllRolesOfUser(ApplicationUser user);
        Task<string> GetDeanORSupervisor(string RoleName);
        Task InitalizeLeavesCountOfUser(string userid);
        Task<bool> IsInRoleAsync(ApplicationUser user, string RoleName);
        Task<IdentityResult> RemoveUserFromRole(ApplicationUser user, string RoleName);
        Task<IdentityResult> AddUserToRole(ApplicationUser user, string role);
        Task<bool> CheckPassword(ApplicationUser user, string passsword);
        Task<IdentityResult> Create(string RoleName, CreateUserDTO model);
        Task<AuthModel> GetTokenAsync(ApplicationUser user);
        Task<IdentityResult> Update(ApplicationUser user);
        Task<IdentityResult> Delete(ApplicationUser user);
    }
}
