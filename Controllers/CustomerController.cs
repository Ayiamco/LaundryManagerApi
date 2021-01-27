using LaundryApi.Dtos;
using LaundryApi.Models;
using LaundryApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using static LaundryApi.Infrastructure.HelperMethods;


namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        //POST: api/customer/new
        [HttpPost("new")]
        public async Task<ActionResult<CustomerDto>> AddCustomer([FromBody] CustomerDto newCustomer)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            CustomerDto customer;
            ResponseDto<CustomerDto> response = new ResponseDto<CustomerDto>()
            {
                statusCode = "201",
                message = "successfully created new customer"
            };
            try
            {
                if (HttpContext.IsInRole(RoleNames.LaundryEmployee) || HttpContext.IsInRole(RoleNames.LaundryOwner))
                {
                    customer = await customerRepository.AddCustomer(newCustomer, HttpContext.User.Identity.Name,HttpContext.GetUserRole());
                    response.data = customer;
                    return CreatedAtAction(nameof(GetCustomer), new { id=customer.Id}, response);

                }

                return Unauthorized();
            }
            catch (Exception e)
            {

                if (e.Message == ErrorMessage.InvalidToken)
                {
                    // request does not contain valid jwt token.
                    response.statusCode = "404";
                    response.message = ErrorMessage.InvalidToken;
                    return BadRequest();
                }

                return StatusCode(500);

            }
            

        }
        

        //GET: api/customer/{id}
        [HttpGet("{id}")]
        public ActionResult<CustomerDto> GetCustomer(Guid id)
        {
            try
            {
                var customer = customerRepository.GetCustomer(id);
                return Ok(customer);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest(new ResponseDto<CustomerDto>() { message=ErrorMessage.EntityDoesNotExist});
                
                //if you get to this point something unforseen happened
                return StatusCode(500);
            }
        }

        //PUT: api/customer
        [HttpPut]
        public ActionResult<CustomerDto> UpdateCustomer([FromBody] CustomerDto customer)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                customerRepository.UpdateCustomer(customer);
                return Ok();
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return StatusCode(400);

                //if you get to this point something unforseen happened
                return StatusCode(500);
            }

        }

        //DELETE: api/customer/{customerId}
        [HttpDelete("{customerId}")]
        public ActionResult DeleteCustomer(Guid customerId)
        {

            try
            {
                customerRepository.DeleteCustomer(customerId);
                return Ok();
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    return NotFound();

                //if you get to this point something ususual occured
                return StatusCode(500);
            }

        }

        public ActionResult GetCustomerByName (string name)
        {
            return Ok();
        }
    }
}
