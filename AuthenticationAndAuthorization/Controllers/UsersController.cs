using AuthenticationApp.Data;
using AuthenticationApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController(JwtOptions options,ApplicationDbContext dbContext) : ControllerBase
    {
        [HttpPost]
        [Route("auth")]
        public ActionResult<string> AuthenticateUser(AuthenticationRequest request)
        {
            var user=dbContext.Set<User>().FirstOrDefault(x=>x.Name==request.UserName && x.Password==request.Password);
            if (user == null)
                return Unauthorized();
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
              Issuer = options.Issuer,  
              Audience = options.Audience,  
              SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
              SecurityAlgorithms.HmacSha256),
              Subject=new ClaimsIdentity(new Claim[]
              {
                  new(ClaimTypes.NameIdentifier,user.Id.ToString()),
                  new(ClaimTypes.Name,user.Name.ToString()),
                  new(ClaimTypes.Role,"Admin"),
                  new(ClaimTypes.Role,"SuperUser"),
                  new("UserType","Employee"),
                  new("DateOfBirth","1992-01-01")
              })
            };
            var securityToken= tokenHandler.CreateToken(tokenDescriptor);
            var accessToken=tokenHandler.WriteToken(securityToken);
            return Ok(accessToken); 
        }
    }
}
