using Agazaty.Data.DTOs.RoleDTOs;
using Agazaty.Data.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Agazaty.Data.Services.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public RoleService(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public async Task<IdentityRole> FindById(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }
        public async Task<IdentityRole> FindByName(string RoleName)
        {
            return await _roleManager.FindByNameAsync(RoleName);
        }
        public async Task<IEnumerable<IdentityRole>> GetAllRoles()
        {
            return _roleManager.Roles;
        }
        public async Task<bool> IsRoleExisted(string RoleName)
        {
            var res = await _roleManager.RoleExistsAsync(RoleName);
            return res;
        }
        public async Task<IdentityResult> CreateRole(CreateRoleDTO model)
        {
            IdentityRole role = _mapper.Map<IdentityRole>(model);   
            var res = await _roleManager.CreateAsync(role);
            return res;
        }
        public async Task<IdentityResult> UpdateRole(IdentityRole role)
        {
            return await _roleManager.UpdateAsync(role);
        }
        public async Task<IdentityResult> DeleteRole(IdentityRole role)
        {
            return await _roleManager.DeleteAsync(role);
        }
    }
}
