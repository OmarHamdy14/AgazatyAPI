namespace Agazaty.Data.DTOs.SickLeaveDTOs
{
    public class CreateSickLeaveDTO
    {
        public string Disease { get; set; }
        public string EmployeeAddress { get; set; }
        public DateTime RequestDate { get; set; }
        public int Year { get; set; }
        public string UserID { get; set; }
    }
}
