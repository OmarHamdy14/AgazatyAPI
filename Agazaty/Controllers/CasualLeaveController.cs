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
        public async Task<ActionResult<CasualLeaveDTO>> GetCasualLeaveById(int leaveID)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });
            try
            {
                var casualLeave = await _base.Get(c => c.Id == leaveID);
                if (casualLeave == null)
                {
                    return NotFound(new { Message = "No Casual leave found." });
                }
                var leave = _mapper.Map<CasualLeaveDTO>(casualLeave);
                var user = await _accountService.FindById(leave.UserId);
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                return Ok(leave);
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
        public async Task<ActionResult<IEnumerable<CasualLeaveDTO>>> GetAllCasualLeaves()
        {
            try
            {
                var casualLeaves = await _base.GetAll();
                if (!casualLeaves.Any())
                {
                    return NotFound(new {Message = "There are no casual leaves found."});
                }
                var leaves = _mapper.Map<IEnumerable<CasualLeaveDTO>>(casualLeaves);
                foreach(var leave in leaves)
                {
                    var user = await _accountService.FindById(leave.UserId);
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                }
                return Ok(leaves);
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
        public async Task<ActionResult<IEnumerable<CasualLeaveDTO>>> GetAllCasualLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                var casualLeaves = await _base.GetAll(c => c.UserId == userID);
                if(!casualLeaves.Any())
                {
                    return NotFound(new {Message=$"There are no casual leaves found."});
                }
                var leaves = _mapper.Map<IEnumerable<CasualLeaveDTO>>(casualLeaves);
                foreach (var leave in leaves)
                {
                    var user = await _accountService.FindById(leave.UserId);
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                }
                return Ok(leaves);
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
        public async Task<ActionResult<IEnumerable<CasualLeaveDTO>>> GetAllCasualLeavesByUserIDAndYear(string userID, int year)
        {
            if (string.IsNullOrWhiteSpace(userID) || year < 1900)
                return BadRequest(new { message = "Invalid user ID or year." });
            try
            {
                var casualLeaves = await _base.GetAll(c => c.UserId == userID
                                  && c.Year == year);
                if (!casualLeaves.Any())
                {
                    return NotFound(new {Message="There are no casual leaves found."});
                }
                var leaves = _mapper.Map<IEnumerable<CasualLeaveDTO>>(casualLeaves);
                foreach (var leave in leaves)
                {
                    var user = await _accountService.FindById(leave.UserId);
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                }
                return Ok(leaves);
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
        public async Task<ActionResult<CasualLeaveDTO>> CreateCasualLeave([FromBody] CreateCasualLeaveDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (model == null)
                {
                    return BadRequest();
                }
                ApplicationUser user = await _accountService.FindById(model.UserId);
                if(((model.EndDate-model.StartDate).TotalDays + 1) > user.CasualLeavesCount)
                {
                    return BadRequest(new {Message = $"Request can't be done , The number of days available to you are {user.CasualLeavesCount}."});
                }
                if (model.EndDate >= DateTime.Today || model.StartDate >= DateTime.Today)
                {
                    return BadRequest(new { Message = "The leave period should be in the past." });
                }
                if((model.EndDate - model.StartDate).TotalDays > 1)
                {
                    return BadRequest(new { Message = "You have exceeded the allowed number of days, you can choose 1 or 2 days." });
                }
                if ((model.EndDate - model.StartDate).TotalDays < 0)
                {
                    return BadRequest(new {Message= "StartDate should be less than EndDate." });
                }


                var casualLeave = _mapper.Map<CasualLeave>(model);
                casualLeave.Year = model.StartDate.Year;
                casualLeave.RequestDate = DateTime.UtcNow.Date;
                user.CasualLeavesCount -= (int)((casualLeave.EndDate - casualLeave.StartDate).TotalDays + 1) ;
                await _accountService.Update(user);
                await _base.Add(casualLeave);

                var leave = _mapper.Map<CasualLeaveDTO>(casualLeave);
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                return CreatedAtAction(nameof(GetCasualLeaveById), new { leaveID = casualLeave.Id }, leave);
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
                 return BadRequest(new {Message="Invalid leave Id."});
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var casualLeave = await _base.Get(c => c.Id == leaveID);
                if (casualLeave == null)
                {
                    return NotFound(new { Message = "No Casuual leave found with this ID."});
                }

                var user = await _accountService.FindById(model.UserId);
                user.CasualLeavesCount += (int)((casualLeave.EndDate - casualLeave.StartDate).TotalDays + 1);

                if (((model.EndDate - model.StartDate).TotalDays + 1) > user.CasualLeavesCount)
                {
                    return BadRequest(new { Message = $"The number of days available to you are {user.CasualLeavesCount}." });
                }
                if (model.EndDate >= DateTime.Today || model.StartDate >= DateTime.Today)
                {
                    return BadRequest(new { Message = "The leave period should be in the past." });
                }
                if ((model.EndDate - model.StartDate).TotalDays > 1)
                {
                    return BadRequest(new { Message = "You have exceeded the allowed number of days." });
                }
                if ((model.EndDate - model.StartDate).TotalDays < 0)
                {
                    return BadRequest(new { Message = "StartDate should be less than EndDate." });
                }


                _mapper.Map(model, casualLeave);
                casualLeave.Year = model.StartDate.Year;
                user.CasualLeavesCount -= (int)((casualLeave.EndDate - casualLeave.StartDate).TotalDays + 1);
                await _accountService.Update(user);
                await _base.Update(casualLeave);

                var leave = _mapper.Map<CasualLeaveDTO>(casualLeave);
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                return Ok(new { Message = "Update is succeeded.", CasualLeaveDetails = leave});
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
            if (leaveID<=0)
            {
                return BadRequest(new { Message = "Invalid leave Id." });
            }
            try
            {
                var casualLeave = await _base.Get(c => c.Id == leaveID);
                if (casualLeave == null)
                {
                    return NotFound(new { Message = "No Casuual leave found with this ID." });
                }
                await _base.Remove(casualLeave);
                return Ok(new { Message = "Deletion is succeeded." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}