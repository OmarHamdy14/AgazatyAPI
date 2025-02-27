using Agazaty.Data.Base;
using Agazaty.Data.DTOs.CasualLeaveDTOs;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

namespace Agazaty.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CasualLeaveController : ControllerBase
    {
        private readonly IEntityBaseRepository<CasualLeave> _base;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public CasualLeaveController(IMapper mapper,IEntityBaseRepository<CasualLeave> Ebase, IAccountService accountService)
        {
            _mapper = mapper;
            _base = Ebase;
            _accountService = accountService;
        }
        [Authorize]
        [HttpGet("GetCasualLeaveById/{leaveID:int}", Name = "GetCasualLeave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CasualLeave>> GetCasualLeaveById(int leaveID)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });
            try
            {
                var casualLeave = _base.Get(c => c.Id == leaveID);
                if (casualLeave == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<CasualLeave>(casualLeave));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllCasualLeaves", Name = "GetAllCasualLeaves")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CasualLeave>>> GetAllCasualLeaves()
        {
            try
            {
                var casualLeaves = _base.GetAll().ToList();
                if (casualLeaves == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<CasualLeave>>(casualLeaves));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("GetAllCasualLeavesByUserID/{userID}", Name = "GetAllCasualLeavesByUserID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CasualLeave>>> GetAllCasualLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                var casualLeaves = _base.GetAll(c => c.UserId == userID).ToList();
                if (casualLeaves == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<CasualLeave>>(casualLeaves));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("GetAllCasualLeavesByUserIDAndYear/{userID}/{year:int}", Name = "GetAllCasualLeavesByUserIDAndYear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CasualLeave>>> GetAllCasualLeavesByUserIDAndYear(string userID, int year)
        {
            try
            {
                var casualLeaves = _base.GetAll(c => c.UserId == userID
                                  && c.Year == year).ToList();
                if (casualLeaves == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<CasualLeave>>(casualLeaves));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("CreateCasualLeave", Name = "CreateCasualLeave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CasualLeave>> CreateCasualLeave([FromBody] CreateCasualLeaveDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                ApplicationUser user = await  _accountService.FindById(model.UserId);
                if((model.EndDate-model.StartDate).TotalDays > user.CasualLeavesCount)
                {
                    return Ok(new {Message = $"Request can't be done , The number of days available to you are {user.CasualLeavesCount}" });
                }
                if (model == null)
                {
                    return BadRequest();
                }
                if (model.EndDate >= DateTime.Today || model.StartDate >= DateTime.Today)
                {
                    return BadRequest(new { Message = "The leave period should be in the past." });
                }
                if((model.EndDate - model.StartDate).TotalDays > 1)
                {
                    return BadRequest(new { Message = "You have exceeded the allowed number of days " });
                }
                if ((model.EndDate - model.StartDate).TotalDays < 0)
                {
                    return BadRequest(new {Message= "StartDate should be less than EndDate " });
                }


                var casualLeave = _mapper.Map<CasualLeave>(model);
                user.CasualLeavesCount -= (casualLeave.EndDate - casualLeave.StartDate).TotalDays;
                _base.Add(casualLeave);
                return Ok(casualLeave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpPut("UpdateCasualLeave/{leaveID:int}", Name = "UpdateCasualLeave")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCasualLeave(int leaveID, [FromBody] UpdateCasualLeaveDTO model)
        {
            if (leaveID <= 0)
            {
                 return BadRequest();
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var casualLeave = _base.Get(c => c.Id == leaveID);
                if (casualLeave == null)
                {
                    return NotFound();
                }
                ApplicationUser user = await  _accountService.FindById(model.UserId);
                if ((model.EndDate - model.StartDate).TotalDays > user.CasualLeavesCount)
                {
                    return Ok(new { Message = $"The number of days available to you are {user.CasualLeavesCount}" });
                }
                //casualLeave = _mapper.Map<CasualLeave>(model);
                casualLeave.EndDate = model.EndDate;
                casualLeave.StartDate = model.StartDate;
                casualLeave.UserId = model.UserId;
                casualLeave.Year = model.Year;
                _base.Update(casualLeave);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpDelete("DeleteCasualLeave/{leaveID:int}", Name = "DeleteCasualLeave")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCasualLeave(int leaveID)
        {
            try
            {
                if (leaveID<=0)
                {
                    return BadRequest();
                }
                var casualLeave = _base.Get(c => c.Id == leaveID);
                if (casualLeave == null)
                {
                    return NotFound();
                }
                _base.Remove(casualLeave);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}
