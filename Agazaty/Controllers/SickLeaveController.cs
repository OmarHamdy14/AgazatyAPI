using Agazaty.Data.Base;
using Agazaty.Data.DTOs.CasualLeaveDTOs;
using Agazaty.Data.DTOs.SickLeaveDTOs;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agazaty.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SickLeaveController : ControllerBase
    {
        private readonly IEntityBaseRepository<SickLeave> _base;
        private readonly IMapper _mapper;
        public SickLeaveController(IMapper mapper, IEntityBaseRepository<SickLeave> Ebase)
        {
            _mapper = mapper;
            _base = Ebase;
        }
        [Authorize]
        [HttpGet("GetSickLeaveById/{leaveID:int}")]
        public IActionResult GetSickLeaveById(int leaveID)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });
            try
            {

                var sikLeave = _base.Get(s => s.Id == leaveID);
                if (sikLeave == null)
                {
                    return NotFound(new { message = $"No sick leave found for this leave ID {leaveID}." });
                }
                return Ok(sikLeave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("GetAllSickLeavesByUserID/{userID}")]
        public IActionResult GetAllSickLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {

                var sickleaves = _base.GetAll(s => s.UserID == userID).ToList();
                if (sickleaves.Count == 0)
                {
                    return NotFound(new { message = $"No sick leaves found for this User ID {userID}." });
                }
                return Ok(sickleaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllSickLeave")]
        public IActionResult GetAllSickLeave()
        {
            try
            {
                var sickLeaves = _base.GetAll().ToList();
                return Ok(sickLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving sick leaves.");
            }
        }
        [Authorize]
        [HttpGet("GetAllSickLeavesByUserIDAndYear/{userID}/{year:int}")]
        public IActionResult GetAllSickLeavesByUserIDAndYear(string userID, int year)
        {
            if (string.IsNullOrWhiteSpace(userID) || year < 1900)
            {
                return BadRequest("Invalid user ID or year.");
            }

            try
            {
                var sickLeaves = _base.GetAll(s => s.UserID == userID && s.Year == year).ToList();

                if (sickLeaves.Any())
                {
                    return Ok(sickLeaves);
                }

                return NotFound("No sick leaves found for the given user ID and year.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving sick leaves.");
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetAllWaitingSickLeaves")]
        public IActionResult GetAllWaitingSickLeaves()
        {
            try
            {
                var waitingSickLeaves = _base.GetAll(s => s.RespononseDone == false).ToList();

                if (waitingSickLeaves.Any())
                {
                    return Ok(waitingSickLeaves);
                }

                return NotFound("No waiting sick leaves found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving waiting sick leaves.");
            }
        }
        [Authorize]
        [HttpPost("CreateSickLeave")]
        public IActionResult CreateSickLeave([FromBody]CreateSickLeaveDTO model)
        {
            try
            {
                if (model == null)
                {
                    return NotFound("Invalid sick leave data.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                SickLeave sickLeave = _mapper.Map<SickLeave>(model);

                _base.Add(sickLeave);
                return CreatedAtAction(nameof(CreateSickLeave), new { id = sickLeave.Id }, sickLeave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred.");
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateMedicalCommiteAddressResponse/{leaveID:int}/{address}")]
        public IActionResult UpdateMedicalCommiteAddressResponse(int leaveID, string address)
        {
            if (leaveID <= 0 || string.IsNullOrWhiteSpace(address))
            {
                return BadRequest("Invalid leave ID or address.");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sickLeave = _base.Get(s => s.Id == leaveID);

                if (sickLeave == null)
                {
                    return NotFound("Sick leave request not found.");
                }

                // Update fields
                sickLeave.MedicalCommitteAddress = address;
                sickLeave.RespononseDone = true;

                _base.Update(sickLeave);

                return Ok("Medical committee address updated and response marked as done.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the sick leave request.");
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateSickLeave/{leaveID}")]
        public IActionResult UpdateSickLeave(int leaveID, [FromBody]UpdateSickLeaveDTO model)
        {

            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid sick leave data.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sickleave = _base.Get(s => s.Id == leaveID);

                if (sickleave == null)
                {
                    return NotFound("SickLeave not found.");
                }

                //sickleave = _mapper.Map<SickLeave>(model);
                sickleave.Disease = model.Disease;
                sickleave.EmployeeAddress = model.EmployeeAddress;

                _base.Update(sickleave);

                return Ok(sickleave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the sick leave.");
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpDelete("DeleteSickLeave/{leaveID}")]
        public IActionResult DeleteSickLeave(int leaveID)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });
            try
            {
                var sickleave = _base.Get(s => s.Id == leaveID);

                if (sickleave == null)
                {
                    return NotFound("Sick leave not found.");
                }

                _base.Remove(sickleave);
                return Ok($"Sick leave {sickleave.Id} for user {sickleave.UserID} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the sick leave.");
            }
        }
    }
}