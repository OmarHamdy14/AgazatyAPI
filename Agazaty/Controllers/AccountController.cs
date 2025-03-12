using Agazaty.Data.Base;
using Agazaty.Data.DTOs.AccountDTOs;
using Agazaty.Data.Email.DTOs;
using Agazaty.Data.Enums;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

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
        private readonly IMapper _mapper;
        public AccountController(IMapper mapper, IAccountService accountService, SignInManager<ApplicationUser> signInManager, IRoleService roleService, IEntityBaseRepository<Department> deptBase)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _roleService = roleService;
            _deptBase = deptBase;
            _mapper = mapper;
        }
        
        [Authorize]
        [HttpPut("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO DTO)
        {
            var result = await _accountService.ResetPassword(DTO);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgetPassword(SendOTPDTO DTO)
        {
            var result = await _accountService.ForgetPassword(DTO.Email);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("Send-OTP")]
        public async Task<IActionResult> SendOtp(SendOTPDTO DTO)
        {
            var result = await _accountService.SendOTP(DTO.Email);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _accountService.FindById(userId);
            if (user == null)
                return NotFound("User not found.");

            var result = await _accountService.ChangePassword(user, model);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { Errors = errors });
            }
            return Ok("Password changed successfully.");
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
                    return NotFound(new { Message = "User is not found." });
                }

                var ReturnedUser = _mapper.Map<UserDTO>(user);
                ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                if (user.Departement_ID != null)
                {
                    var dept = await _deptBase.Get(d => d.Id == user.Departement_ID);
                    ReturnedUser.DepartmentName = dept.Name;
                }
                return Ok(ReturnedUser);
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
                var user = await _accountService.FindByNationalId(NationalId);
                if (user == null) return NotFound(new { Message = "User is not found." });

                var ReturnedUser = _mapper.Map<UserDTO>(user);
                ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                if (user.Departement_ID != null)
                {
                    var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                    ReturnedUser.DepartmentName = dpt.Name;
                }
                return Ok(ReturnedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllActiveUsers")]
        public async Task<IActionResult> GetAllActiveUsers()
        {
            try
            {
                var users = await _accountService.GetAllActiveUsers();
                if (!users.Any()) { return NotFound(new { Message = "There Are No Users Found."}); }

                var ReturnedUsers = _mapper.Map<IEnumerable<UserDTO>>(users);
                foreach(var ReturnedUser in ReturnedUsers)
                {
                    var user = await _accountService.FindById(ReturnedUser.Id);
                    ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    if (user.Departement_ID != null)
                    {
                        var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                        ReturnedUser.DepartmentName = dpt.Name;
                    }
                }
                return Ok(ReturnedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllNonActiveUsers")]
        public async Task<IActionResult> GetAllNonActiveUsers()
        {
            try
            {
                var users = await _accountService.GetAllNonActiveUsers();
                if (!users.Any()) { return NotFound(new { Message = "There Are No Users Found." }); }

                var ReturnedUsers = _mapper.Map<IEnumerable<UserDTO>>(users);
                foreach (var ReturnedUser in ReturnedUsers)
                {
                    var user = await _accountService.FindById(ReturnedUser.Id);
                    ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    if (user.Departement_ID != null)
                    {
                        var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                        ReturnedUser.DepartmentName = dpt.Name;
                    }
                }
                return Ok(ReturnedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("GetAllAvailabelCoworkers/{userId}")]
        public async Task<IActionResult> GetAllAvailableCoworkers(string userId)
        {
            try
            {
                var u = await _accountService.FindById(userId);
                if (u == null) return NotFound(new { Message = "User is not found" });

                var users = await _accountService.GetAllActiveAvailableCoworkers(u);
                if (!users.Any()) { return NotFound(new { Message = "There Are No Users Found." }); }

                var ReturnedUsers = _mapper.Map<IEnumerable<CoworkerDTO>>(users);
                foreach (var ReturnedUser in ReturnedUsers)
                {
                    var user = await _accountService.FindById(ReturnedUser.Id);
                    ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    if (user.Departement_ID != null)
                    {
                        var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                        ReturnedUser.DepartmentName = dpt.Name;
                    }
                }
                return Ok(ReturnedUsers);
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
                return BadRequest(new {Message = "Invalid Department Id."});
            try
            {
                var dept = await _deptBase.Get(d=>d.Id==DepartmentId);
                if (dept == null) return NotFound(new { Message = "There is no department found with this id." });

                var users = await _accountService.GetAllUsersByDepartmentId(DepartmentId);
                if(!users.Any()) return NotFound(new { Message = "There are no users found in this department." });

                var ReturnedUsers = _mapper.Map<IEnumerable<UserDTO>>(users);
                foreach (var ReturnedUser in ReturnedUsers)
                {
                    var user = await _accountService.FindById(ReturnedUser.Id);
                    ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    if (user.Departement_ID != null)
                    {
                        var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                        ReturnedUser.DepartmentName = dpt.Name;
                    }
                }
                return Ok(ReturnedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }        
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPost("CreateUser/{RoleName}")]
        public async Task<IActionResult> CreateUser([FromRoute]string RoleName, [FromBody] CreateUserDTO model) 
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(RoleName=="عميد الكلية" || RoleName == "أمين الكلية")
                    {
                        var list = await _accountService.GetAllUsersInRole(RoleName);
                        if (list.Any()) return BadRequest(new { Message = $"There is already user has the {RoleName} role, , this role should be only assigned to not more than 1 user." });
                    }
                    if (await _accountService.FindByEmail(model.Email) is not null)
                        return BadRequest(new { Message = "Email is already registered!" });

                    if (await _accountService.FindByNationalId(model.NationalID) is not null)
                        return BadRequest(new { Message = "NationalID is already registered!" });

                    if (await _accountService.FindByName(model.UserName) is not null)
                        return BadRequest(new { Message = "UserName is already registered!" });

                    if (!await _roleService.IsRoleExisted(RoleName))
                        return BadRequest(new { Message = "Invalid user ID or Role!" });

                    if (model.Departement_ID != null)
                    {
                        if (await _deptBase.Get(d => d.Id == model.Departement_ID) is null)
                            return BadRequest(new { Message = "Invalid department!" });
                    }

                    var res = await _accountService.Create(RoleName,model);
                    if (res.Succeeded)
                    {
                        var user = await _accountService.FindByNationalId(model.NationalID);
                        var ress = await _accountService.AddUserToRole(user, RoleName);
                        if (ress.Succeeded)
                        {
                            var ReturnedUser = _mapper.Map<UserDTO>(user);
                            ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                            if(user.Departement_ID != null)
                            {
                                var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                                ReturnedUser.DepartmentName = dpt.Name;
                            }
                            return CreatedAtAction(nameof(GetUserById), new { userID = user.Id }, ReturnedUser);
                        }
                        return BadRequest(ress.Errors);
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
        [AllowAnonymous]
        [HttpPost("UserLogin")]
        public async Task<IActionResult> Login([FromBody] LogInUserDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _accountService.FindByName(model.UserName);
                    bool checkPassword = await _accountService.CheckPassword(user, model.Password);
                    if (user is null || !checkPassword)
                    {
                        return Unauthorized(new { Message = "User Name or Password is incorrect!" });
                    }

                    var res = await _accountService.GetTokenAsync(user);
                    if (res.IsAuthenticated)
                    {
                        await _accountService.TransferingUserNormalLeaveCountToNewSection(user);
                        return Ok(res);
                    }
                    return Unauthorized(new {Message = res.Message});
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateUser/{userid}/{RoleName}")]
        public async Task<IActionResult> UdpateUser(string userid, string RoleName, [FromBody]UpdateUserDTO model)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(RoleName))
                return BadRequest(new { message = "Invalid user ID or Role." });


            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _accountService.FindById(userid);
                    if (user == null) return NotFound(new { Message = "User is not found" });

                    if (RoleName == "عميد الكلية" || RoleName == "أمين الكلية")
                    {
                        var list = await _accountService.GetAllUsersInRole(RoleName);
                        var manager = list.FirstOrDefault();    
                        if (manager != null)
                        {
                            if (manager.Id != userid) return BadRequest(new { Message = $"There is already user has the {RoleName} role, this role should be only assigned to not more than 1 user." });
                        }
                    }
                    var u = await _accountService.FindByEmail(model.Email);
                    if (u != null)
                    {
                        if (u.Id != userid)
                            return BadRequest(new { Message = "Email is already registered!" });
                    }

                    u = await _accountService.FindByNationalId(model.NationalID);
                    if (u != null)
                    {
                        if (u.Id != userid)
                            return BadRequest(new { Message = "NationalID is already registered!" });
                    }

                    u = await _accountService.FindByName(model.UserName);
                    if (u != null)
                    {
                        if (u.Id != userid)
                            return BadRequest(new { Message = "UserName is already registered!" });
                    }

                    if (!await _roleService.IsRoleExisted(RoleName))
                        return BadRequest(new { Message = "Invalid Role!" });

                    if (model.Departement_ID != null)
                    {
                        if (await _deptBase.Get(d => d.Id == model.Departement_ID) is null)
                            return BadRequest(new { Message = "Invalid department!" });
                    }

                    _mapper.Map(model, user);

                    var res = await _accountService.Update(user);
                    if (res.Succeeded)
                    {
                        var roles = await _accountService.GetAllRolesOfUser(user);
                        foreach (var role in roles)
                        {
                            await _accountService.RemoveUserFromRole(user, role);
                        }
                        await _accountService.AddUserToRole(user, RoleName);

                        var ReturnedUser = _mapper.Map<UserDTO>(user);
                        ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                        if (user.Departement_ID != null)
                        {
                            var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                            if(dpt!=null) ReturnedUser.DepartmentName = dpt.Name;
                        }
                        return Ok(new { Message = "Update is succeeded" , User = ReturnedUser });
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
        [Authorize]
        [HttpPut("UdpateUserForUser/{userid}")]
        public async Task<IActionResult> UdpateUserForUser(string userid, [FromBody] UpdateUserDTOforuser model)
        {
            if (string.IsNullOrWhiteSpace(userid))
                return BadRequest(new { message = "Invalid user ID." });

            try
            {
                if (ModelState.IsValid)
                {
                    //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _accountService.FindById(userid);
                    if (user == null) return NotFound(new { Message = "User is not found" });

                    var u = await _accountService.FindByEmail(model.Email);
                    if (u != null)
                    {
                        if (u.Id != userid)
                            return BadRequest(new { Message = "Email is already registered!" });
                    }

                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;

                    var res = await _accountService.Update(user);
                    if (res.Succeeded)
                    {
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
        [Authorize(Roles = "مدير الموارد البشرية")]
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
                    var IsDeptHead = await _deptBase.Get(d => d.ManagerId == user.Id);
                    if (IsDeptHead != null)
                    {
                        return BadRequest(new { Message = $"This user is {IsDeptHead.Name} department manager, you should assign a new manager to this department before deleting this user." });
                    }
                    user.Active = false;
                    return Ok(new { Message = "Deletion is succeeded" });
                    //var res = await _accountService.Delete(user);
                    //if (res.Succeeded)
                    //{
                    //    return Ok(new { Message = "Deletion is succeeded" });
                    //}
                    //return BadRequest(res.Errors);
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

