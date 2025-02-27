using Agazaty.Data.Base;
using Agazaty.Data.DTOs;
using Agazaty.Data.DTOs.AccountDTOs;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace Agazaty.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        IEntityBaseRepository<Department> _deptBase;
        public AccountController(IAccountService accountService, SignInManager<ApplicationUser> signInManager, IRoleService roleService, IEntityBaseRepository<Department> deptBase)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _roleService = roleService;
            _deptBase = deptBase;
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetUserById/{UserId}")]
        public async Task<IActionResult> GetUserById(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                var user = _accountService.FindById(userID);
                if (user == null) return NotFound(new { Message = "User is not found" });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetUserByNationalId/{NationalId}")]
        public async Task<IActionResult> GetUserByNationalId(string NationalId)
        {
            if (string.IsNullOrWhiteSpace(NationalId))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                var user = _accountService.FindByNationalId(NationalId);
                if (user == null) return NotFound(new { Message = "User is not found" });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = _accountService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllUsersByDepartmentId/{DepartmentId}")]
        public async Task<IActionResult> GetAllUsersByDepartmentId(int DepartmentId)
        {
            if (DepartmentId <= 0)
                return BadRequest();
            try
            {
                var users = _accountService.GetAllUsersByDepartmentId(DepartmentId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("InitalizeLeavesCountOfUser/{userid}")]
        public async Task<IActionResult> InitalizeLeavesCountOfUser(string userid)
        {
            try
            {
                var user = _accountService.FindById(userid);
               DateTime HireDate = DateTime.Parse(user.HireDate);
               DateTime DateOfBirth = DateTime.Parse(user.DateOfBirth);
                if ((HireDate - DateTime.UtcNow.Date).TotalDays >= 30 * 6)
                {
                    user.NormalLeavesCount = 15;
                    user.CasualLeavesCount = 7; 
                }
                else if (DateTime.UtcNow.Month==7 && DateTime.UtcNow.Day == 1)
                {
                    user.CasualLeavesCount = 7;
                    if ((HireDate - DateTime.UtcNow.Date).TotalDays >= 30 * 12) user.NormalLeavesCount = 28;
                    if ((HireDate - DateTime.UtcNow.Date).TotalDays >= 365 * 10) user.NormalLeavesCount = 37;
                    if ((DateOfBirth - DateTime.UtcNow.Date).TotalDays >= 365 * 50) user.NormalLeavesCount = 52;
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("RegsitrationPageNeededData")]
        public async  Task<IActionResult> CreateUserRequest()
        {
            var depts = _deptBase.GetAll().ToList();
            var roles = await _roleService.GetAllRoles();
            CreateUserRequestDTO model = new CreateUserRequestDTO()
            {
                Roles = roles,
                Depts = depts
            };
            return Ok(model);
        }
        //[HttpGet("LoginPageNeededData")]
        //public IActionResult Login()
        //{
        //    return Ok();
        //}
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpPost("CreateUser/{RoleName}")]
        public async Task<IActionResult> CreateUser([FromRoute]string RoleName, [FromBody] CreateUserDTO model) // Register
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = await _accountService.Create(RoleName,model);
                    if (res.IsAuthenticated)
                    {
                        var cookieOptions = new CookieOptions()
                        {
                            HttpOnly = true,
                            Expires = DateTime.Now.AddDays(1)
                        };
                        ApplicationUser user = _accountService.FindByNationalId(model.NationalID);
                        Response.Cookies.Append("UserName", user.UserName, cookieOptions);
                        Response.Cookies.Append("UserId", user.Id, cookieOptions);


                        await _accountService.AddUserToRole(user, RoleName);
                        await InitalizeLeavesCountOfUser(user.Id);
                        await _signInManager.SignInAsync(user, false);
                        return Ok(res);
                    }
                    return BadRequest(res.Message);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("UserLogin")]
        public async Task<IActionResult> Login([FromBody] LogInUserDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    var res = await _accountService.GetTokenAsync(model);
                    if (res.IsAuthenticated)
                    {
                        ApplicationUser applicationUser = await _accountService.FindByEmail(model.Email);
                        var cookieOptions = new CookieOptions()
                        {
                            Expires = DateTime.Now.AddHours(1),
                            HttpOnly = true
                        };
                        Response.Cookies.Append("UserName", applicationUser.UserName, cookieOptions);
                        Response.Cookies.Append("UserId", applicationUser.Id, cookieOptions);

                        await InitalizeLeavesCountOfUser(applicationUser.Id);
                        await _signInManager.SignInAsync(applicationUser, false);
                        return Ok(res);
                    }
                    return Unauthorized(res);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("UserLogout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Response.Cookies.Delete("UserName");
                await _signInManager.SignOutAsync();
                return Ok(new { Message = "Logout is succeeded" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpPost("AddUserToRole/{UserId}/{RoleName}")]
        public async Task<IActionResult> AddUserToRole(string UserId, string RoleName)
        {
            if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrWhiteSpace(RoleName))
                return BadRequest(new { message = "Invalid UserID or RoleName." });
            try
            {
                var user = _accountService.FindById(UserId);
                var res = await _accountService.AddUserToRole(user, RoleName);
                return Ok(new { Message = res });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpPut("UpdateUser/{UserId}/{RoleName}")]
        public async Task<IActionResult> UdpateUser(string userid, string RoleName, [FromBody]UpdateUserDTO model)
        {
            if (string.IsNullOrWhiteSpace(userid))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _accountService.FindById(userid);
                    if (user == null) return NotFound(new { Message = "User is not found" });


                    //var res = await _accountService.Update(userid, model);
                    user.UserName = model.UserName;
                    user.NationalID = model.NationalID;
                    user.HireDate = model.HireDate;
                    user.Active = true;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.PasswordHash = model.Password;
                    user.Position = model.Position;
                    user.DateOfBirth = model.DateOfBirth;
                    var res = await _accountService.Update(user);
                    if (res.Succeeded)
                    {
                        var roles = await _accountService.GetAllRolesOfUser(user);
                        foreach (var role in roles)
                        {
                            await _accountService.RemoveUserFromRole(user, role);
                        }
                        await _accountService.AddUserToRole(user, RoleName);
                        return Ok(new { Message = "Update is succeeded"});
                    }
                    return BadRequest(res.Errors);
                }
                return BadRequest(ModelState);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpDelete("DeleteUser/{userid}")]
        public async Task<IActionResult> DeleteUser(string userid)
        {
            if (string.IsNullOrWhiteSpace(userid))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                var user = _accountService.FindById(userid);
                if (user != null)
                {
                    var res = await _accountService.Delete(user);
                    if (res.Succeeded)
                    {
                        return Ok(new { Message = "Deletion is succeeded" });
                    }
                    return BadRequest(res.Errors);
                }
                return NotFound(new { Message = "User is not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}
