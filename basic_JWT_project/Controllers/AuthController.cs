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
using Azure.Core;

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
        [HttpPost("GetMember")]
        public async Task<IActionResult> GetMember([FromBody] string roleName)
        {
            var reponse = await _service.AuthService().GetMember(roleName);
            return Ok(reponse);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> ToLogin(Login request)
        {
            var reponse = await _service.AuthService().ToLogin(request);
            return Ok(reponse);
        }
        [HttpPost("LoginStaft")]
        public async Task<IActionResult> LoginStaft(LoginStaftRequestModel request)

        {
            var reponse = await _service.AuthService().LoginStaft(request);
            return Ok(reponse);
        }

    }
}
