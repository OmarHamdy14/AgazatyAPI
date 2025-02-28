using Agazaty.Data.Base;
using Agazaty.Data.DTOs.CasualLeaveDTOs;
using Agazaty.Data.DTOs.DepartmentDTOs;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly IEntityBaseRepository<DepartmentsManagers> _baseDepartmentsManagers;
        public DepartmentController(IMapper mapper, IEntityBaseRepository<Department> Ebase, IAccountService accountService, IEntityBaseRepository<DepartmentsManagers> baseDepartmentsManagers)
        {
            _mapper = mapper;
            _base = Ebase;
            _accountService = accountService;
            _baseDepartmentsManagers = baseDepartmentsManagers;
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllDepartments")]
        public IActionResult GetAllDepartments()
        {
            try
            {
                var departments = _base.GetAll().ToList();

                if (!departments.Any())
                {
                    return NotFound("No departments found.");
                }

                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving departments.");
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetDepartmentById/{departmentID:int}")]
        public IActionResult GetDepartmentById(int departmentID)
        {
            if (departmentID <= 0)
            {
                return BadRequest();
            }
            try
            {
                var department = _base.Get(d => d.Id == departmentID);

                if (department == null)
                {
                    return NotFound($"No department found with ID {departmentID}.");
                }

                return Ok(department);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the department.");
            }
        }
        //[Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpPost("CreateDepartment")]
        public IActionResult CreateDepartment([FromBody]CreateDepartmentDTO model)
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
                //var res = _accountService.FindByNationalId(model.ManagerNationalNumber);
                //if (res == null)
                //{
                //    return NotFound(new { Message = "Manager National Number is not found" });
                //}
                var department = _mapper.Map<Department>(model);

                _base.Add(department);

                return CreatedAtAction(nameof(CreateDepartment), new { id = department.Id }, department);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the department.");
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpPut("UpdateDepartment/{departmentID:int}")]
        public IActionResult UpdateDepartment([FromRoute]int departmentID, [FromBody]UpdateDepartmentDTO model)
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
                //var res = _accountService.FindByNationalId(model.ManagerNationalNumber);
                //if (res == null)
                //{
                //    return NotFound(new { Message = "Manager National Number is not found" });
                //}

                var department = _base.Get(d => d.Id == departmentID);

                if (department == null)
                {
                    return NotFound($"Department with ID {departmentID} not found.");
                }

                //department = _mapper.Map<Department>(model);
                department.Name = model.Name;
                //department.ManagerNationalNumber = model.ManagerNationalNumber;

                _base.Update(department);

                return Ok($"Department {department.Id} has been successfully updated.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the department.");
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpDelete("DeleteDepartment/{departmentID:int}")]
        public async Task<IActionResult> DeleteDepartment(int departmentID)
        {
            if (departmentID <= 0)
            {
                return BadRequest();
            }

            try
            {
                var department = _base.Get(d => d.Id == departmentID);

                if (department == null)
                {
                    return NotFound($"No department found with ID {departmentID}.");
                }
                var entity = _baseDepartmentsManagers.Get(dm => dm.departmentId == department.Id);
                var manager = await _accountService.FindById(entity.managerid);
                manager.IsDepartmentManager = false;
                _baseDepartmentsManagers.Remove(entity);

                _base.Remove(department);

                return Ok($"Department {department.Id} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the department.");
            }
        }
    }
}