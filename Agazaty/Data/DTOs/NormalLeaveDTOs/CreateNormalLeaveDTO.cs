namespace Agazaty.Data.DTOs.NormalLeaveDTOs
{
    public class CreateNormalLeaveDTO
    {
        public DateTime RequestDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? NotesFromEmployee { get; set; }
        public int Year { get; set; }
        public string UserID { get; set; }
        public string Coworker_ID { get; set; }
    }
}
