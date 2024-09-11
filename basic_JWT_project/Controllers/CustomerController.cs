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
        [HttpPost("GetCart/{tableID}")]
        public async Task<IActionResult> GetCartBytableID(Guid tableID)
        {
            var response = await _service.CustomerService().GetCartBytableID(tableID);
            return Ok(response);
        }
        [HttpPost("AddCart")]
        public async Task<IActionResult> AddCart(AddCartRequest request)
        {
            var response = await _service.CustomerService().AddCart(request);
            return Ok(response);
        }
        [HttpDelete("DeleteCart/{cartID}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartID)
        {
            var response = await _service.CustomerService().DeleteCartItem(cartID);
            return Ok(response);
        }
    }
}
