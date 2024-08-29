using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Security.Claims;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using japanese_resturant_project.model.response;
using japanese_resturant_project.services;
using Dapper;

using Microsoft.AspNetCore.Mvc;
using japanese_resturant_project.model.request;

namespace japanese_resturant_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {

        private readonly ServiceFactory _service;

        public AuthController(ServiceFactory service)
        {
            _service = service;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> AddRegister(RegisterRequest request)
        {
            var reponse = await _service.AuthService().AddRegister(request);
            return Ok(reponse);
        }
    }
}
