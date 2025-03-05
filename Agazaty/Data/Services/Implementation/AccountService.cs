using Agazaty.Data.Base;
using Agazaty.Data.DTOs.AccountDTOs;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Agazaty.Data.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEntityBaseRepository<Department> _baseDepartment;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext appDbContext, IMapper mapper, IOptions<JWT> jwt, IEntityBaseRepository<Department> baseDepartment)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _baseDepartment = baseDepartment;
        }
        public async Task<IdentityResult> ChangePassword(ApplicationUser user, ChangePasswordDTO model)
        {
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            return result;
        }
        public async Task<ApplicationUser> FindById(string UserId)
        {
            return await _userManager.FindByIdAsync(UserId);
        }
        public async Task<ApplicationUser> FindByNationalId(string NationalId)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.NationalID == NationalId);
        }
        public async Task<ApplicationUser> FindByName(string UserName)
        {
            return await _userManager.FindByNameAsync(UserName);
        }
        public async Task<ApplicationUser> FindByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await _appDbContext.Users.ToListAsync();/*.AsNoTracking();*/
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersInRole(string RoleName)
        {
            return await _userManager.GetUsersInRoleAsync(RoleName);
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersByDepartmentId(int DepartmentId)
        {
            return await _appDbContext.Users.Where(u => u.Departement_ID==DepartmentId).ToListAsync();
        }
        public async Task<IEnumerable<string>> GetAllRolesOfUser(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
        public async Task<string> GetDeanORSupervisor(string RoleName)
        {
            var res = await _userManager.GetUsersInRoleAsync(RoleName);
            return res.FirstOrDefault().Id;
        }
        public async Task InitalizeLeavesCountOfUser(string userid)
        {
            var user = await FindById(userid);
            user.CasualLeavesCount = 7;
            if ((user.HireDate - DateTime.UtcNow.Date).TotalDays >= 30 * 6) user.NormalLeavesCount = 15;
            if ((user.HireDate - DateTime.UtcNow.Date).TotalDays >= 28 * 12) user.NormalLeavesCount = 28;
            if ((user.HireDate - DateTime.UtcNow.Date).TotalDays >= 364 * 10) user.NormalLeavesCount = 37;
            if ((user.DateOfBirth - DateTime.UtcNow.Date).TotalDays >= 365 * 50) user.NormalLeavesCount = 52;
        }
        public async Task<bool> IsInRoleAsync(ApplicationUser user, string RoleName)
        {
            return await _userManager.IsInRoleAsync(user, RoleName);
        }
        public async Task<IdentityResult> RemoveUserFromRole(ApplicationUser user, string RoleName)
        {
            return await _userManager.RemoveFromRoleAsync(user, RoleName);
        }
        public async Task<IdentityResult> AddUserToRole(ApplicationUser user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            return result;
        }
        public async Task<bool> CheckPassword(ApplicationUser user, string passsword)
        {
            return await _userManager.CheckPasswordAsync(user, passsword);
        }
        public async Task<IdentityResult> Create(string RoleName, CreateUserDTO model)
        {
            var user = _mapper.Map<ApplicationUser>(model);
            user.Active = true;
            user.SickLeavesCount = 0;
            user.IntializationCheck = false;

            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
                //new Claim("UserID", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwt.DurationInHours),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<AuthModel> GetTokenAsync(ApplicationUser user)
        {
            var rolesList = await _userManager.GetRolesAsync(user);

            var jwtSecurityToken = await CreateJwtToken(user);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            //user.ActiveToken = tokenString;
            await Update(user);

            var authmodell = new AuthModel()
            {
                IsAuthenticated = true,
                Token = tokenString,
                //ExpiresOn = jwtSecurityToken.ValidTo,
                Roles = rolesList.ToList(),
                FullName = $"{user.ForthName} {user.ThirdName} {user.SecondName} {user.FirstName}",
                NormalLeavesCount = user.NormalLeavesCount,
                CasualLeavesCount = user.CasualLeavesCount,
                SickLeavesCount = user.SickLeavesCount
            };
            if (user.Departement_ID != null)
            {
                var dept = await _baseDepartment.Get(d => d.Id == user.Departement_ID);
                authmodell.DepartmentName = dept.Name;
            }
            return authmodell;
        }
        public async Task<IdentityResult> Update(ApplicationUser user)
        {
            
            return await _userManager.UpdateAsync(user);
        }
        public async Task<IdentityResult> Delete(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }
    }
}