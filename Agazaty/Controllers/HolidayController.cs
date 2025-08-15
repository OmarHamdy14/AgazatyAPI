using Agazaty.Data.Base;
using Agazaty.Data.DTOs.DepartmentDTOs;
using Agazaty.Data.DTOs.HolidayDTOs;
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
    public class HolidayController : ControllerBase
    {
        private readonly IEntityBaseRepository<Holiday> _base;
        private readonly IEntityBaseRepository<NormalLeave> _normalLeavebase;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public HolidayController(IMapper mapper, IEntityBaseRepository<Holiday> Ebase, IAccountService accountService, IEntityBaseRepository<NormalLeave> normalLeavebase)
        {
            _mapper = mapper;
            _base = Ebase;
            _accountService = accountService;
            _normalLeavebase = normalLeavebase;
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetAllHolidays")]
        public async Task<IActionResult> GetAllHolidays()
        {
            try
            {
                var holiday = await _base.Get(h => h.Date == Date);
                if (holiday == null) leaveDays++;
                var holidays = await _baseHoliday.GetAll(h => h.Date.Year == DateTime.UtcNow.Year);
                if (!holidays.Any())
                {
                    return NotFound("No holidays found.");
                }
                return Ok(holidays);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetHolidayById/{holidayID:int}")]
        public async Task<IActionResult> GetHolidayById(int holidayID)
        {
            if (holidayID <= 0)
            {
                return BadRequest(new { Message = "Invalid department Id" });
            }
            try
            {
                var holiday = await _base.Get(h=> h.Id == holidayID);
                if (holiday == null)
                {
                    return NotFound($"No department found.");
                }
                return Ok(holiday);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPost("CreateHoliday")]
        public async Task<IActionResult> CreateHoliday([FromBody] CreateHolidayDTO model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid holiday data.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var holiday = await _base.Get(h => h.Date.Date == model.Date);
                if (holiday != null)
                {
                    return NotFound(new { Message = "There is holiday already with this date." });
                }
                var role = await _accountService.GetFirstRole(user);
                var holiday = _mapper.Map<Holiday>(model);
                await _base.Add(holiday);

                var AllNormalLeaves = await _normalLeavebase.GetAll(l => l.StartDate.Year == holiday.Date.Year);
                foreach(var leave in AllNormalLeaves)
                {
                    if(leave.StartDate >= model.Date && leave.EndDate <= model.Date)
                    {
                        var user = await _accountService.FindById(leave.UserID);
                        user.NormalLeavesCount++;
                    }
                }
                return CreatedAtAction(nameof(GetHolidayById), new { holidayID = holiday.Id }, holiday);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateHoliday/{holidayID:int}")]
        public async Task<IActionResult> UpdateHoliday([FromRoute] int holidayID, [FromBody] UpdateHolidayDTO model)
        {
            if (holidayID <= 0)
            {
                return BadRequest("Invalid holiday data.");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var holiday = await _base.Get(d => d.Id == holidayID);
                if (holiday == null)
                {
                    return NotFound(new { Message = "Holiday is not found." });
                }
                else // old date wrong
                {
                    var AllNormalLeaves1 = await _normalLeavebase.GetAll(l => l.StartDate.Year == holiday.Date.Year);
                    foreach (var leave in AllNormalLeaves1)
                    {
                        if (leave.StartDate >= holiday.Date && leave.EndDate <= holiday.Date)
                        {
                            var user = await _accountService.FindById(leave.UserID);
                            user.NormalLeavesCount--;
                        }
                    }
                }
                _mapper.Map(model, holiday);
                await _base.Update(holiday);

                var AllNormalLeaves = await _normalLeavebase.GetAll(l => l.StartDate.Year == holiday.Date.Year);  // new date
                foreach (var leave in AllNormalLeaves)
                {
                    if (leave.StartDate >= holiday.Date && leave.EndDate <= holiday.Date)
                    {
                        var user = await _accountService.FindById(leave.UserID);
                        user.NormalLeavesCount++;
                    }
                }

                return Ok(new { Message = $"Holiday has been successfully updated.", Holiday = holiday });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        [Authorize(Roles = "مدير الموارد البشرية")]
        [HttpDelete("DeleteHoliday/{holidayID:int}")]
        public async Task<IActionResult> DeleteHoliday(int holidayID)
        {
            if (holidayID <= 0)
            {
                return BadRequest(new { Message = "Invalid holiday Id." });
            }

            try
            {
                var holiday = await _base.Get(d => d.Id == holidayID);

                if (holiday == null)
                {
                    return NotFound($"No holiday found.");
                }
                await _base.Remove(holiday);

                var AllNormalLeaves = await _normalLeavebase.GetAll(l => l.StartDate.Year == holiday.Date.Year);
                foreach (var leave in AllNormalLeaves)
                {
                    if (leave.StartDate >= holiday.Date && leave.EndDate <= holiday.Date)
                    {
                        var user = await _accountService.FindById(leave.UserID);
                        user.NormalLeavesCount--;
                    }
                }
                return Ok($"Holiday has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}