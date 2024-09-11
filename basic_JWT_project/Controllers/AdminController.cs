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

        [HttpGet("GetMenu")]
        public async Task<IActionResult> GetMenuList()
        {
            //var response = await _service.AdminService().GetMenuList();
            //return Ok(response);
            var Response = new AdminResponse()
            {
                menuList = new List<Menu_tb>()
            };
            //function ในการเปลี่ยนจาก imagePath => imageFile
            
            try
            {
                if (string.IsNullOrEmpty(_connection))
                {
                    return BadRequest("Database connecting string is not");
                }
                using (var dbConnection = new SqlConnection(_connection))
                {
                    await dbConnection.OpenAsync();
                    //ภาพขึ้นล่ะ ถ้าใช้เป็นformat https://localhost:7202/Image/16aba539-bbd5-472d-bd14-31ab4227c4ec_food.jpg อันนี้เป็นตัวอย่าง
                    var sql = @"
                SELECT 
                 m.menuID,
                 m.menuName,
                 m.menuDescription,
                 m.unitPrice,
                 m.categoryName,
                 m.optionID,
                 m.createDate,
                 m.updateDate,
                 m.rating,
                 m.imageName,
                 o.optionName,
                 o.value
                 FROM  menu_tb m
                 LEFT JOIN
                   option_tb o ON o.optionID = m.optionID
                 ";
                    var menuValue = await dbConnection.QueryAsync<Menu_tb>(sql);

                    // Check if any reservations were found
                    if (menuValue != null && menuValue.Any())
                    {
                        // Populate the booking response with the reservations
                        //ดึง imageName จาก database
                        //ใส่ function  แปลงในตรงนี้ แล้วก็นำไปใส่ในตัว Menu_tb
                        //image path from database => get file => get to front end
                        //Response.menuList = menuValue.ToList();

                        Response.menuList.Select(x => new Menu_tb()
                        {
                            menuID = x.menuID,
                            menuName = x.menuName,
                            menuDescription = x.menuDescription,
                            unitPrice = x.unitPrice,
                            categoryName = x.categoryName,
                            optionID = x.optionID,
                            createDate = x.createDate,
                            updateDate = x.createDate,
                            rating = x.rating,
                            imageName = x.imageName,
                            optionName = x.optionName,
                            value = x.value,
                            imageSrc = String.Format("{0}://{1}{2}/Image/{3}",Request.Scheme, Request.Host,Request.PathBase,x.imageName)

                        }).ToList();
                        //ดึงข้อมูลออกมา

                        Response.message = "successfully.";
                        Response.success = true;

                    }
                    else
                    {
                        // Handle case where no reservations were found
                        Response.message = "Not found data 404.";
                        Response.success = false;

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., log the error
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Set error message in the bookingResponse
                Response.message = $"An error occurred while fetching the reservations: {ex.Message}";
                Response.success = false;
            }

            return Ok(Response);
        }
        [HttpGet("GetMenu2")]
        public async Task<IActionResult> GetMenuList2()
        {
            var response = await _service.AdminService().GetMenuList2();
            return Ok(response);
        }
        [HttpPost("GetMenuByID/{menuID}")]
        public async Task<AdminResponse> GetMenuByID(Guid menuID)
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
        [HttpPut("UptionMenu")]
        public async Task<IActionResult> UpdateMenu([FromForm] MenuUpdate request)
        {
            var response = await _service.AdminService().UpdateMenu(request);
            return Ok(response);
        }
        [HttpDelete("DeleteMenu")]
        public async Task<IActionResult> DeleteMenu([FromBody]  Guid menuID)
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
    }
}
