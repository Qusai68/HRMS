namespace HRMS.DTO.Employee
{
    public class EmployeeDto
    {
        public long? Id { get; set; }
        
        public string? Name { get; set; }
        public string? Position { get; set; }
        public int? Age { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Email { get; set; }
        public decimal? Salary { get; set; }
        public long? DepartmentId { get; set; }
        public long? ManagerId { get; set; }
        public string? DepartmentName { get; set; }
        public string? ManagerName { get; set ; }
    }
}
