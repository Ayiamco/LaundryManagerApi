using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Services;
using LaundryApi.Models;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using static LaundryApi.Services.HelperMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LaundryController : ControllerBase
    {
        private readonly ILaundryDbService _context;
        private readonly IJwtAuthenticationManager jwtManager ;
        private LaundryApiContext context= new LaundryApiContext(new DbContextOptions<LaundryApiContext>());

        public LaundryController(ILaundryDbService _context, IJwtAuthenticationManager jwtManager )
        {
            this._context = _context;
            this.jwtManager = jwtManager;

        }

        [HttpGet]
        public async Task<ActionResult<Laundry>> GetLaundry(Guid id)
        {
            var user = await _context.FindAsync(id);
            return user;
        }

        //POST: /login
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] UserLoginDto user)
        {
            //get password hash
            string hashedPassword = HelperMethods.HashPassword(user.Password);

            var _user = _context.GetLaundry(user.Username);
            var response = new ResponseDto<string>()
            {
                statusCode = "400",
                message = "User does not exist",
            };
            if (_user == null)
                return BadRequest(response);
             
            if(_user.Password != hashedPassword)
            {
                response.message = "password is incorrect";
                return BadRequest(response);
            }
         
            //get jwt token
            string token = jwtManager.GetToken(user);
            response.statusCode = "200";
            response.message = "login details are correct";
            response.data = token;
            return Ok(response);

        }

        [AllowAnonymous]
        //POST: /register
        [HttpPost("register")]
        public async Task<ActionResult<Laundry>> PostAdminUser([FromBody] NewLaundryDto user)
        {
            if (user == null)
                return BadRequest();
            if ( user.Password != user.ConfirmPassword)
                return BadRequest(new { message="Password do not match" });

            try
            {
                //save new laundry to database
                Laundry laundry = await _context.Create(user);

                //override password hash
                laundry.Password = user.Password;

                return CreatedAtAction(nameof(GetLaundry),new { id=laundry.LaundryId},laundry);
            }
          
           catch
            {
                //log error
                throw new ArgumentException("some internal error occured");

            }
        }

        
        
        


    }
}
