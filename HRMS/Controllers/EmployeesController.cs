
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

             new Employee(){ Id=1,FirstName ="QUSAI",LastNmae="Mohammad", Email="Qusai@123.com",Position="Developer", BirthDate=new DateTime(2005,10,20)},
            new Employee() { Id=2,FirstName ="Rand ",LastNmae="Abbad", Position="HR", BirthDate=new DateTime(2003,11,5)},
             new Employee(){ Id=3,FirstName ="Mohammad",LastNmae="Ezate", Position="Developer", BirthDate=new DateTime(2002,6,7)},

        };
        [HttpGet("GetByCriteria")]//عشان تحول الميثود ل API ENDPOINT
        public IActionResult GetByCriteria([FromQuery]SearchEmployeeDto employeeDto) //Method--> api Endpoint 
        {
            var result = from employee in employees
                         where (employeeDto.position == null || employee.Position.ToUpper().Contains(employeeDto.Posisiton.ToUpper()))&&
                         (employeeDto.Name == null || employee.FirstName.ToUpper().Contains(employeeDto.Name.ToUpper()))
                         orderby employee.Id descending
                         select new EmployeeDto
                         {
                             Id = employee.Id,
                             Name = employee.FirstName + " " + employee.LastNmae,
                             Position = employee.Position,
                             BirthDate = employee.BirthDate,
                             Email = employee.Email
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
            var result = employees.Select(x => new EmployeeDto
            {
                Id = x.Id,
                Name = x.FirstName + " " + x.LastNmae,
                Position = x.Position,
                BirthDate = x.BirthDate,
                Email = x.Email
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
                Id = (employees.LastOrDefault()?.Id ?? 0) + 1,  //عملنا هاي الحركه عشان الداتا
                                                                //اذا كانت فاضيه حيعمل ايرور فا
                                                                //؟؟ بتعمل الشرط الي هوه زيرو

                FirstName = employeeDto.FirstName,
                LastNmae = employeeDto.LastNmae,
                Email = employeeDto.Email,
                BirthDate = employeeDto.BirthDate,
                Position = employeeDto.Position,


            };

            employees.Add(employee);

            return Ok();
        }


        [HttpPut("Update")]
        public IActionResult Update([FromBody] SaveEmployeeDto employeeDto)
        {

            var employee = employees.FirstOrDefault(x => x.Id == employeeDto.Id);
            if (employee == null)
                return NotFound("Employee Not Exist");

            employee.FirstName = employeeDto.FirstName;
            employee.LastNmae = employeeDto.LastNmae;
            employee.Email = employeeDto.Email;
            employee.BirthDate = employeeDto.BirthDate;
            employee.Position = employeeDto.Position;
            return Ok();




        }
        [HttpDelete("Delete")]
        public IActionResult Delete([FromQuery] long id)
        {

            var employee = employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
                return NotFound("Employee Does Not Exist");
            employees.Remove(employee);
            return Ok();



        }

    }


}
