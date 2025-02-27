using Agazaty.Data.Base;
using Agazaty.Data.DTOs.PermitLeavesDTOs;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public PermitLeaveController(IMapper mapper, IEntityBaseRepository<PermitLeave> Ebase, IEntityBaseRepository<PermitLeaveImage> PermitImagebase, IWebHostEnvironment webHostEnvironment)
        {
            _mapper = mapper;
            _Permitbase = Ebase;
            _PermitImagebase = PermitImagebase;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetPermitLeaveById/{leaveID:int}", Name = "GetPermitLeave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PermitLeave> GetPermitLeaveById(int leaveID)
        {
            if (leaveID <= 0)
                return BadRequest(new { message = "Invalid leave ID." });
            try
            {
                var permitLeave = _Permitbase.Get(c => c.Id == leaveID);
                if (permitLeave == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<PermitLeaveDTO>(permitLeave));
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
        public ActionResult<IEnumerable<PermitLeave>> GetAllPermitLeaves()
        {
            try
            {
                var permitLeaves = _Permitbase.GetAll().ToList();
                if (permitLeaves == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<PermitLeave>>(permitLeaves));
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
        public ActionResult<IEnumerable<PermitLeave>> GetAllPermitLeavesByUserID(string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "Invalid user ID." });
            try
            {
                var permitLeaves = _Permitbase.GetAll(p => p.UserId == userID).ToList();
                if (permitLeaves == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<PermitLeave>>(permitLeaves));
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
        public ActionResult<IEnumerable<PermitLeave>> GetAllPermitLeavesByUserIDAndMonth(string userID, int month)
        {
            if (string.IsNullOrWhiteSpace(userID) || month<=0)
                return BadRequest(new { message = "Invalid user ID or month." });
            try
            {
                var permitLeaves = _Permitbase.GetAll(p => p.UserId == userID &&
                                   p.Date.Month == month).ToList();
                if (permitLeaves == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<PermitLeave>>(permitLeaves));
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
        public ActionResult<IEnumerable<PermitLeave>> GetAllPermitLeavesByUserIDAndYear(string userID, int year)
        {
            if (string.IsNullOrWhiteSpace(userID) || year < 1900)
                return BadRequest(new { message = "Invalid user IDor year." });
            try
            {
                var permitLeaves = _Permitbase.GetAll(p => p.UserId == userID &&
                                   p.Date.Year == year).ToList();
                if (permitLeaves == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<PermitLeave>>(permitLeaves));
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
        public ActionResult<PermitLeave> CreatePermitLeave([FromForm] CreatePermitLeaveDTO model, [FromForm] List<IFormFile>? files)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                

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
                _Permitbase.Add(permitLeave);

                return Ok(new { Message =  "Creation is succeeded"});
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
        public IActionResult UpdatePermitLeave(int leaveID, [FromForm] UpdatePermitLeaveDTO model, [FromForm] List<IFormFile>? files)
        {
            if (leaveID<=0)
            {
                return BadRequest();
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var permitLeave = _Permitbase.Get(c => c.Id == leaveID);
                if (permitLeave == null)
                {
                    return NotFound();
                }
                //permitLeave = _mapper.Map<PermitLeave>(model);
                permitLeave.Hours = model.Hours;
                permitLeave.EmployeeNationalNumber = model.EmployeeNationalNumber;
                permitLeave.UserId = model.UserId;
                permitLeave.Date = model.Date;
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
                _Permitbase.Update(permitLeave);
                return NoContent();
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
        public IActionResult DeletePermitLeave(int leaveID)
        {
            if (leaveID<=0)
            {
                return BadRequest();
            }
            try
            {
                var permitLeave = _Permitbase.Get(c => c.Id == leaveID);
                if (permitLeave == null)
                {
                    return NotFound();
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
                _Permitbase.Remove(permitLeave);

                return NoContent();
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
        public IActionResult DeleteImage(int imageId)
        {
            if (imageId <= 0)
                return BadRequest();
            try
            {
                var imageToBeDeleted = _PermitImagebase.Get(pi => pi.Id == imageId);
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
                    _PermitImagebase.Remove(imageToBeDeleted);

                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}
