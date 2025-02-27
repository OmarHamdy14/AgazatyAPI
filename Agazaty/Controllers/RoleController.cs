using Agazaty.Data.DTOs.RoleDTOs;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
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
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetRoleByID/{roleId}")]
        public async Task<IActionResult> GetRoleByID([FromRoute]string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return BadRequest(new { message = "Invalid role ID." });
            try
            {
                var role = await _roleService.FindById(roleId);
                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            try { 
                var roles = _roleService.GetAllRoles(); 
                return Ok(roles); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody]CreateRoleDTO CR)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityResult res = await _roleService.CreateRole(CR);
                    if (res.Succeeded)
                    {
                        return CreatedAtAction("GetAllRoles", new { });
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
        [HttpPut("UpdateRole/{roleid}")]
        public async Task<IActionResult> UpdateRole([FromRoute]string roleid, [FromBody]UpdateRoleDTO UR)
        {
            if (string.IsNullOrWhiteSpace(roleid))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                if (ModelState.IsValid)
                {
                    var role = await _roleService.FindById(roleid);
                    if (role == null) return NotFound(new { Message = "There is no role with this ID" });
                    role.Name = UR.Name;
                    var res = await _roleService.UpdateRole(role);
                    if (res.Succeeded)
                    {
                        return Ok(new { Message = "Update is succeeded" });
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
        [HttpDelete("DeleteRole/{roleid}")]
        public async Task<IActionResult> DeleteRole(string roleid)
        {
            if (string.IsNullOrWhiteSpace(roleid))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                var role = await _roleService.FindById(roleid);
                if (role == null) return NotFound(new { Message = "There is no role with this ID" });

                var res = await _roleService.DeleteRole(role);
                if (res.Succeeded)
                {
                    return Ok(new { Message = "Delete is succeeded" });
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