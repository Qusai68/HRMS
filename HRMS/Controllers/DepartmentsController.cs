using HRMS.Dtos.Departments;
using HRMS.Dtos.Employee;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        public static List<Department> departments = new List<Department>()
        {
        new Department(){Id =1, Name ="Human Resources" , Description="Hr Department" , FloorNumber=1 },

        new Department(){Id =2, Name ="Finance" , Description="Finance Department" , FloorNumber=2 },
        new Department(){Id =3, Name ="Developmant" , Description="Developmaint Department" , FloorNumber=1 },


        };
        [HttpGet("GetByCriteri")]
        public IActionResult GetByCriteri([FromQuery] FilterDepartmentsDto filterDto)
        {
            var result = from department in departments
                         where (filterDto.Name == null || department.Name.ToUpper().Contains(filterDto.Name.ToUpper())) &&
                         (filterDto.FloorNumber == null || department.FloorNumber == filterDto.FloorNumber)
                         orderby department.Id descending
                         select new DepartmentDto
                         {
                             Id = department.Id,
                             Name = department.Name,
                             Description = department.Description,
                             FloorNumber = department.FloorNumber,


                         };
            return Ok(result);

        } 
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(long id)
        {
            var deartment = departments.Select(x => new DepartmentDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                FloorNumber = x.FloorNumber,



            }).FirstOrDefault(x => x.Id == id);
            if (deartment == null)
                return NotFound("Department Not Found");
            return Ok(deartment);
        }
        [HttpPost("Add")]
        public IActionResult Add([FromBody]SaveDeparetmentDto saveDto)
        {
            var deartment = new Department
            {
                Id = (departments.LastOrDefault()?.Id ?? 0) + 1,
                Name = saveDto.Name,
                Description = saveDto.Description,
                FloorNumber = saveDto.FloorNumber,

            };
            departments.Add(deartment);
            return Ok();

        }
        [HttpPut("Update")]
        public IActionResult Update([FromBody] SaveDeparetmentDto saveDto)
        { 
        var department =departments.FirstOrDefault(x => x.Id == saveDto.Id);
            if (department==null)
                return NotFound("Department Not Found");
            department.Name = saveDto.Name;
            department.Description = saveDto.Description;
            department.FloorNumber = saveDto.FloorNumber;
            return Ok();
        }
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(long id)
        {
            var department = departments.FirstOrDefault(x => x.Id == id);
            if (department == null)
                return NotFound("Department Not Found");
            departments.Remove(department);
            return Ok();

        }
    }
}

