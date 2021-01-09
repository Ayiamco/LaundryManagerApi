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

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaundryController : ControllerBase
    {
        private readonly ILaundryContext _context;
        private readonly IJwtAuthenticationManager jwtManager ;

        public LaundryController(ILaundryContext _context, IJwtAuthenticationManager jwtManager )
        {
            this._context = _context;
            this.jwtManager = jwtManager;

        }

        //[HttpGet]
        //public async Task<ActionResult<Laundry>> GetAdminUser(int id)
        //{
        //   var user=await _context.Laundries.FindAsync( id);
        //    return user;
        //}

        ////POST: /login
        //[HttpPost("login")]
        //public  ActionResult<string> Login ([FromBody] UserLoginDto user)
        //{
        //    //get password hash
        //    string hashedPassword=HelperMethods.HashPassword(user.Password);
        //    var _user = _context.Laundries.FirstOrDefault(_user => _user.Username == user.Username);
        //    if (_user == null)
        //        return NotFound(new { message="Username does not exist "});
        //    if (_user.Password != hashedPassword)
        //        return NotFound(new { message = "incorrect password" });

        //    //get jwt token
        //    string token = jwtManager.Authenticate(user);
        //    return Ok(new { token });

        //}

        //POST: /register
        [HttpPost("register")]
        public async Task<ActionResult<Laundry>> PostAdminUser([FromBody] NewLaundryDto user)
        {
            if (user == null)
                return BadRequest();
            if ( user.Password != user.ConfirmPassword)
                return BadRequest(new { message="Password do not match" });


            //save new laundry to database
            Laundry laundry= await  _context.Create(user);
            //get jwt token
            string token = jwtManager.Authenticate( new UserLoginDto { 
                Username=user.Username,
                Password=user.Password,
            });
            if (laundry != null)
            {
                laundry.Password = user.Password;
                return CreatedAtAction("", new { id = laundry.LaundryId }, laundry);
            }
            else
            {
                throw new ArgumentException("some internal error occured");
            }
        }

        
        
        


    }
}
