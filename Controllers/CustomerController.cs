using LaundryApi.Dtos;
using LaundryApi.Entites;
using LaundryApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Interfaces;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Models;


namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        //GET: api/customer/all
        [HttpGet("all")]
        public ActionResult GetCustomers()
        {
            var queryParam = Request.Query;
            var pageNumber=int.Parse(queryParam["page"]);
            customerRepository.GetCustomers(2,pageNumber);
            return Ok();
        }

        //POST: api/customer/new
        [HttpPost("new")]
        public async Task<ActionResult<CustomerDto>> AddCustomer([FromBody] NewCustomerDto newCustomer)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto<string>() { message=ErrorMessage.InvalidModel});

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
                response.statusCode = "404";
                if (e.Message == ErrorMessage.InvalidToken)
                {
                    response.message = ErrorMessage.InvalidToken;
                    return BadRequest(response);
                }
                else if (e.Message == ErrorMessage.UsernameAlreadyExist)
                {
                    response.message = ErrorMessage.UsernameAlreadyExist;
                    return BadRequest(response);
                }

                response.statusCode = "500";
                return StatusCode(500,response);
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
        [HttpPatch]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer([FromBody] CustomerDto customer)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                await customerRepository.UpdateCustomer(customer);
                return Ok();
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest(new ResponseDto<CustomerDto>() { message=ErrorMessage.EntityDoesNotExist});

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

        //GET:  api/customer/search?name={name}
        [HttpGet("search")]
        public ActionResult Search(string name,string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(name))
                    return BadRequest(new ResponseDto<string>() { message = ErrorMessage.NoEntityMatchesSearch });

                var customer = customerRepository.GetCustomer(name, username);
                return Ok(new ResponseDto<IEnumerable<CustomerDto>>() { data = customer, statusCode = "200", });
            }
            catch(Exception e)
            {
                if(e.Message == ErrorMessage.NoEntityMatchesSearch)
                    return BadRequest(new ResponseDto<string>() { message = ErrorMessage.NoEntityMatchesSearch });

                //if you get to these point somthing unusual occured
                return StatusCode(500);
            }
            
        }

        [HttpGet("debtors")]
        public ActionResult GetDebtors()
        {
            try
            {
                var debtors = customerRepository.GetCustomer(HttpContext.User.Identity.Name);
                return Ok(new ResponseDto<IEnumerable<CustomerDto>> { data = debtors, statusCode = "200", message = "" });
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.NoEntityMatchesSearch)
                    return StatusCode(204);

                return StatusCode(500);
            }
            
        }

        
    }
}