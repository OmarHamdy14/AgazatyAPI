using Agazaty.Data.Base;
using Agazaty.Data.DTOs.CasualLeaveDTOs;
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
        //[Authorize]
        [HttpGet("GetNormalLeaveById/{leaveID:int}")]
        public async Task<IActionResult> GetNormalLeaveById(int leaveID)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });
            try
            {

                var NormalLeave = await _base.Get(n => n.ID == leaveID);
                if (NormalLeave == null)
                {
                    return NotFound(new { message = "No normal leave found." });
                }

                var leave = _mapper.Map<NormalLeaveDTO>(NormalLeave);
                var user = await _accountService.FindById(NormalLeave.UserID);
                var coworker = await _accountService.FindById(NormalLeave.Coworker_ID);
                var generalManager = await _accountService.FindById(NormalLeave.General_ManagerID); ;
                var directManager = await _accountService.FindById(NormalLeave.Direct_ManagerID); 
                leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";
                return Ok(leave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("GetAllAcceptedNormalLeaves")]
        public async Task<IActionResult> GetAllAcceptedNormalLeaves()
        {
            try
            {

                var NormalLeaves = await _base.GetAll(n => n.Accepted == true && n.ResponseDone == true);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = "No accepted normal leaves found." });
                }
                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("GetAllRejectedNormalLeaves")]
        public async Task<IActionResult> GetAllRejectedNormalLeaves()
        {
            try
            {
                var NormalLeaves = await _base.GetAll(n => n.Accepted == false && n.ResponseDone == true);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = "No rejected normal leaves found." });
                }
                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("AllNormalLeavesByUserId/{userID}")]
        public async Task<IActionResult> GetAllNormalLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {

                var NormalLeaves = await _base.GetAll(n => n.UserID == userID);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = "No normal leaves found." });
                }
                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("AcceptedByUserId/{userID}")]
        public async Task<IActionResult> GetAllAcceptedNormalLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {

                var NormalLeaves = await _base.GetAll(n => n.UserID == userID &&
                    n.Accepted == true &&
                    n.ResponseDone == true);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = "No accepted normal leaves found." });
                }
                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("AcceptedByUserIdAndYear/{userID}/{year:int}")]
        public async Task<IActionResult> GetAllAcceptedNormalLeavesByUserIDAndYear(string userID, int year)
        {
            try
            {
                var errors = new List<string>();
                int currentYear = DateTime.Now.Year;
                if (string.IsNullOrWhiteSpace(userID))
                    errors.Add("Invalid user ID.");
                if (year < 1900)
                    errors.Add("Invalid year.");
                else if (year > currentYear)
                    errors.Add($"Year cannot be older than the current year ({currentYear}).");
                if (errors.Any())
                    return BadRequest(new { messages = errors });

                var NormalLeaves = await _base.GetAll(n =>
                    n.UserID == userID &&
                    n.Year == year &&
                    n.Accepted == true &&
                    n.ResponseDone == true);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = $"No accepted normal leaves found in {year}." });
                }
                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("RejectedByUserId/{userID}")]
        public async Task<IActionResult> GetAllRejectedNormalLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {

                var NormalLeaves = await _base.GetAll(n =>
                    n.UserID == userID &&
                    n.Accepted == false &&
                    n.ResponseDone == true);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = "No rejected normal leaves found." });
                }
                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("WaitingByUserID/{userID}")]
        public async Task<IActionResult> GetAllWaitingNormalLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {

                var NormalLeaves = await _base.GetAll(n =>
                    n.UserID == userID &&
                    n.ResponseDone == false);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = "No waiting normal leaves found." });
                }

                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "عميد الكلية,أمين الكلية")]
        [HttpGet("WaitingByGeneral_ManagerID/{general_managerID}")]
        public async Task<IActionResult> GetAllWaitingNormalLeavesByGeneral_ManagerID(string general_managerID)
        {
            if (string.IsNullOrWhiteSpace(general_managerID))
                return BadRequest(new { message = "Invalid general manager ID." });
            try
            {

                var NormalLeaves = await _base.GetAll(n =>
                    n.General_ManagerID == general_managerID &&
                    n.GeneralManager_Decision==false &&
                    n.DirectManager_Decision == true &&
                    n.CoWorker_Decision == true &&
                    n.ResponseDone == false);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = "No waiting normal leaves found." });
                }

                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
                //return Ok(_mapper.Map<IEnumerable<NormalLeaveDTO>>(NormalLeaves));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("WaitingByDirect_ManagerID/{direct_managerID}")]
        public async Task<IActionResult> GetAllWaitingNormalLeavesByDirect_ManagerID(string direct_managerID)
        {
            if (string.IsNullOrWhiteSpace(direct_managerID))
                return BadRequest(new { message = "Invalid direct manager ID." });
            try
            {

                var NormalLeaves = await _base.GetAll(n =>
                    n.Direct_ManagerID == direct_managerID &&
                    n.DirectManager_Decision==false &&
                    n.CoWorker_Decision == true &&
                    n.ResponseDone == false);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = "No waiting normal leaves found." });
                }

                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("WaitingByCoWorkerID/{coworkerID}")]
        public async Task<IActionResult> GetAllWaitingNormalLeavesByCoWorkerID(string coworkerID)
        {
            if (string.IsNullOrWhiteSpace(coworkerID))
                return BadRequest(new { message = "Invalid coworker ID." });
            try
            {

                var NormalLeaves = await _base.GetAll(n =>
                    n.Coworker_ID == coworkerID &&
                    n.ResponseDone == false && n.CoWorker_Decision==false);
                if (!NormalLeaves.Any())
                {
                    return NotFound(new { message = "No waiting normal leaves found." });
                }

                var leaves = new List<NormalLeaveDTO>();
                foreach (var normalleave in NormalLeaves)
                {
                    var leave = _mapper.Map<NormalLeaveDTO>(normalleave);
                    var user = await _accountService.FindById(normalleave.UserID);
                    var coworker = await _accountService.FindById(normalleave.Coworker_ID);
                    var generalManager = await _accountService.FindById(normalleave.General_ManagerID); ;
                    var directManager = await _accountService.FindById(normalleave.Direct_ManagerID);
                    leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                    leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                    leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                    leaves.Add(leave);
                }
                return Ok(leaves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية,هيئة تدريس,موظفين")]
        [HttpGet("GetLeaveTypes")]
        public async Task<IActionResult> GetLeaveTypes()
        {
            return Ok(LeaveTypes.res);
        }
        //[Authorize]
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
                var cowrker = await _accountService.FindById(model.Coworker_ID); 
                var user = await _accountService.FindById(model.UserID);
                if(model.Coworker_ID==model.UserID || user ==null || cowrker == null)
                {
                    return BadRequest(new { Message = "Invalid user id or coworker id." });
                }
                if (((model.EndDate - model.StartDate).TotalDays + 1) > user.CasualLeavesCount)
                {
                    return Ok(new { Message = $"You request {((model.EndDate - model.StartDate).TotalDays + 1)} days, but the number of days available to you are {user.NormalLeavesCount}" });
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
                normalLeave.Year = model.RequestDate.Year;
                normalLeave.LeaveStatus = LeaveStatus.Waiting;
                normalLeave.Holder = Holder.CoWorker;
                normalLeave.RejectedBy = RejectedBy.NotRejected;
                if (await _accountService.IsInRoleAsync(user, "هيئة تدريس"))
                {
                    var res = await _accountService.GetAllUsersInRole("عميد الكلية");
                    var Dean = res.FirstOrDefault();
                    if(Dean == null) { return BadRequest( new { Message = "There no user with the Dean role" });  }
                    normalLeave.General_ManagerID = Dean.Id;

                    var DepartmentofUser = await _departmentBase.Get(dm => dm.Id == user.Departement_ID);
                    if(DepartmentofUser==null) { return BadRequest(new { Message = "This user doesn't have a department." });  }
                    normalLeave.Direct_ManagerID = DepartmentofUser.ManagerId;
                }
                else if(await _accountService.IsInRoleAsync(user, "موظف"))
                {
                    var res = await _accountService.GetAllUsersInRole("أمين الكلية");
                    var Supervisor = res.FirstOrDefault();
                    if (Supervisor == null) { return BadRequest(new { Message = "There no user with the Supervisor role" }); }
                    normalLeave.General_ManagerID = Supervisor.Id;

                    var DepartmentofUser = await _departmentBase.Get(dm => dm.Id == user.Departement_ID);
                    if (DepartmentofUser == null) { return BadRequest(new { Message = "This user doesn't have a department." }); }
                    normalLeave.Direct_ManagerID = DepartmentofUser.ManagerId;
                }
                else if(await _accountService.IsInRoleAsync(user, "أمين الكلية"))
                {
                    // if أمين الكلية made a leave request
                    var res = await _accountService.GetAllUsersInRole("عميد الكلية");
                    var Dean = res.FirstOrDefault();
                    if (Dean == null) { return BadRequest(new { Message = "There no user with the Dean role" }); }
                    normalLeave.General_ManagerID = Dean.Id;
                    normalLeave.Direct_ManagerID = Dean.Id;
                }
                else if (await _accountService.IsInRoleAsync(user, "مدير الموارد البشرية"))
                {
                    var res = await _accountService.GetAllUsersInRole("أمين الكلية");
                    var Supervisor = res.FirstOrDefault();
                    if (Supervisor == null) { return BadRequest(new { Message = "There no user with the Supervisor role" }); }
                    normalLeave.General_ManagerID = Supervisor.Id;
                    normalLeave.Direct_ManagerID = Supervisor.Id;
                }
                await _base.Add(normalLeave);


                var leave = _mapper.Map<NormalLeaveDTO>(normalLeave);
                var coworker = await _accountService.FindById(normalLeave.Coworker_ID);
                var generalManager = await _accountService.FindById(normalLeave.General_ManagerID); ;
                var directManager = await _accountService.FindById(normalLeave.Direct_ManagerID);
                leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";
                return CreatedAtAction(nameof(GetNormalLeaveById), new { leaveID = normalLeave.ID }, leave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateNormalLeave/{leaveID:int}")]
        public async Task<IActionResult> UpdateNormalLeave(int leaveID, [FromBody] UpdateNormalLeaveDTO model)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });

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

                var NormalLeave = await _base.Get(n =>
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
                var user = await _accountService.FindById(NormalLeave.UserID);

                NormalLeave.NotesFromEmployee = model.NotesFromEmployee;
                user.NormalLeavesCount += ((NormalLeave.EndDate - model.EndDate).TotalDays + 1);
                NormalLeave.EndDate = model.EndDate;

                await _base.Update(NormalLeave);

                var leave = _mapper.Map<NormalLeaveDTO>(NormalLeave);
                var coworker = await _accountService.FindById(NormalLeave.Coworker_ID);
                var generalManager = await _accountService.FindById(NormalLeave.General_ManagerID); ;
                var directManager = await _accountService.FindById(NormalLeave.Direct_ManagerID);
                leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                return Ok(new
                {
                    message = "Normal Leave updated successfully",
                    Leave = leave
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating", error = ex.Message });
            }

        }
        //[Authorize(Roles = "عميد الكلية,أمين الكلية")]
        [HttpPut("UpdateGeneralManagerDecision/{leaveID:int}")]
        public async Task<IActionResult> UpdateGeneralManagerDecision(int leaveID, [FromBody] GeneralManagerDecisionDTO model)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });

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
                var NormalLeave = await _base.Get(n =>
                    n.ID == leaveID &&
                    n.CoWorker_Decision == true &&
                    n.DirectManager_Decision == true &&
                    n.Accepted == false &&
                    n.ResponseDone == false
                    );

                if (NormalLeave == null)
                {
                    return NotFound(new { message = "Normal Leave not found or not eligible for update" });
                }
                var user = await _accountService.FindById(NormalLeave.UserID);
                // Update properties
                NormalLeave.GeneralManager_Decision = model.GeneralManagerDecision;
                NormalLeave.ResponseDone = true;
                if (model.GeneralManagerDecision == true)
                {
                    NormalLeave.Accepted = true;
                    user.NormalLeavesCount -= ((NormalLeave.EndDate - NormalLeave.StartDate).TotalDays + 1);
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

                await _base.Update(NormalLeave);

                var leave = _mapper.Map<NormalLeaveDTO>(NormalLeave);
                var coworker = await _accountService.FindById(NormalLeave.Coworker_ID);
                var generalManager = await _accountService.FindById(NormalLeave.General_ManagerID); ;
                var directManager = await _accountService.FindById(NormalLeave.Direct_ManagerID);
                leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                return Ok(new
                {
                    message = "General manager decision updated successfully",
                    Leave = leave
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating", error = ex.Message });
            }     
        }
        //[Authorize]
        [HttpPut(("UpdateDirectManagerDecision/{leaveID:int}"))]
        public async Task<IActionResult> UpdateDirectManagerDecision(int leaveID, [FromBody] DirectManagerDecisionDTO model)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var NormalLeave = await _base.Get(n =>
                    n.ID == leaveID &&
                    n.ResponseDone == false &&
                    n.Accepted == false &&
                    n.DirectManager_Decision == false &&
                    n.GeneralManager_Decision == false &&
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

                await _base.Update(NormalLeave);

                var leave = _mapper.Map<NormalLeaveDTO>(NormalLeave);
                var user = await _accountService.FindById(NormalLeave.UserID);
                var coworker = await _accountService.FindById(NormalLeave.Coworker_ID);
                var generalManager = await _accountService.FindById(NormalLeave.General_ManagerID); ;
                var directManager = await _accountService.FindById(NormalLeave.Direct_ManagerID);
                leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                return Ok(new
                {
                    message = "Direct Manager decision updated successfully.",
                    Leave = leave
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating", error = ex.Message });
            }    
        }
        //[Authorize]
        [HttpPut("UpdateCoworkerDecision/{leaveID:int}")]
        public async Task<IActionResult> UpdateCoworkerDecision([FromRoute]int leaveID, [FromQuery]bool CoworkerDecision)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });

            try
            {
                var NormalLeave = await _base.Get(n =>
                    n.ID == leaveID &&
                    n.ResponseDone == false &&
                    n.DirectManager_Decision == false &&
                    n.GeneralManager_Decision == false &&
                    n.Accepted == false);

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
                    var userr = await _accountService.FindById(NormalLeave.UserID);
                    var IsdeptManager = await _departmentBase.Get(d => d.ManagerId == userr.Id);
                    bool cheackRole = await _accountService.IsInRoleAsync(userr, "أمين الكلية");
                    if (cheackRole || IsdeptManager != null)
                        NormalLeave.DirectManager_Decision = true;
                }


                await _base.Update(NormalLeave);

                var leave = _mapper.Map<NormalLeaveDTO>(NormalLeave);
                var user = await _accountService.FindById(NormalLeave.UserID);
                var coworker = await _accountService.FindById(NormalLeave.Coworker_ID);
                var generalManager = await _accountService.FindById(NormalLeave.General_ManagerID); ;
                var directManager = await _accountService.FindById(NormalLeave.Direct_ManagerID);
                leave.GeneralManagerName = $"{generalManager.FirstName} {generalManager.SecondName} {generalManager.ThirdName} {generalManager.ForthName}";
                leave.DirectManagerName = $"{directManager.FirstName} {directManager.SecondName} {directManager.ThirdName} {directManager.ForthName}";
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                leave.CoworkerName = $"{coworker.FirstName} {coworker.SecondName} {coworker.ThirdName} {coworker.ForthName}";

                return Ok(new
                {
                    message = "Coworker decision updated successfully.",
                    Leave = leave
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating", error = ex.Message });
            }
         }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpDelete("DeleteNormalLeave/{leaveID}")]
        public async Task<IActionResult> DeleteNormalLeave([FromRoute]int leaveID)
        {
            if (leaveID <= 0)
            {
                return BadRequest("Wrong Leave ID.");
            }
            try
            {
                var NormalLeave = await _base.Get(n => n.ID == leaveID);
                if (NormalLeave == null)
                {
                    return NotFound(new { message = "Normal Leave not found" });
                }
                await _base.Remove(NormalLeave);

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
