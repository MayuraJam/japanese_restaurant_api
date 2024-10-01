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
using japanese_resturant_project.model.response.adminResponse;
using japanese_resturant_project.model.DatabaseModel;
using Microsoft.Data.SqlClient;

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
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string _connection;
        public AdminController(IConfiguration configuration,ILogger<AdminController>logger, ServiceFactory service,IWebHostEnvironment hostEnvironment)
        {
        
            _configuration = configuration;
            _logger = logger;
            _service = service;
            this._hostEnvironment = hostEnvironment;
            _connection = Environment.GetEnvironmentVariable("DBConnection");
        }


        [HttpPost("registerTest")]
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

        [HttpPost("loginTest")]
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

            //ทดสอบการใช้งานการเข้ารหัส
        }
        [HttpGet("GetStaftData")]
        public async Task<IActionResult> GetStaftProfile(string staftID)
        {
            var response = await _service.AdminService().GetStaftProfile(staftID);
            return Ok(response);
        }
        [HttpPut("UpdateStaftData")]
        public async Task<IActionResult> UpdateStaftProfile(UpdateStaftProfileRequest request)
        {
            var response = await _service.AdminService().UpdateStaftProfile(request);
            return Ok(response);
        }
        [HttpGet("OptionGet")]
        public async Task<IActionResult> GetOptionList()
        {
            var response = await _service.AdminService().GetOptionList();
            return Ok(response);
        }
        [HttpPost("AddOption")]
        public async Task<IActionResult> AddOption(OptionRequest request)
        {
            var response = await _service.AdminService().AddOption(request);
            return Ok(response);
        }
        [HttpPut("UptionOption")]
        public async Task<IActionResult> UpdateOption(Option_tb_ request)
        {
            var response = await _service.AdminService().UpdateOption(request);
            return Ok(response);
        }
        [HttpDelete("DeleteOption")]
        public async Task<IActionResult>DeleteOption([FromBody]  Guid optionID)
        {
            var response = await _service.AdminService().DeleteOption(optionID);
            return Ok(response);
        }
        [HttpPost("GetMenu")]
        public async Task<IActionResult> GetMenuList2(SearchRequest request)
        {
            var response = await _service.AdminService().GetMenuList2(request);
            return Ok(response);
        }
        [HttpPost("GetMenuByID/{menuID}")]
        public async Task<AdminResponse> GetMenuByID(string menuID)
        {
            var response = await _service.AdminService().GetMenuByID(menuID);
            return response;
        }

        [HttpPost("AddMenu")]
        public async Task<IActionResult> AddMenu([FromForm] MenuRequest request)
        {   
            var response = await _service.AdminService().AddMenu(request);
            return Ok(response);
        }
        [HttpPut("UpdateMenu")]
        public async Task<IActionResult> UpdateMenu(MenuUpdate request)
        {
            var response = await _service.AdminService().UpdateMenu(request);
            return Ok(response);
        }
        [HttpDelete("DeleteMenu/{menuID}")]
        public async Task<IActionResult> DeleteMenu(string menuID)
        {
            var response = await _service.AdminService().DeleteMenu(menuID);
            return Ok(response);
        }
        [NonAction]
        public async Task SaveImage(IFormFile imageFile)
        {
            
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images");
            using(var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            //return imageFile.Name;
        }
        [HttpGet("GetTable")]
        public async Task<IActionResult> GetTableList()
        {
            var response = await _service.AdminService().GetTableList();
            return Ok(response);
        }

        [HttpPost("GetOrder")]
        public async Task<IActionResult> GetOrderForAdmin(SearchOrderRequest request)
        {
            var response = await _service.AdminService().GetOrderForAdmin(request);
            return Ok(response);
        }
        [HttpGet("GetOrderByID/{orderID}")]
        public async Task<IActionResult> GetOrderByID(string orderID)
        {
            var response = await _service.AdminService().GetOrderByID(orderID);
            return Ok(response);
        }
        [HttpPut("ConfirmOrder")]
        public async Task<AdminResponse> ConfirmOrder(ConfirmRequest request)
        {
            var response = await _service.AdminService().ConfirmOrder(request);
            return response;
        }
        [HttpGet("GetOrderStatus")]
        public async Task<IActionResult> GetOrderDetail()
        {
            var response = await _service.AdminService().GetOrderDetail();
            return Ok(response);
        }
        [HttpPut("UpdateOrderStatus")]
        public async Task<IActionResult> updateOrderStatus(UpdateOrderStatusRequest request)
        {
            var response = await _service.AdminService().updateOrderStatus(request);
            return Ok(response);
        }
        [HttpGet("GetNotification")]
        public async Task<IActionResult> GetNotification()
        {
            var response = await _service.AdminService().GetNotification();
            return Ok(response);
        }

        [HttpPut("ReableNotification/{notificationID}")]
        public async Task<IActionResult> Readable(string notificationID)
        {
            var response = await _service.AdminService().Readable(notificationID);
            return Ok(response);
        }
        [HttpDelete("DeleteNotification/{notificationID}")]
        public async Task<IActionResult> DeleteNotification(string notificationID)
        {
            var response = await _service.AdminService().DeleteNotification(notificationID);
            return Ok(response);
        }
    }
}
