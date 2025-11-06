
using HRMS.DbContexts;
using HRMS.DTO.Employee;
using HRMS.Dtos.Employee;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace HRMS.Controllers
{
    [Route("api/[controller]")] // data Annotation
    [ApiController]// data Annotation 
    public class EmployeesController : ControllerBase
    {
        public static List<Employee> employees = new List<Employee>()
        {

             new Employee(){ Id=1,FirstName ="QUSAI",LastName="Mohammad", Email="Qusai@123.com",Position="Developer", BirthDate=new DateTime(2005,10,20)},
            new Employee() { Id=2,FirstName ="Rand ",LastName="Abbad", Position="HR", BirthDate=new DateTime(2003,11,5)},
             new Employee(){ Id=3,FirstName ="Mohammad",LastName="Ezate", Position="Developer", BirthDate=new DateTime(2002,6,7)},

        };
        private readonly HRMSContexts _dbcontext;
        public EmployeesController(HRMSContexts dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet("GetByCriteria")]//عشان تحول الميثود ل API ENDPOINT
        public IActionResult GetByCriteria([FromQuery] SearchEmployeeDto employeeDto) //Method--> api Endpoint 
        {
            var result = from employee in _dbcontext.Employees
                         from department in _dbcontext.Departments.Where(x => x.Id == employee.DepartmentId).DefaultIfEmpty()//==left join
                         from manager in _dbcontext.Employees.Where(x => x.Id == employee.ManagerId).DefaultIfEmpty()//left join
                         where (employeeDto.position == null || employee.Position.ToUpper().Contains(employeeDto.Posisiton.ToUpper())) &&
                         (employeeDto.Name == null || employee.FirstName.ToUpper().Contains(employeeDto.Name.ToUpper()))
                         orderby employee.Id descending
                         select new EmployeeDto
                         {
                             Id = employee.Id,
                             Name = employee.FirstName + " " + employee.LastName,
                             Position = employee.Position,
                              BirthDate = employee.BirthDate,
                             Email = employee.Email,
                             Salary = employee.Salary,
                             DepartmentId = employee.DepartmentId,
                             DepartmentName = department.Name,
                             ManagerId = employee.ManagerId,

                             ManagerName = manager.FirstName
                         };

            return Ok(result);
            //return Ok("Name ; Qusai , Age :20");//200
            // return NotFound("Data Not Found");//404
            //return BadRequest("Data Missing");//500
            //return Ok(new { Name = "Qusai", Age = 20 });


        }

        [HttpGet("GetById/{id}")]// Route parameter
        public IActionResult GetById(long id)
        {
            if (id == 0)
                return BadRequest("Id Value Is Invalid");
            var result = _dbcontext.Employees.Select(x => new EmployeeDto
            {
                Id = x.Id,
                Name = x.FirstName + " " + x.LastName,
                Position = x.Position,
                BirthDate = x.BirthDate,
                Email = x.Email,
                Salary = x.Salary,
                DepartmentId = x.DepartmentId,
                DepartmentName = "",

                ManagerId = x.ManagerId,
                ManagerName = ""

            }).FirstOrDefault(x => x.Id == id);
            if (result == null)
                return NotFound("Employee Not Found");
            return Ok(result);


        }
        [HttpPost("Add")]
        public IActionResult Add(long id, [FromBody] SaveEmployeeDto employeeDto)
        {
            var employee = new Employee()
            {
                //Id = (employees.LastOrDefault()?.Id ?? 0) + 1,  //عملنا هاي الحركه عشان الداتا
                Id = 0,                                                //اذا كانت فاضيه حيعمل ايرور فا
                                                                       //؟؟ بتعمل الشرط الي هوه زيرو

                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastNmae,
                Email = employeeDto.Email,
                BirthDate = employeeDto.BirthDate,
                Position = employeeDto.Position,
                Salary = employeeDto.Salary,
                DepartmentId = employeeDto.DepartmentId,


            };

            _dbcontext.Employees.Add(employee);
            _dbcontext.SaveChanges();//عشان تحفظ التعديلات في الداتا بيس

            return Ok();
        }


        [HttpPut("Update")]
        public IActionResult Update([FromBody] SaveEmployeeDto employeeDto)
        {

            var employee = _dbcontext.Employees.FirstOrDefault(x => x.Id == employeeDto.Id);
            if (employee == null)
                return NotFound("Employee Not Exist");

            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastNmae;
            employee.Email = employeeDto.Email;
            employee.BirthDate = employeeDto.BirthDate;
            employee.Position = employeeDto.Position;
            employee.Salary = employeeDto.Salary;
            employee.DepartmentId = employeeDto.DepartmentId;
            employee.ManagerId = employeeDto.ManagerId;

            _dbcontext.SaveChanges();
            return Ok();



        }
        [HttpDelete("Delete")]
        public IActionResult Delete([FromQuery] long id)
        {

            var employee = _dbcontext.Employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
                return NotFound("Employee Does Not Exist");
            _dbcontext.Employees.Remove(employee);

            _dbcontext.SaveChanges();
            return Ok();



        }

    }


}
