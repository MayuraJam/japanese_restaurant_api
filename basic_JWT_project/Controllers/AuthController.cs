
using Microsoft.AspNetCore.Mvc;
using japanese_resturant_project.services;
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
        [HttpGet("GetMember/{roleName}")]
        public async Task<IActionResult> GetMember(string roleName)
        {
            var reponse = await _service.AuthService().GetMember(roleName);
            return Ok(reponse);
        }
        [HttpPost("LoginCustomerMember")]
        public async Task<IActionResult> ToLoginCustomer(Login request)
        {
            var reponse = await _service.AuthService().ToLoginCustomer(request);
            return Ok(reponse);
        }
        [HttpPost("LoginStaft")]
        public async Task<IActionResult> LoginStaft(LoginStaftRequestModel request)

        {
            var reponse = await _service.AuthService().LoginStaft(request);
            return Ok(reponse);
        }
        [HttpPut("LogoutStaft/{staftID}")]
        public async Task<IActionResult> LogoutStaft(string staftID)

        {
            var reponse = await _service.AuthService().LogoutStaft(staftID);
            return Ok(reponse);
        }
        [HttpDelete("DelMemberAccount/{memberID}")]
        public async Task<IActionResult> DeleteMemberAccount(Guid memberID)

        {
            var reponse = await _service.AuthService().DeleteMemberAccount(memberID);
            return Ok(reponse);
        }
    }
}
