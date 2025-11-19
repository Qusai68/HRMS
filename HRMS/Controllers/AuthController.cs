using HRMS.DbContexts;
using HRMS.Dtos.Auth;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Dependency Injection
        private readonly HRMSContexts _dbcontext;
        public AuthController(HRMSContexts dbcontext)
        {
            _dbcontext = dbcontext;
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = _dbcontext.Users.FirstOrDefault(x => x.UserName.ToUpper() == loginDto.UserName.ToUpper());
                if (user == null)
                {
                    return NotFound("Invalid Username or Password");
                }

                //if (loginDto.Password == user.HashedPassword)
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.HashedPassword))
                {

                    return BadRequest("Invalid Username or Password");


                }
                var token = GenrateJwtToken(user);
                return Ok(token);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);


            }
        }
        private string GenrateJwtToken(User user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            //Role --> HR/Admin/Developer/Manager
            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            }
            else
            {
                var employee = _dbcontext.Employees.Include(x => x.Lookup).FirstOrDefault(x => x.UserId == user.Id);
                claims.Add(new Claim(ClaimTypes.Role, employee.Lookup.Name));
            }

            // Secret Key= WHAFWEI#!@S!!112312WQEQW@RWQEQW432
            //Encoding.UTF8.GetBytes("WHAFWEI#!@S!!112312WQEQW@RWQEQW432")== [45,56,67,87,....]
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("WHAFWEI#!@S!!112312WQEQW@RWQEQW432"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//signing the token
            var tokenSetting = new JwtSecurityToken(
                claims: claims,// user information
                signingCredentials: creds,// secret key
                expires: DateTime.Now.AddHours(2)// when does the token expire

                );
            var toknHandler = new JwtSecurityTokenHandler();
            var token = toknHandler.WriteToken(tokenSetting);
            return token;



        }
    }
}
