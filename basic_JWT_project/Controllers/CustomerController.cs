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
using japanese_resturant_project.model.response.customerResponse;
using Azure.Core;

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
        public async Task<IActionResult> GetCartBytableID(string tableID)
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
        [HttpPut("UpdateCart")]
        public async Task<IActionResult> UpdateCart(UpdateCartRequest request)
        {
            var response = await _service.CustomerService().UpdateCart(request);
            return Ok(response);
        }
        [HttpDelete("DeleteCart/{cartID}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartID)
        {
            var response = await _service.CustomerService().DeleteCartItem(cartID);
            return Ok(response);
        }

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder(AddOrderRequest request)
        {
            var response = await _service.CustomerService().AddOrder(request);
            return Ok(response);
        }

        [HttpGet("GetOrderDetail/{orderID}")]
        public async Task<CustomerResponse> GetOrderDetail(string orderID)
        {
            var response = await _service.CustomerService().GetOrderDetail(orderID);
            return response;
        }

        [HttpGet("GetOrder/{customerID}")]
        public async Task<CustomerResponse> GetOrder(string customerID)
        {
            var response = await _service.CustomerService().GetOrder(customerID);
            return response;
        }
        [HttpPut("CancleOrder/{orderID}")]
        public async Task<IActionResult> CancleOrder(string orderID)
        {
            var response = await _service.CustomerService().CancleOrder(orderID);
            return Ok(response);
        }
        [HttpPost("AddPayment")]
        public async Task<IActionResult> AddPayment(PaymentRequest request)
        {
            var response = await _service.CustomerService().AddPayment(request);
            return Ok(response);
        }
        [HttpPost("AddNotification")]
        public async Task<IActionResult> AddNotification(NotificationRequest request)
        {
            var response = await _service.CustomerService().AddNotification(request);
            return Ok(response);
        }
        [HttpGet("GetPyment/{orderID}")]
        public async Task<CustomerResponse> GetPayment(string orderID)
        {
            var response = await _service.CustomerService().GetPayment(orderID);
            return response;
        }
    }
}
