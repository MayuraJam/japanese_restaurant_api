using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using japanese_resturant_project.model.request;
//using japanese_resturant_project.model.request;
using System.Security.Claims;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using japanese_resturant_project.model.response;
using japanese_resturant_project.services;
using Dapper;

namespace japanese_resturant_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Microsoft.AspNetCore.Mvc.Controller
    {
        public static User user = new User(); //response model
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminController> _logger;
        ServiceFactory _service;

        public AdminController(IConfiguration configuration,ILogger<AdminController>logger, ServiceFactory service)
        {
        
            _configuration = configuration;
            _logger = logger;
            _service = service;
        }


        [HttpPost("register")]
        public ActionResult<User> RegiterHash(UserRequestModel request)
       // public async Task<IActionResult> RegiterPost1(UserRequestModel request)
        {
           // var response = await _service.AuthService().RegiterPost(request);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); //การแปลง password ให้เป็นรหัส Hash

            ////การนำค่าใส่ลงใน Model
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.massage = "Success";

            return Ok(user);
        }

        [HttpPost("login")]
       public ActionResult<User> RegiterJWT(UserRequestModel request)
        {
            try
            {
                if (user.Username != request.Username)
                {
                    user.massage = "User not found";
                    return BadRequest("user not found");
                }
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    user.massage = "Wrong password!!";
                    return BadRequest("Wrong password!!");
                }
                if(user.Username != request.Username && !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    user.massage = "Wrong password and Username!!";
                    return BadRequest("Wrong password and Username!!");
                }
                string token = CreateToken(user); //เมื่อทำการเข้าสู่ระบบได้แล้ว ให้ทำการแปลงข้อมูลให้อยู่ในรูป token 
                return Ok(token);
            }
            catch (Exception ex) { }
            {
                user.massage = "An error while processing to create Token";
                _logger.LogError("An error while processing to create Token");
                return StatusCode(500, "Internet server error");
            }

        }

        private string CreateToken(User user)
        {
            try
            {
                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(

                    _configuration.GetValue<string>("AppSettings:Token")));

                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: cred
                    );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return jwt;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating JWT token");
                throw new ApplicationException("An error occurred while creating JWT token", ex);
            }

        }
    }
}
