using LaundryApi.Dtos;
using LaundryApi.Models;
using LaundryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using static LaundryApi.Services.HelperMethods;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerDbService _context;

        public CustomerController(ICustomerDbService context)
        {
            _context = context;
        }

        [HttpPost("new")]
        public async Task<ActionResult<Customer>> AddCustomer([FromBody] CustomerDto newCustomer)
        {
            try
            {
                var currentUser = HttpContext.User;
                
                Customer customer;
                string username;
                
                if (currentUser.HasClaim(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"))
                {
                    username = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value);
                    try
                    {
                        customer = await _context.AddCustomer(newCustomer, username);
                       
                        ResponseDto<CustomerDto> response = new ResponseDto<CustomerDto>()
                        {
                            statusCode = "201",
                            data = newCustomer,
                            message = "successfully created new customer"
                        };
                        return Created("", response);
                    }
                    catch (Exception e)
                    {
                        if (e.Message == ErrorMessage.InvalidToken)
                        {
                            // request does not contain valid jwt token.
                            return BadRequest();
                        }
                        return BadRequest();

                    }

                }
                ResponseDto<Customer> errorResponse = new ResponseDto<Customer>()
                {
                    statusCode = "401",
                    message = ErrorMessage.InvalidToken
                };

                return BadRequest(errorResponse);

            }
            catch
            {
                 return StatusCode(500);
            }
            
           
        }
    }
}
