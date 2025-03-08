using Agazaty.Data.Base;
using Agazaty.Data.DTOs.CasualLeaveDTOs;
using Agazaty.Data.DTOs.DepartmentDTOs;
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
    public class DepartmentController : ControllerBase
    {
        private readonly IEntityBaseRepository<Department> _base;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public DepartmentController(IMapper mapper, IEntityBaseRepository<Department> Ebase, IAccountService accountService)
        {
            _mapper = mapper;
            _base = Ebase;
            _accountService = accountService;
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetAllDepartments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                var departments = await _base.GetAll();

                if (!departments.Any())
                {
                    return NotFound("No departments found.");
                }
                var depts = _mapper.Map<IEnumerable<DepartmentDTO>>(departments);
                foreach(var dept in depts)
                {
                    var manager = await _accountService.FindById(dept.ManagerId);
                    dept.ManagerName = $"{manager.FirstName} {manager.SecondName} {manager.ThirdName} {manager.ForthName}";
                }
                return Ok(depts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetDepartmentById/{departmentID:int}")]
        public async Task<IActionResult> GetDepartmentById(int departmentID)
        {
            if (departmentID <= 0)
            {
                return BadRequest(new { Message = "Invalid department Id" });
            }
            try
            {
                var department = await _base.Get(d => d.Id == departmentID);

                if (department == null)
                {
                    return NotFound($"No department found with ID {departmentID}.");
                }

                var dept = _mapper.Map<DepartmentDTO>(department);
                var manager = await _accountService.FindById(dept.ManagerId);
                dept.ManagerName = $"{manager.FirstName} {manager.SecondName} {manager.ThirdName} {manager.ForthName}";
                return Ok(dept);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPost("CreateDepartment")]
        public async Task<IActionResult> CreateDepartment([FromBody]CreateDepartmentDTO model)
        {

            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid department data.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _accountService.FindById(model.ManagerId);
                if (res == null)
                {
                    return NotFound(new { Message = "Manager is not found" });
                }

                var department = _mapper.Map<Department>(model);
                await _base.Add(department);
                var manager = await _accountService.FindById(department.ManagerId);
                manager.Departement_ID = department.Id;
                await _accountService.Update(manager);

                var dept = _mapper.Map<DepartmentDTO>(department);
                dept.ManagerName = $"{manager.FirstName} {manager.SecondName} {manager.ThirdName} {manager.ForthName}";
                return CreatedAtAction(nameof(GetDepartmentById), new { departmentID = department.Id }, dept);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateDepartment/{departmentID:int}")]
        public async Task<IActionResult> UpdateDepartment([FromRoute]int departmentID, [FromBody]UpdateDepartmentDTO model)
        {
            if (departmentID<=0)
            {
                return BadRequest("Invalid department data.");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _accountService.FindById(model.ManagerId);
                if (res == null)
                {
                    return NotFound(new { Message = "Manager is not found" });
                }

                var department = await _base.Get(d => d.Id == departmentID);
                if (department == null)
                {
                    return NotFound(new { Message = "Department is not found." });
                }

                _mapper.Map(model, department);
                await _base.Update(department);

                var dept = _mapper.Map<DepartmentDTO>(department);
                var manager = await _accountService.FindById(dept.ManagerId);
                dept.ManagerName = $"{manager.FirstName} {manager.SecondName} {manager.ThirdName} {manager.ForthName}";
                return Ok(new { Message = $"Department has been successfully updated.", Department = dept });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpDelete("DeleteDepartment/{departmentID:int}")]
        public async Task<IActionResult> DeleteDepartment(int departmentID)
        {
            if (departmentID <= 0)
            {
                return BadRequest(new { Message = "Invalid department Id." });
            }

            try
            {
                var department = await _base.Get(d => d.Id == departmentID);

                if (department == null)
                {
                    return NotFound($"No department found.");
                }
                var users = await _accountService.GetAllUsersByDepartmentId(departmentID);
                if (users.Any())
                {
                    return BadRequest(new { Message = "This department has members, Edit their department to another one so you can delete this department." });
                }

                await _base.Remove(department);

                return Ok($"Department has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}