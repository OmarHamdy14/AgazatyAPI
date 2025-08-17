namespace Agazaty.Data.Services.Interfaces
{
    public interface ILeaveValidationService
    {
        Task<bool> IsLeaveOverlapping(string userId, DateTime startDate, DateTime endDate, string leaveType);
        Task<bool> IsSameLeaveOverlapping(string userId, DateTime startDate, DateTime endDate, string leaveType);
        Task<int> CalculateLeaveDays(DateTime startDate, DateTime endDate);
    }
}