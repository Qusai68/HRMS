using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    public class Employee
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string Position { get; set; }
        public DateTime? BirthDate { get; set; }
        public decimal Salary { get; set; }
        [ForeignKey("Department")]
        public long?  DepartmentId { get; set; }
         public Department? Department { get; set; }//navigation property
        [ForeignKey("Manager")]
        public long? ManagerId { get; set; }
     public Employee? Manager { get; set; }//navigation property
    }
}
