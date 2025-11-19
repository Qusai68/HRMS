
using HRMS.DbContexts;
using HRMS.DTO.Employee;
using HRMS.Dtos.Employee;
using HRMS.Mod411els;
using HRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Xml.Linq;

namespace HRMS.Controllers
{
    [Authorize]// save all the endpoints in this controller
    [Route("api/[controller]")] // data Annotation
    [ApiController]// data Annotation 
    public class EmployeesController : ControllerBase
    {
        public static List<Employee> employees = new List<Employee>()
        {

            //new Employee(){ Id=1,FirstName ="QUSAI",LastName="Mohammad", Email="Qusai@123.com",Position="Developer", BirthDate=new DateTime(2005,10,20)},
            //new Employee() { Id=2,FirstName ="Rand ",LastName="Abbad", Position="HR", BirthDate=new DateTime(2003,11,5)},
            // new Employee(){ Id=3,FirstName ="Mohammad",LastName="Ezate", Position="Developer", BirthDate=new DateTime(2002,6,7)},

        };
        private readonly HRMSContexts _dbcontext;
        public EmployeesController(HRMSContexts dbcontext)
        {
            _dbcontext = dbcontext;
        }


        [HttpGet("GetByCriteria")]//عشان تحول الميثود ل API ENDPOINT
        public IActionResult GetByCriteria([FromQuery] SearchEmployeeDto employeeDto) //Method--> api Endpoint 
        {

            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;// admin, HR, Developer, Manager
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;// id


                var result = from employee in _dbcontext.Employees
                             from department in _dbcontext.Departments.Where(x => x.Id == employee.DepartmentId).DefaultIfEmpty()//==left join
                             from manager in _dbcontext.Employees.Where(x => x.Id == employee.ManagerId).DefaultIfEmpty()//left join
                             from Lookup in _dbcontext.Lookups.Where(x => x.Id == employee.PositionId)
                             where
                             (employeeDto.PositionId == null || employee.PositionId == employeeDto.PositionId) &&
                             (employeeDto.Name == null || employee.FirstName.ToUpper().Contains(employeeDto.Name.ToUpper()))
                             orderby employee.Id descending
                             select new EmployeeDto
                             {
                                 Id = employee.Id,
                                 Name = employee.FirstName + " " + employee.LastName,
                                 PositionId = employee.PositionId,
                                 PositionName = Lookup.Name,
                                 BirthDate = employee.BirthDate,
                                 Email = employee.Email,
                                 Salary = employee.Salary,
                                 DepartmentId = employee.DepartmentId,
                                 DepartmentName = department.Name,
                                 ManagerId = employee.ManagerId,
                                 ManagerName = manager.FirstName,
                                 UserId = employee.UserId
                             };
                if (role?.ToUpper() != "ADMIN" && role?.ToUpper() != "HR")
                {
                    result = result.Where(x => x.UserId == long.Parse(userId));// عشان المستخدم العادي يشوف بياناته فقط 

                }

                return Ok(result);
                //return Ok("Name ; Qusai , Age :20");//200
                // return NotFound("Data Not Found");//404
                //return BadRequest("Data Missing");//500
                //return Ok(new { Name = "Qusai", Age = 20 });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet("GetById/{id}")]// Route parameter
        public IActionResult GetById(long id)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;// admin, HR, Developer, Manager
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;// id


                if (id == 0)
                    return BadRequest("Id Value Is Invalid");
                var result = _dbcontext.Employees.Select(x => new EmployeeDto // projection
                {
                    Id = x.Id,
                    Name = x.FirstName + " " + x.LastName,
                    PositionId = x.PositionId,
                    PositionName = x.Lookup.Name,
                    BirthDate = x.BirthDate,
                    Email = x.Email,
                    Salary = x.Salary,
                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department.Name,
                    ManagerId = x.ManagerId,
                    ManagerName = x.Manager.FirstName,
                    UserId = x.UserId

                }).FirstOrDefault(x => x.Id == id);
                
                if (result == null)
                {
                    return NotFound("Employee Not Found");
                }

                if (role?.ToUpper() != "ADMIN" && role?.ToUpper() != "HR")
                {
                    if (result.UserId != long.Parse(userId))
                    {
                        return Forbid();// 403
                    }
                }
              

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        [Authorize(Roles = "HR,Admin")]
        [HttpPost("Add")]
        public IActionResult Add(long id, [FromBody] SaveEmployeeDto employeeDto)
        {
            try
            {
                var user = new User()
                {
                    Id = 0,
                    UserName = $"{employeeDto.FirstName}_{employeeDto.LastName}_HRMS",//اسم المستخدم بيتكون من الاسم الاول والاخير
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword($"{employeeDto.FirstName}@123"),//كلمة المرور الافتراضية
                    IsAdmin = false
                };
                var isUsername = _dbcontext.Users.Any(x => x.UserName.ToUpper() == user.UserName.ToUpper());
                if (isUsername == true)
                {
                    return BadRequest("Username Already Exists, pleas choose another one");
                }
                _dbcontext.Users.Add(user);

                var employee = new Employee()
                {
                    //Id = (employees.LastOrDefault()?.Id ?? 0) + 1,  //عملنا هاي الحركه عشان الداتا
                    Id = 0,                                                //اذا كانت فاضيه حيعمل ايرور فا
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    Email = employeeDto.Email,
                    BirthDate = employeeDto.BirthDate,
                    PositionId = employeeDto.PositionId,
                    Salary = employeeDto.Salary,
                    DepartmentId = employeeDto.DepartmentId,
                    ManagerId = employeeDto.ManagerId,
                    //UserId = user.Id
                    User = user // عشان تربط بين اليوزر والموظف


                };

                _dbcontext.Employees.Add(employee);
                _dbcontext.SaveChanges();//عشان تحفظ التعديلات في الداتا بيس

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "HR,Admin")]
        [HttpPut("Update")]
        public IActionResult Update([FromBody] SaveEmployeeDto employeeDto)
        {

            try
            {

                var employee = _dbcontext.Employees.FirstOrDefault(x => x.Id == employeeDto.Id);
                /* if (employee == null)
                 return NotFound("Employee Not Exist");*/


                employee.FirstName = employeeDto.FirstName;
                employee.LastName = employeeDto.LastName;
                employee.Email = employeeDto.Email;
                employee.BirthDate = employeeDto.BirthDate;
                employee.PositionId = employeeDto.PositionId;
                employee.Salary = employeeDto.Salary;
                employee.DepartmentId = employeeDto.DepartmentId;
                employee.ManagerId = employeeDto.ManagerId;

                _dbcontext.SaveChanges();
                return Ok();

            }
            catch (NullReferenceException ex)
            {
                return NotFound("Employee Does Not Exist");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
        [Authorize(Roles = "HR,Admin")]
        [HttpDelete("Delete")]

        public IActionResult Delete([FromQuery] long id)
        {
            try
            {
                var employee = _dbcontext.Employees.FirstOrDefault(x => x.Id == id);
                if (employee == null)
                    return NotFound("Employee Does Not Exist");
                _dbcontext.Employees.Remove(employee);

                _dbcontext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

    }


}
