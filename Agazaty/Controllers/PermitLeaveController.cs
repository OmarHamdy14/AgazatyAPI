using Agazaty.Data.Base;
using Agazaty.Data.DTOs.DepartmentDTOs;
using Agazaty.Data.DTOs.PermitLeavesDTOs;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;

namespace Agazaty.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermitLeaveController : ControllerBase
    {
        private readonly IEntityBaseRepository<PermitLeave> _Permitbase;
        private readonly IEntityBaseRepository<PermitLeaveImage> _PermitImagebase;
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public PermitLeaveController(IAccountService accountService, IMapper mapper, IEntityBaseRepository<PermitLeave> Ebase, IEntityBaseRepository<PermitLeaveImage> PermitImagebase, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _Permitbase = Ebase;
            _PermitImagebase = PermitImagebase;
            _webHostEnvironment = webHostEnvironment;
            _accountService = accountService;
        }
        [Authorize]
        [HttpGet("GetPermitLeaveImageByleaveId/{leaveID:int}", Name = "GetPermitLeaveImageByleaveId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermitLeaveDTO>> GetPermitLeaveImageByleaveId(int leaveID)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });
            try
            {
                var permitLeaveImage = await _PermitImagebase.Get(c => c.LeaveId == leaveID);
                if (permitLeaveImage == null)
                {
                    return NotFound(new { Message = "No permit Leave image found." });
                }
                return Ok(_mapper.Map<PermitLeaveImageDTO>(permitLeaveImage));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("GetPermitLeaveById/{leaveID:int}", Name = "GetPermitLeave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermitLeaveDTO>> GetPermitLeaveById(int leaveID)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });
            try
            {
                var permitLeave = await _Permitbase.Get(c => c.Id == leaveID);
                if (permitLeave == null)
                {
                    return NotFound(new { Message = "No permit Leave found." });
                }
                var leave = _mapper.Map<PermitLeaveDTO>(permitLeave);
                var user = await _accountService.FindById(permitLeave.UserId);
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                return Ok(leave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetAllPermitLeaves", Name = "GetAllPermitLeaves")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PermitLeaveDTO>>> GetAllPermitLeaves()
        {
            try
            {
                var permitLeaves = await _Permitbase.GetAll();
                if (!permitLeaves.Any())
                {
                    return NotFound(new { Message = "Ino permit leaves found." });
                }

                var leaves = _mapper.Map<IEnumerable<PermitLeaveDTO>>(permitLeaves);
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
        [HttpGet("GetAllPermitLeavesByUserID/{userID}", Name = "GetAllPermitLeavesByUserID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PermitLeaveDTO>>> GetAllPermitLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                var permitLeaves = await _Permitbase.GetAll(p => p.UserId == userID);
                if (!permitLeaves.Any())
                {
                    return NotFound(new { Message = "no permit leaves found" });
                }

                var leaves = _mapper.Map<IEnumerable<PermitLeaveDTO>>(permitLeaves);
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
        [HttpGet("GetAllPermitLeavesByUserIDAndMonth/{userID}/{month:int}", Name = "GetAllPermitLeavesByUserIDAndMonth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PermitLeaveDTO>>> GetAllPermitLeavesByUserIDAndMonth(string userID, int month)
        {
            if (string.IsNullOrWhiteSpace(userID) || month < 1 || month > 12)
                return BadRequest(new { message = "Invalid user ID or month." });
            try
            {
                var permitLeaves = await _Permitbase.GetAll(p => p.UserId == userID &&
                                   p.Date.Month == month);
                if (!permitLeaves.Any())
                {
                    return NotFound(new { Message = "no permit leaves found." });
                }

                var leaves = _mapper.Map<IEnumerable<PermitLeaveDTO>>(permitLeaves);
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
        [HttpGet("GetAllPermitLeavesByUserIDAndYear/{userID}/{year:int}", Name = "GetAllPermitLeavesByUserIDAndYear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PermitLeaveDTO>>> GetAllPermitLeavesByUserIDAndYear(string userID, int year)
        {
            if (string.IsNullOrWhiteSpace(userID) || year < 1900)
                return BadRequest(new { message = "Invalid user ID or year." });
            try
            {
                var permitLeaves = await _Permitbase.GetAll(p => p.UserId == userID &&
                                   p.Date.Year == year);
                if (!permitLeaves.Any())
                {
                    return NotFound(new{ Message = "no permit leaves found."});
                }

                var leaves = _mapper.Map<IEnumerable<PermitLeaveDTO>>(permitLeaves);
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
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPost("CreatePermitLeave", Name = "CreatePermitLeave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PermitLeave>> CreatePermitLeave([FromForm] CreatePermitLeaveDTO model, [FromForm] List<IFormFile>? files)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                if (model.Hours <= 0) return BadRequest(new { Message = "Hours should be more than 0." });
                if(await _accountService.FindById(model.UserId) is null) return BadRequest(new { Message = "User is not found." });

                var permitLeave = _mapper.Map<PermitLeave>(model);

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string permitLeavePath = @"images\PermitLeaves\PermitLeaveUser-" + permitLeave.UserId;
                        string finalPath = Path.Combine(wwwRootPath, permitLeavePath);

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        PermitLeaveImage permitLeaveImage = new PermitLeaveImage()
                        {
                            ImageUrl = @"\" + permitLeavePath + @"\" + fileName,
                            LeaveId = permitLeave.Id
                        };


                        if (permitLeave.PermitLeaveImages == null)
                        {
                            permitLeave.PermitLeaveImages = new List<PermitLeaveImage>();
                        }
                        permitLeave.PermitLeaveImages.Add(permitLeaveImage);
                    }
                }
                await _Permitbase.Add(permitLeave);

                var leave = _mapper.Map<PermitLeaveDTO>(permitLeave);
                var user = await _accountService.FindById(permitLeave.UserId);
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                return CreatedAtAction(nameof(GetPermitLeaveById), new { leaveID = permitLeave.Id }, leave);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdatePermitLeave/{leaveID:int}", Name = "UpdatePermitLeave")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePermitLeave(int leaveID, [FromForm] UpdatePermitLeaveDTO model, [FromForm] List<IFormFile>? files) 
        {
            if (leaveID<=0)
            {
                return BadRequest(new { Message = "Invalid leave Id." });
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (model.Hours <= 0) return BadRequest(new { Message = "Hours should be more than 0." });
                if (await _accountService.FindById(model.UserId) is null) return BadRequest(new { Message = "User is not found." });


                var permitLeave = await _Permitbase.Get(c => c.Id == leaveID);
                if (permitLeave == null)
                {
                    return NotFound(new { Message = "no permit leaves found." });
                }

                _mapper.Map(model, permitLeave);

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string permitLeavePath = @"images\PermitLeaves\PermitLeaveUser-" + permitLeave.UserId;
                        string finalPath = Path.Combine(wwwRootPath, permitLeavePath);

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        PermitLeaveImage permitLeaveImage = new PermitLeaveImage()
                        {
                            ImageUrl = @"\" + permitLeavePath + @"\" + fileName,
                            LeaveId = permitLeave.Id
                        };


                        if (permitLeave.PermitLeaveImages == null)
                        {
                            permitLeave.PermitLeaveImages = new List<PermitLeaveImage>();
                        }
                        permitLeave.PermitLeaveImages.Add(permitLeaveImage);
                    }
                }
                await _Permitbase.Update(permitLeave);

                var leave = _mapper.Map<PermitLeaveDTO>(permitLeave);
                var user = await _accountService.FindById(permitLeave.UserId);
                leave.UserName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                return Ok(new {Message="Update is succeeded",PermitLeave = leave });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpDelete("DeletePermitLeave/{leaveID:int}", Name = "DeletePermitLeave")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePermitLeave(int leaveID)
        {
            if (leaveID<=0)
            {
                return BadRequest(new { Message = "Invalid leave Id" });
            }
            try
            {
                var permitLeave = await _Permitbase.Get(c => c.Id == leaveID);
                if (permitLeave == null)
                {
                    return NotFound(new {Message = "No Permit Leave found"});
                }
                string permitLeavePath = @"images\Permitleaves\PermitLeaveUser-" + permitLeave.UserId;
                string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, permitLeavePath);

                if (Directory.Exists(finalPath))
                {
                    string[] filePaths = Directory.GetFiles(finalPath);
                    foreach (string filePath in filePaths)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    Directory.Delete(finalPath);
                }
                await _Permitbase.Remove(permitLeave);

                return Ok(new { Message = "Deletion is succeeded." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpDelete("DeleteImage/{imageId:int}", Name = "DeleteImage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            if (imageId <= 0)
                return BadRequest(new { Message = "Invalid image Id" });
            try
            {
                var imageToBeDeleted = await _PermitImagebase.Get(pi => pi.Id == imageId);
                if (imageToBeDeleted != null)
                {
                    if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    await _PermitImagebase.Remove(imageToBeDeleted);

                    return Ok(new { Message = "Deletion is succeeded." });
                }
                return NotFound(new { Message = "no image found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}