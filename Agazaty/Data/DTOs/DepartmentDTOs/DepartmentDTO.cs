namespace Agazaty.Data.DTOs.DepartmentDTOs
{
    public class DepartmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime CreateDate { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
    }
}
