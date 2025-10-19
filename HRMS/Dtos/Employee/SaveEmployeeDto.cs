namespace HRMS.Dtos.Employee
{
    public class SaveEmployeeDto
    {
        public long? Id { get; set; }
        public string FirstName { get; set; }
        public string LastNmae { get; set; }
        public string? Email { get; set; }
        public string Position { get; set; }
        public DateTime? BirthDate { get; set; }

    }
}
