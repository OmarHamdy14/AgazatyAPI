using Agazaty.Data.Base;
using Agazaty.Data.DTOs.NormalLeaveDTOs;
using Agazaty.Data.Enums;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NormalLeaveTask.Models;
using System.Data;

namespace Agazaty.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NormalLeaveController : ControllerBase
    {
        private readonly IEntityBaseRepository<NormalLeave> _base;
        private readonly IAccountService _accountService;
        private readonly IEntityBaseRepository<Department> _departmentBase;  
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        public NormalLeaveController(AppDbContext appDbContext, IEntityBaseRepository<NormalLeave> Ebase, IAccountService accountService, IMapper mapper, IEntityBaseRepository<Department> departmentBase)
        {
            _base = Ebase;
            _accountService = accountService;
            _mapper = mapper;   
            _appDbContext = appDbContext;
            _departmentBase = departmentBase;
        }
        [Authorize]
        [HttpGet("GetNormalLeaveById/{leaveID:int}")]
        public IActionResult GetNormalLeaveById(int leaveID)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });
            try
            {

                var NormalLeave = _base.Get(n => n.ID == leaveID);
                if (NormalLeave == null)
                {
                    return NotFound(new { message = $"No normal leave found for this leave ID {leaveID}." });
                }
                return Ok(NormalLeave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        // GetNormalLeaveByIdAndUserId

        [Authorize]
        [HttpGet("AllNormalLeavesByUserId/{userID}")]
        public IActionResult GetAllNormalLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {

                var NormalLeaves = _base.GetAll(n => n.UserID == userID).ToList();
                if (NormalLeaves.Count == 0)
                {
                    return NotFound(new { message = $"No normal leaves found for this User ID {userID}." });
                }
                return Ok(NormalLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("AcceptedByUserId/{userID}")]
        public IActionResult GetAllAcceptedNormalLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {

                var NormalLeaves = _base.GetAll(n => n.UserID == userID &&
                    n.Accepted == true &&
                    n.ResponseDone == true)
                    .ToList();
                if (NormalLeaves.Count == 0)
                {
                    return NotFound(new { message = $"No accepted normal leaves found for this User ID {userID}." });
                }
                return Ok(NormalLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("AcceptedByUserIdAndYear/{userID}/{year:int}")]
        public IActionResult GetAllAcceptedNormalLeavesByUserIDAndYear(string userID, int year)
        {
            try
            {
                var errors = new List<string>();
                int currentYear = DateTime.Now.Year;
                if (string.IsNullOrWhiteSpace(userID))
                    errors.Add("Invalid user ID.");
                if (year <= 0)
                    errors.Add("Invalid year.");
                else if (year > currentYear)
                    errors.Add($"Year cannot be older than the current year ({currentYear}).");
                if (errors.Any())
                    return BadRequest(new { messages = errors });
                var NormalLeaves = _base.GetAll(n =>
                    n.UserID == userID &&
                    n.Year == year &&
                    n.Accepted == true &&
                    n.ResponseDone == true)
                    .ToList();
                if (NormalLeaves.Count == 0)
                {
                    return NotFound(new { message = $"No accepted normal leaves found for this user ID {userID} in {year}." });
                }
                return Ok(NormalLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("RejectedByUserId/{userID}")]
        public IActionResult GetAllRejectedNormalLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {

                var NormalLeaves = _base.GetAll(n =>
                    n.UserID == userID &&
                    n.Accepted == false &&
                    n.ResponseDone == true)
                    .ToList();
                if (NormalLeaves.Count == 0)
                {
                    return NotFound(new { message = $"No rejected normal leaves found for this User ID {userID}." });
                }
                return Ok(NormalLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("WaitingByUserID/{userID}")]
        public IActionResult GetAllWaitingNormalLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {

                var NormalLeaves = _base.GetAll(n =>
                    n.UserID == userID &&
                    n.ResponseDone == false)
                    .ToList();
                if (NormalLeaves.Count == 0)
                {
                    return NotFound(new { message = $"No waiting normal leaves found for this User ID {userID}." });
                }
                return Ok(NormalLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية")]
        [HttpGet("WaitingByGeneral_ManagerID/{general_managerID}")]
        public IActionResult GetAllWaitingNormalLeavesByGeneral_ManagerID(string general_managerID)
        {
            if (string.IsNullOrWhiteSpace(general_managerID))
                return BadRequest(new { message = "Invalid general manager ID." });
            try
            {

                var NormalLeaves = _base.GetAll(n =>
                    n.General_ManagerID == general_managerID &&
                    n.GeneralManager_Decision == false &&
                    n.DirectManager_Decision == true &&
                    n.CoWorker_Decision == true &&
                    n.ResponseDone == false)
                    .ToList();
                if (NormalLeaves.Count == 0)
                {
                    return NotFound(new { message = $"No waiting normal leaves found for this general manager id {general_managerID}." });
                }
                return Ok(NormalLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("WaitingByDirect_ManagerID/{direct_managerID}")]
        public IActionResult GetAllWaitingNormalLeavesByDirect_ManagerID(string direct_managerID)
        {
            if (string.IsNullOrWhiteSpace(direct_managerID))
                return BadRequest(new { message = "Invalid direct manager ID." });
            try
            {

                var NormalLeaves = _base.GetAll(n =>
                    n.Direct_ManagerID == direct_managerID &&
                    n.DirectManager_Decision == false &&
                    n.CoWorker_Decision == true &&
                    n.ResponseDone == false)
                    .ToList();
                if (NormalLeaves.Count == 0)
                {
                    return NotFound(new { message = $"No waiting normal leaves found for this direct manager id {direct_managerID}." });
                }
                return Ok(NormalLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("WaitingByCoWorkerID/{coworkerID}")]
        public IActionResult GetAllWaitingNormalLeavesByCoWorkerID(string coworkerID)
        {
            if (string.IsNullOrWhiteSpace(coworkerID))
                return BadRequest(new { message = "Invalid coworker ID." });
            try
            {

                var NormalLeaves = _base.GetAll(n =>
                    n.Coworker_ID == coworkerID &&
                    n.CoWorker_Decision == false &&
                    n.ResponseDone == false)
                    .ToList();
                if (NormalLeaves.Count == 0)
                {
                    return NotFound(new { message = $"No waiting normal leaves found for this coworker id {coworkerID}." });
                }
                return Ok(NormalLeaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية,هيئة تدريس,موظفين")]
        [HttpGet("GetLeaveTypes")]
        public IActionResult GetLeaveTypes()
        {
            //var leaveTypes = Enum.GetValues(typeof(LeaveTypes.Leaves)).Cast<LeaveTypes.Leaves>();
            return Ok(LeaveTypes.res);
        }
        [Authorize]
        [HttpPost("CreateNormalLeave")]
        public async Task<IActionResult> CreateNormalLeave([FromBody]CreateNormalLeaveDTO model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { message = "Invalid normal leave data." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _accountService.FindById(model.UserID);
                if ((model.EndDate - model.StartDate).TotalDays > user.CasualLeavesCount)
                {
                    return Ok(new { Message = $"The number of days available to you are {user.NormalLeavesCount}" });
                }

                var errors = new List<string>();
                DateTime today = DateTime.Today;
                int year = DateTime.Now.Year;

                // Check if the user already has a pending leave request
                bool hasPendingLeave = _appDbContext.NormalLeaves
                .Any(l => l.UserID == model.UserID && l.ResponseDone == false);
                if (hasPendingLeave)
                {
                    return BadRequest(new { message = "You already have a pending leave request that has not been processed yet." });
                }

                // Check if the new leave period overlaps with any existing approved leave
                bool hasOverlappingLeave = _appDbContext.NormalLeaves
                    .Any(l => l.UserID == model.UserID && l.Accepted == true &&
                              !(model.EndDate < l.StartDate || model.StartDate > l.EndDate));
                // This ensures that the new leave period does NOT completely fall outside an existing leave period
                if (hasOverlappingLeave)
                {
                    return BadRequest(new { message = "Your requested leave period overlaps with an existing approved leave." });
                }

                //validation on year
                if (model.EndDate < today)
                    errors.Add("The leave period has already passed. Please select future dates.");

                if (model.StartDate < today)
                    errors.Add("The start date cannot be in the past. Please select today or a future date.");

                if (model.StartDate > model.EndDate)
                    errors.Add("Start date cannot be after the end date.");

                if (model.RequestDate > model.StartDate)
                    errors.Add("Request date cannot be after the start date.");

                if (errors.Any())
                    return BadRequest(new { messages = errors });

                var normalLeave = _mapper.Map<NormalLeave>(model);
                if (await _accountService.IsInRoleAsync(user, "Staff"))
                {
                    var res = await _accountService.GetAllUsersInRole("Dean");
                    var Dean = res.FirstOrDefault();
                    normalLeave.General_ManagerID = Dean.Id;

                    var dept = _departmentBase.Get(d => d.Id == user.Departement_ID);
                    var DepartmentHead = _accountService.FindByNationalId(dept.ManagerNationalNumber);
                    normalLeave.Direct_ManagerID = _accountService.FindByNationalId(dept.ManagerNationalNumber).Id;
                }
                else if(await _accountService.IsInRoleAsync(user, "Employee"))
                {
                    var res = await _accountService.GetAllUsersInRole("Supervisor");
                    var Dean = res.FirstOrDefault();
                    normalLeave.General_ManagerID = res.FirstOrDefault().Id;

                    var dept = _departmentBase.Get(d => d.Id == user.Departement_ID);
                    var DepartmentHead = _accountService.FindByNationalId(dept.ManagerNationalNumber);
                    normalLeave.Direct_ManagerID = _accountService.FindByNationalId(dept.ManagerNationalNumber).Id;
                }
                else if(await _accountService.IsInRoleAsync(user, "Supervisor"))
                {
                    // if أمين الكلية made a leave request
                    var res = await _accountService.GetAllUsersInRole("Dean");
                    var Dean = res.FirstOrDefault();
                    normalLeave.General_ManagerID = Dean.Id;
                    normalLeave.Direct_ManagerID = Dean.Id;
                }
                else if (await _accountService.IsInRoleAsync(user, "HR"))
                {
                    var res = await _accountService.GetAllUsersInRole("Supervisor");
                    var super = res.FirstOrDefault();
                    normalLeave.General_ManagerID = super.Id;
                    normalLeave.Direct_ManagerID = super.Id;
                }
                _base.Add(normalLeave);

                return CreatedAtAction(nameof(GetNormalLeaveById), new { leaveID = normalLeave.ID }, normalLeave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateNormalLeave/{leaveID:int}")]
        public IActionResult UpdateNormalLeave(int leaveID, [FromBody] UpdateNormalLeaveDTO model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { message = "Invalid request data." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var NormalLeave = _base.Get(n =>
                    n.ID == leaveID &&
                    n.ResponseDone == true &&
                    n.Accepted == true
                    );

                if (NormalLeave == null)
                {
                    return NotFound(new { message = "Normal Leave not found or not eligible for update" });
                }
                DateTime today = DateTime.Today;
                var errors = new List<string>();

                if (model.EndDate < today)
                    errors.Add("End date cannot be in the past.");

                if (NormalLeave.StartDate > model.EndDate)
                    errors.Add("Start date cannot be after the new end date.");

                if (errors.Any())
                    return BadRequest(new { messages = errors });
                // Update properties
                ApplicationUser user = _accountService.FindById(NormalLeave.UserID);
                if ((model.EndDate - NormalLeave.StartDate).TotalDays > user.CasualLeavesCount)
                {
                    return Ok(new { Message = $"The number of days available to you are {user.NormalLeavesCount}" });
                }

                //NormalLeave = _mapper.Map<NormalLeave>(model);
                NormalLeave.NotesFromEmployee = model.NotesFromEmployee;
                user.NormalLeavesCount += (NormalLeave.EndDate - model.EndDate).TotalDays;
                NormalLeave.EndDate = model.EndDate;

                _base.Update(NormalLeave);

                return Ok(new
                {
                    message = "Normal Leave updated successfully",
                    leaveID = NormalLeave.ID,
                    userID = NormalLeave.UserID,
                    newEndDate = NormalLeave.EndDate,
                    notes = NormalLeave.NotesFromEmployee
                    //update number of Normal leaves's user 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating", error = ex.Message });
            }

        }
        [Authorize(Roles = "عميد الكلية,أمين الكلية")]
        [HttpPut("UpdateGeneralManagerDecision/{leaveID:int}")]
        public IActionResult UpdateGeneralManagerDecision(int leaveID, [FromBody] GeneralManagerDecisionDTO model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { message = "Invalid request data." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var NormalLeave = _base.Get(n =>
                    n.ID == leaveID &&
                    n.CoWorker_Decision == true &&
                    n.DirectManager_Decision == true &&
                    n.ResponseDone == false
                    );

                if (NormalLeave == null)
                {
                    return NotFound(new { message = "Normal Leave not found or not eligible for update" });
                }
                ApplicationUser user = _accountService.FindById(NormalLeave.UserID);
                // Update properties
                NormalLeave.GeneralManager_Decision = model.GeneralManagerDecision;
                NormalLeave.ResponseDone = true;
                if (model.GeneralManagerDecision == true)
                {
                    NormalLeave.Accepted = true;
                    user.NormalLeavesCount -= (NormalLeave.EndDate - NormalLeave.StartDate).TotalDays;
                    NormalLeave.LeaveStatus = LeaveStatus.Accepted;
                    NormalLeave.Holder = Holder.NotWaiting;
                }
                else
                {
                    NormalLeave.DisapproveReasonOfGeneral_Manager = model.DisapproveReason;
                    NormalLeave.LeaveStatus = LeaveStatus.Rejected;
                    NormalLeave.RejectedBy = RejectedBy.GeneralManager;
                    NormalLeave.Holder = Holder.NotWaiting;
                }

                _base.Update(NormalLeave);

                return Ok(new
                {
                    message = "General manager decision updated successfully",
                    leaveID = NormalLeave.ID,
                    userID = NormalLeave.UserID,
                    accepted = NormalLeave.Accepted,
                    disapproveReason = NormalLeave.DisapproveReasonOfGeneral_Manager
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating", error = ex.Message });
            }     
        }
        [Authorize]
        [HttpPut(("UpdateDirectManagerDecision/{leaveID:int}"))]
        public IActionResult UpdateDirectManagerDecision(int leaveID, [FromBody] DirectManagerDecisionDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var NormalLeave = _base.Get(n =>
                    n.ID == leaveID &&
                    n.ResponseDone == false &&
                    n.CoWorker_Decision == true);

                if (NormalLeave == null)
                {
                    return NotFound(new { message = "Normal Leave not found or not eligible for update" });
                }

                // Update properties
                NormalLeave.DirectManager_Decision = model.DirectManagerDecision;
                if (model.DirectManagerDecision == false)
                {
                    NormalLeave.DisapproveReasonOfDirect_Manager = model.DisapproveReason;
                    NormalLeave.ResponseDone = true;
                    NormalLeave.LeaveStatus = LeaveStatus.Rejected;
                    NormalLeave.Holder = Holder.NotWaiting;
                    NormalLeave.RejectedBy = RejectedBy.DirectManager;
                }
                else
                {
                    NormalLeave.ResponseDone = false;
                    NormalLeave.Holder = Holder.GeneralManager;
                }

                _base.Update(NormalLeave);

                return Ok(new
                {
                    message = "Direct Manager decision updated successfully.",
                    leaveID = NormalLeave.ID,
                    userID = NormalLeave.UserID,
                    directManagerDecision = NormalLeave.DirectManager_Decision,
                    responseDone = NormalLeave.ResponseDone
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating", error = ex.Message });
            }    
        }
        [Authorize]
        [HttpPut("UpdateCoworkerDecision/{leaveID:int}")]
        public async Task<IActionResult> UpdateCoworkerDecision([FromRoute]int leaveID, [FromQuery]bool CoworkerDecision)
        {
            try
            {
                var NormalLeave = _base.Get(n =>
                    n.ID == leaveID &&
                    n.ResponseDone == false);

                if (NormalLeave == null)
                {
                    return NotFound(new { message = "Normal Leave not found or not eligible for update" });
                }

                // Update properties
                NormalLeave.CoWorker_Decision = CoworkerDecision;
                if (CoworkerDecision == false)
                {
                    NormalLeave.ResponseDone = true;
                    NormalLeave.LeaveStatus = LeaveStatus.Rejected;
                    NormalLeave.Holder = Holder.NotWaiting;
                    NormalLeave.RejectedBy = RejectedBy.CoWorker;
                }
                else
                {
                    NormalLeave.Holder = Holder.DirectManager;


                    // if Head of Departement made a leave request
                    // if أمين الكلية made a leave request
                    var user = _accountService.FindById(NormalLeave.UserID);
                    var dept = _departmentBase.Get(d => d.ManagerNationalNumber == user.NationalID);
                    bool cheackRole = await _accountService.IsInRoleAsync(user, "أمين الكلية");
                    if (cheackRole || dept != null)
                        NormalLeave.DirectManager_Decision = true;
                }


                _base.Update(NormalLeave);  

                return Ok(new
                {
                    message = "Coworker decision updated successfully.",
                    leaveID = NormalLeave.ID,
                    userID = NormalLeave.UserID,
                    coworkerDecision = NormalLeave.CoWorker_Decision,
                    responseDone = NormalLeave.ResponseDone
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpDelete("DeleteNormalLeave/{leaveID}")]
        public IActionResult DeleteNormalLeave([FromRoute]int leaveID)
        {
            if (leaveID <= 0)
            {
                return BadRequest("Wrong Leave ID.");
            }
            try
            {
                var NormalLeave = _base.Get(n => n.ID == leaveID);
                if (NormalLeave == null)
                {
                    return NotFound(new { message = "Normal Leave not found" });
                }
                _base.Remove(NormalLeave);

                return Ok(new
                {
                    message = "Normal Leave deleted successfully",
                    leaveID = NormalLeave.ID,
                    userID = NormalLeave.UserID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting", error = ex.Message });
            }
        }
    }
}
