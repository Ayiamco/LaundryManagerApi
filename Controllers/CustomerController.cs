using LaundryApi.Dtos;
using LaundryApi.Models;
using LaundryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly LaundryApiContext _context;
        //private readonly IJwtAuthenticationManager jwtManager;

        public CustomerController(LaundryApiContext context)
        {
            _context = context;
            //this.jwtManager = jwtManager;

        }
       [HttpPost("add")]
        public async Task<ActionResult<Customer>> AddCustomer([FromBody] CustomerDto customer)
        {
            try
            {
                var currentUser = HttpContext.User;
                Laundry laundry;
                string username;
                if (currentUser.HasClaim(c => c.Type == "Name"))
                {
                    username = Convert.ToString(currentUser.Claims.FirstOrDefault(c => c.Type == "Name").Value);
                    laundry = (Laundry)_context.Laundries.Where(u => u.Username == username);
                    await _context.Customers.AddAsync(new Customer()
                    {
                        Name = customer.Name,
                        Address = customer.Address,
                        CreatedAt = DateTime.Now,
                        LaundryId = laundry.LaundryId,
                        Email = customer.Email,
                    });

                    await _context.SaveChangesAsync();
                }


            }
            catch
            {
                // request does not contain authorization header.
                return BadRequest();
            }
            return Ok();
        }
    }
}
