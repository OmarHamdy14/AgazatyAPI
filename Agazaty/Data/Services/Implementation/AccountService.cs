using Agazaty.Data.Base;
using Agazaty.Data.DTOs.AccountDTOs;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using AutoMapper;
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
        //private readonly IEntityBaseRepository<DepartmentsManagers> _baseDepartmentsManagers;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext appDbContext, IMapper mapper, IOptions<JWT> jwt/*, IEntityBaseRepository<DepartmentsManagers> baseDepartmentsManagers*/)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            //_baseDepartmentsManagers = baseDepartmentsManagers;
        }
        public async Task<ApplicationUser> FindById(string UserId)
        {
            return await _userManager.FindByIdAsync(UserId);
            //return _appDbContext.Users.FirstOrDefault(u => u.Id == UserId);
        }
        public ApplicationUser FindByNationalId(string NationalId)
        {
            return _appDbContext.Users.FirstOrDefault(u => u.NationalID == NationalId);
        }
        public async Task<ApplicationUser> FindByName(string UserName)
        {
            return await _userManager.FindByNameAsync(UserName);
        }
        public async Task<ApplicationUser> FindByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return _appDbContext.Users.AsNoTracking();
        }
        public async Task<bool> IsInRoleAsync(ApplicationUser user, string RoleName)
        {
            return await _userManager.IsInRoleAsync(user, RoleName);
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersInRole(string RoleName)
        {
            return await _userManager.GetUsersInRoleAsync(RoleName);
        }
        public async Task<string> GetDeanORSupervisor(string RoleName)
        {
            var res = await _userManager.GetUsersInRoleAsync(RoleName);
            return res.FirstOrDefault().Id;
        }
        public async Task InitalizeLeavesCountOfUser(string userid)
        {
            var user = await FindById(userid);
            //DateTime HireDate = DateTime.Parse(user.HireDate);
            //DateTime DateOfBirth = DateTime.Parse(user.DateOfBirth);
                //if (creationProcess)
                //{
                //    user.CasualLeavesCount = 7;
                //    if ((HireDate - DateTime.UtcNow.Date).TotalDays >= 30 * 6) user.NormalLeavesCount = 15;
                //    if ((HireDate - DateTime.UtcNow.Date).TotalDays >= 30 * 12) user.NormalLeavesCount = 28;
                //    if ((HireDate - DateTime.UtcNow.Date).TotalDays >= 365 * 10) user.NormalLeavesCount = 37;
                //    if ((DateOfBirth - DateTime.UtcNow.Date).TotalDays >= 365 * 50) user.NormalLeavesCount = 52;
                //}
          /// if (DateTime.UtcNow.Month == 7 && DateTime.UtcNow.Day == 1)
           //{
                user.CasualLeavesCount = 7;
                if ((user.HireDate - DateTime.UtcNow.Date).TotalDays >= 30 * 6) user.NormalLeavesCount = 15;
                if ((user.HireDate - DateTime.UtcNow.Date).TotalDays >= 28 * 12) user.NormalLeavesCount = 28;
                if ((user.HireDate - DateTime.UtcNow.Date).TotalDays >= 364 * 10) user.NormalLeavesCount = 37;
                if ((user.DateOfBirth - DateTime.UtcNow.Date).TotalDays >= 365 * 50) user.NormalLeavesCount = 52;
           //}
        }
        public IEnumerable<ApplicationUser> GetAllUsersByDepartmentId(int DepartmentId)
        {
            return _appDbContext.Users.Where(u => u.Departement_ID==DepartmentId);
        }
        public async Task<IEnumerable<string>> GetAllRolesOfUser(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
        public async Task<IdentityResult> RemoveUserFromRole(ApplicationUser user, string RoleName)
        {
            return await _userManager.RemoveFromRoleAsync(user, RoleName);
        }
        public async Task<string> AddUserToRole(ApplicationUser user, string role)
        {
            //var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }
        public async Task<AuthModel> Create(string RoleName, CreateUserDTO model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (FindByNationalId(model.NationalID) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            if (!await _roleManager.RoleExistsAsync(RoleName))
                return new AuthModel { Message = "Invalid user ID or Role" };

            var user = _mapper.Map<ApplicationUser>(model);
            user.Active = true;
            //user.CasualLeavesCount = 7;
            user.IntializationCheck = false;

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }
            var res = await _userManager.AddToRoleAsync(user,RoleName);
            var jwtSecurityToken = await CreateJwtToken(user);

            

            return new AuthModel
            {
                Email = user.Email,
                UserId = user.Id,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { RoleName },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
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
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        public async Task<AuthModel> GetTokenAsync(LogInUserDTO model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.UserId = user.Id;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;
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
