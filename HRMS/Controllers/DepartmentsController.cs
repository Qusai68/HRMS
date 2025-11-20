using HRMS.DbContexts;
using HRMS.Dtos.Departments;
using HRMS.Dtos.Employee;
using HRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HRMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        //public static List<Department> departments = new List<Department>()
        //{
        //new Department(){Id =1, Name ="Human Resources" , Description="Hr Department" , FloorNumber=1 },
        //new Department(){Id =2, Name ="Finance" , Description="Finance Department" , FloorNumber=2 },
        //new Department(){Id =3, Name ="Developmant" , Description="Developmaint Department" , FloorNumber=1 },

        //};
        private readonly HRMSContexts _dbcontext;
        public DepartmentsController(HRMSContexts dbcontext)
        {
            _dbcontext = dbcontext;
        
        }
        [HttpGet("GetByCriteri")]
        public IActionResult GetByCriteri([FromQuery] FilterDepartmentsDto filterDto)
        {
            try
            {
                var result = from department in _dbcontext.Departments
                             from type in _dbcontext.Lookups.Where(x => x.Id == department.TypeId)
                             where (filterDto.Name == null || department.Name.ToUpper().Contains(filterDto.Name.ToUpper())) &&
                             (filterDto.FloorNumber == null || department.FloorNumber == filterDto.FloorNumber)
                             orderby department.Id descending
                             select new DepartmentDto
                             {
                                 Id = department.Id,
                                 Name = department.Name,
                                 Description = department.Description,
                                 FloorNumber = department.FloorNumber,
                                 TypeId = department.TypeId,
                                 TypeName = type.Name



                             };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(long id)
        {
            try
            {
                var deartment = _dbcontext.Departments.Select(x => new DepartmentDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    FloorNumber = x.FloorNumber,
                    TypeId = x.TypeId,
                    TypeName = x.Type.Name




                }).FirstOrDefault(x => x.Id == id);
                if (deartment == null)
                    return NotFound("Department Not Found");
                return Ok(deartment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admon,HR")]
        [HttpPost("Add")]
        public IActionResult Add([FromBody] SaveDeparetmentDto saveDto)
        {
            try
            {

                var deartment = new Department
                {
                    Id = 0,
                    Name = saveDto.Name,
                    Description = saveDto.Description,
                    FloorNumber = saveDto.FloorNumber,
                    TypeId = saveDto.TypeId

                };
                _dbcontext.Departments.Add(deartment);
                _dbcontext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin,HR")]
        [HttpPut("Update")]
        public IActionResult Update([FromBody] SaveDeparetmentDto saveDto)
        {
            try
            {
                var department = _dbcontext.Departments.FirstOrDefault(x => x.Id == saveDto.Id);
                if (department == null)
                    return NotFound("Department Not Found");
                department.Name = saveDto.Name;
                department.Description = saveDto.Description;
                department.FloorNumber = saveDto.FloorNumber;
                department.TypeId = saveDto.TypeId;
                _dbcontext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin,HR")]
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(long id)
        {
            try
            {
                var department = _dbcontext.Departments.FirstOrDefault(x => x.Id == id);
                if (department == null)
                    return NotFound("Department Not Found");
                var employeeAssociate = _dbcontext.Employees.Any(x => x.DepartmentId == id);//check if any employee is associated with this department
                if (employeeAssociate)
                { 
                 return BadRequest("Cannot Delete Department Associated with Employees");

                }
                _dbcontext.Departments.Remove(department);
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

