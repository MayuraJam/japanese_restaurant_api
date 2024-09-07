using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.Security.Claims;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using japanese_resturant_project.services;
using Dapper;
using japanese_resturant_project.model.response;
using japanese_resturant_project.model.request.customerRequest;

namespace japanese_resturant_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ServiceFactory _service;

        public CustomerController(ServiceFactory service) {
            _service = service;
        }

        [HttpGet("GetFoodData")]

        public async Task<IActionResult> GetMenuData()
        {
            var response = await _service.TestService().GetMenuData();
            return Ok(response);
        }

        [HttpPut("OpenTable")]
        public async Task<IActionResult> OpenTable(OpenTableRequest request)
        {
            var response = await _service.CustomerService().OpenTable(request);
            return Ok(response);
        }

        [HttpPut("CloseTable")]
        public async Task<IActionResult> CloseTable(OpenTableRequest request)
        {
            var response = await _service.CustomerService().CloseTable(request);
            return Ok(response);
        }
    }
}
