using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Dapper;
using System.Linq;
using Agazaty.Data.Base;



namespace Agazaty.Data.Services
{
    public class LeaveValidationService : ILeaveValidationService
    {
        private readonly AppDbContext _context;
        private readonly IDbConnection _dbConnection;
        private readonly IEntityBaseRepository<Holiday> _baseHoliday;


        public LeaveValidationService(AppDbContext context, IDbConnection dbConnection, IEntityBaseRepository<Holiday> baseHoliday)
        {
            _context = context;
            _dbConnection = dbConnection;
            _baseHoliday = baseHoliday;
        }

        public async Task<bool> IsLeaveOverlapping(string userId, DateTime startDate, DateTime endDate, string leaveType)
        {
            //((l.StartDate <= endDate && l.EndDate >= startDate) ||
            //(l.EndDate >= startDate && l.StartDate <= startDate))))
            string sql = @"
             SELECT TOP 1 OverlapExists FROM (
                SELECT 1 AS OverlapExists FROM SickLeaves 
                WHERE @LeaveType != 'SickLeave' AND UserID = @userId 
                AND ((StartDate <= @endDate AND StartDate >= @startDate) OR (EndDate >= @endDate AND StartDate <= @startDate) OR (@startDate<=EndDate AND EndDate<=@endDate))
                UNION ALL

                SELECT 1 AS OverlapExists FROM NormalLeaves
                WHERE @LeaveType != 'NormalLeave' AND UserID = @userId
                AND ((StartDate <= @endDate AND StartDate >= @startDate) OR (EndDate >= @endDate AND StartDate <= @startDate) OR (@startDate<=EndDate AND EndDate<=@endDate))
                AND (Accepted = 1)

                UNION ALL

                SELECT 1 AS OverlapExists FROM CasualLeaves 
                WHERE @LeaveType != 'CasualLeave' AND UserID = @userId 
                AND ((StartDate <= @endDate AND StartDate >= @startDate) OR (EndDate >= @endDate AND StartDate <= @startDate) OR (@startDate<=EndDate AND EndDate<=@endDate))
            ) AS OverlappingLeaves";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<int?>(sql, new
            {
                userId = userId,
                startDate = startDate,
                endDate = endDate,
                LeaveType = leaveType
            });

            return result.HasValue; // Returns true if an overlapping leave exists
        }
        public async Task<bool> IsSameLeaveOverlapping(string userId, DateTime startDate, DateTime endDate, string leaveType)
        {
            if (leaveType == "SickLeave")
            {
                string sql = @" SELECT 1 FROM SickLeaves 
                WHERE @LeaveType = 'SickLeave' AND UserID = @userId 
                AND ((StartDate <= @endDate AND StartDate >= @startDate) OR (EndDate >= @endDate AND StartDate <= @startDate) OR (@startDate<=EndDate AND EndDate<=@endDate))";
                var result = await _dbConnection.QueryFirstOrDefaultAsync<int?>(sql, new
                {
                    userId = userId,
                    startDate = startDate,
                    endDate = endDate,
                    LeaveType = leaveType
                });
                return result.HasValue;
                //(leaveType == "SickLeave" && await _context.SickLeaves.AnyAsync(l =>
                //     l.UserID == userId &&
                //     ((l.StartDate <= endDate && l.EndDate >= startDate) || (l.EndDate >= startDate && l.StartDate <= startDate))));
            }
            if (leaveType == "CasualLeave")
            {
                string sql = @" SELECT 1 FROM CasualLeaves
                WHERE @LeaveType = 'CasualLeave' AND UserID = @userId 
                AND ((StartDate <= @endDate AND StartDate >= @startDate) OR (EndDate >= @endDate AND StartDate <= @startDate) OR (@startDate<=EndDate AND EndDate<=@endDate))";
                var result = await _dbConnection.QueryFirstOrDefaultAsync<int?>(sql, new
                {
                    userId = userId,
                    startDate = startDate,
                    endDate = endDate,
                    LeaveType = leaveType
                });
                return result.HasValue;
            }
            if (leaveType == "NormalLeave")
            {
                string sql = @" SELECT 1 FROM NormalLeaves
                WHERE @LeaveType = 'NormalLeave' AND UserID = @userId AND (Accepted = 1)
                AND ((StartDate <= @endDate AND StartDate >= @startDate) OR (EndDate >= @endDate AND StartDate <= @startDate) OR (@startDate<=EndDate AND EndDate<=@endDate))";
                var result = await _dbConnection.QueryFirstOrDefaultAsync<int?>(sql, new
                {
                    userId = userId,
                    startDate = startDate,
                    endDate = endDate,
                    LeaveType = leaveType
                });
                return result.HasValue;
            }

            return false;

        }
        // ✅ 3. ميثود مستقلة لحساب عدد أيام الإجازة بعد استبعاد الجمعة، السبت، والإجازات الرسمية
        public async Task<int> CalculateLeaveDays(DateTime startDate, DateTime endDate)
        {
            // التأكد من أن تاريخ البداية ليس بعد تاريخ النهاية
            if (startDate > endDate)
            {
                throw new ArgumentException("تاريخ البداية يجب أن يكون قبل تاريخ النهاية.");
            }
            int leaveDays = 0;
            //var holidays = await _baseHoliday.GetAll(h => h.Date.Year == DateTime.UtcNow.Year);
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var holiday = await _baseHoliday.Get(h => h.Date.Date == date.Date);
                if (holiday == null)
                    leaveDays++;
                // استبعاد الجمعة والسبت
                //if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                //{
                //    continue;
                //}
                //leaveDays++;
            }
            return leaveDays;
            //return  Enumerable.Range(0, (endDate - startDate).Days + 1)
            //                 .Select(i => startDate.AddDays(i))
            //                 .Count(d => d.DayOfWeek != DayOfWeek.Friday  // استبعاد الجمعة
            //                          && d.DayOfWeek != DayOfWeek.Saturday); // استبعاد السبت
        }
    }
}