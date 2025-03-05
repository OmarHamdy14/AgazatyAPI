using Agazaty.Data.DTOs.DepartmentDTOs;
using Agazaty.Data.DTOs.PermitLeavesDTOs;
using Agazaty.Data.DTOs.RoleDTOs;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Agazaty.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public RoleController(IRoleService roleService, IMapper mapper, IAccountService accountService)
        {
            _roleService = roleService;
            _mapper = mapper;
            _accountService = accountService;
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetRoleByID/{roleId}")]
        public async Task<IActionResult> GetRoleByID([FromRoute]string roleId)
        { 
            if (string.IsNullOrWhiteSpace(roleId))
                return BadRequest(new { message = "Invalid role ID." });
            try
            {
                var role = await _roleService.FindById(roleId);
                if(role==null) return NotFound(new {Message="No role with this id is found."});
                return Ok(_mapper.Map<RoleDTO>(role));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRoles();
                if (roles.Count() == 0) return NotFound(new {Message = "no roles found"});
                return Ok(_mapper.Map<IEnumerable<RoleDTO>>(roles)); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody]CreateRoleDTO CR)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(await _roleService.IsRoleExisted(CR.Name) == true)
                    {
                        return BadRequest(new {Message = "This role is already created"});
                    }
                    var res = await _roleService.CreateRole(CR);
                    if (res.Succeeded)
                    {
                        var role = await _roleService.FindByName(CR.Name);
                        return CreatedAtAction(nameof(GetRoleByID), new { roleId = role.Id }, _mapper.Map<RoleDTO>(role));
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
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateRole/{roleid}")]
        public async Task<IActionResult> UpdateRole([FromRoute]string roleid, [FromBody]UpdateRoleDTO UR)
        {
            if (string.IsNullOrWhiteSpace(roleid))
                return BadRequest(new { message = "Invalid role ID." });
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _roleService.IsRoleExisted(UR.Name) == true)
                    {
                        return BadRequest(new { Message = "This role is already created" });
                    }
                    var role = await _roleService.FindById(roleid);
                    if (role == null) return NotFound(new { Message = "There is no role with this ID." });

                    _mapper.Map(UR,role);

                    var res = await _roleService.UpdateRole(role);
                    if (res.Succeeded)
                    {
                        return Ok(new { Message = "Update is succeeded.", Role = _mapper.Map<RoleDTO>(role) });
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
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpDelete("DeleteRole/{roleid}")]
        public async Task<IActionResult> DeleteRole(string roleid)
        {
            if (string.IsNullOrWhiteSpace(roleid))
                return BadRequest(new { message = "Invalid role ID." });
            try
            {
                var role = await _roleService.FindById(roleid);
                if (role == null) return NotFound(new { Message = "There is no role with this ID." });

                var users = await _accountService.GetAllUsersInRole(roleid);
                if(users.Any()) return BadRequest(new {Message = "There are users with this role, edit their role to another one so you can delete this role."});  

                var res = await _roleService.DeleteRole(role);
                if (res.Succeeded)
                {
                    return Ok(new { Message = "Deletion is succeeded." });
                }
                return BadRequest(res.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}