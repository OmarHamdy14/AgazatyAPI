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
        private readonly IEntityBaseRepository<Department> _deptBase;
        private readonly IEntityBaseRepository<DepartmentsManagers> _baseDepartmentsManagers;

        public AccountController(IAccountService accountService, SignInManager<ApplicationUser> signInManager, IRoleService roleService, IEntityBaseRepository<Department> deptBase, IEntityBaseRepository<DepartmentsManagers> baseDepartmentsManagers)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _roleService = roleService;
            _deptBase = deptBase;
            _baseDepartmentsManagers = baseDepartmentsManagers;
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetUserById/{userID}")]
        public async Task<IActionResult> GetUserById([FromRoute]string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                var user = await _accountService.FindById(userID);
                if (user == null)
                {
                    return NotFound(new { Message = "User is not found" });
                }
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
                        ApplicationUser user = _accountService.FindByNationalId(model.NationalID);

                        // CheckFor : if this user is a manager of his department
                        if (user.IsDepartmentManager)
                        {
                            var isExisted = _baseDepartmentsManagers.Get(dm => dm.departmentId == user.Departement_ID);
                            if (isExisted != null)
                            {
                                return Ok(new AuthModel { Message = "This department is already has a manager" });
                            }
                            DepartmentsManagers DepartmentsManagers = new DepartmentsManagers()
                            {
                                managerid = user.Id,
                                departmentId = (int)user.Departement_ID
                            };
                            _baseDepartmentsManagers.Add(DepartmentsManagers);
                        }

                        var cookieOptions = new CookieOptions()
                        {
                            HttpOnly = true,
                            Expires = DateTime.Now.AddDays(1)
                        };
                        Response.Cookies.Append("UserName", user.UserName, cookieOptions);
                        Response.Cookies.Append("UserId", user.Id, cookieOptions);


                        await _accountService.AddUserToRole(user, RoleName);
                        //await InitalizeLeavesCountOfUser(user.Id);
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

                        // Handling leavesCounts
                        if (DateTime.UtcNow.Month == 7 && applicationUser.IntializationCheck)
                        {
                            await _accountService.InitalizeLeavesCountOfUser(applicationUser.Id);
                            applicationUser.IntializationCheck = false;
                        }
                        else if(DateTime.UtcNow.Month != 7)
                        {
                            applicationUser.IntializationCheck = true;
                        }

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
                var user = await _accountService.FindById(UserId);
                var res = await _accountService.AddUserToRole(user, RoleName);
                return Ok(new { Message = res });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpPut("UpdateUser/{userid}/{RoleName}")]
        public async Task<IActionResult> UdpateUser(string userid, string RoleName, [FromBody]UpdateUserDTO model)
        {
            if (string.IsNullOrWhiteSpace(userid))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _accountService.FindById(userid);
                    if (user == null) return NotFound(new { Message = "User is not found" });


                    //var res = await _accountService.Update(userid, model);
                    user.FName = model.FName;
                    user.SName = model.SName;
                    user.TName = model.TName;
                    user.LName = model.LName;
                    user.UserName = model.UserName;
                    user.NationalID = model.NationalID;
                    user.HireDate = model.HireDate;
                    user.Active = true;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    //user.PasswordHash = model.Password;
                    user.Position = model.Position;
                    user.DateOfBirth = model.DateOfBirth;
                    user.Departement_ID = model.Departement_ID;

                    if (user.IsDepartmentManager)
                    {
                        var entity = _baseDepartmentsManagers.Get(dm => dm.departmentId == user.Departement_ID);
                        var oldmanager = await _accountService.FindById(entity.managerid);
                        oldmanager.IsDepartmentManager = false;
                        await _accountService.Update(oldmanager);
                        entity.managerid = user.Id;
                        _baseDepartmentsManagers.Update(entity);
                    }
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
                var user = await _accountService.FindById(userid);
                if (user != null)
                {
                    var entity = _baseDepartmentsManagers.Get(dm => dm.managerid == user.Id);
                    _baseDepartmentsManagers.Remove(entity);
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
