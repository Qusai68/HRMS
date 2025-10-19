using HRMS.Dtos.Departments;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        public List<Department> departments = new List<Department>()
        {
        new Department(){Id =1, Name ="Human Resources" , Description="Hr Department" , FloorNumber=1 },

        new Department(){Id =2, Name ="Finance" , Description="Finance Department" , FloorNumber=2 },
        new Department(){Id =3, Name ="Developmant" , Description="Developmaint Department" , FloorNumber=1 },


        };
        [HttpGet("GetByCriteri")]
        public IActionResult GetByCriteri(string? name )
        {
            var result = from department in departments
                         where (name == null || department.Name == name)
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
    }
}
