namespace Agazaty.Data.DTOs.SickLeaveDTOs
{
    public class UpdateSickLeaveDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Chronic { get; set; }
        public bool Certified { get; set; }
    }
}