using LaundryApi.Dtos;
using LaundryApi.Infrastructure;
using LaundryApi.Interfaces;
using LaundryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {

        private readonly ILaundryRepository laundryRepository;
        private readonly IJwtAuthenticationManager jwtManager;

        public AccountController(ILaundryRepository laundryRepository, IJwtAuthenticationManager jwtManager)
        {
            this.laundryRepository = laundryRepository;
            this.jwtManager = jwtManager;

        }

        ////POST: api/laundry/login
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] UserLoginDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                //get password hash
                string hashedPassword = HashPassword(user.Password);

                var _user = laundryRepository.GetUserByUsername(user.Username);
                var response = new ResponseDto<string>()
                {
                    statusCode = "400",
                    message = "User does not exist",
                };
                if (_user == null)
                    return BadRequest(response);

                if (_user.PasswordHash != hashedPassword)
                {
                    response.message = "password is incorrect";
                    return BadRequest(response);
                }

                //get jwt token
                string token = jwtManager.GetToken(user,_user.UserRole);

                //create response body
                response.statusCode = "200";
                response.message = "login details are correct";
                response.data = token;
                if (_user.UserRole==RoleNames.LaundryOwner)
                    response.role = RoleNames.LaundryOwner;
                else if(_user.UserRole==RoleNames.LaundryEmployee)
                    response.role = RoleNames.LaundryEmployee;
                else
                    response.role = RoleNames.Admin;

                return Ok(response);
            }
            catch
            {
                return StatusCode(500);
            }

        }

        [AllowAnonymous]
        //POST: api/account/register
        [HttpPost("register")]
        public async Task<ActionResult<LaundryDto>> RegisterLaundry([FromBody] NewLaundryDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (user.Password != user.ConfirmPassword)
                return BadRequest(new { message = "Password do not match" });

            try
            {
                //save new laundry to database
                LaundryDto laundryDto = await laundryRepository.CreateAsync(user);
                return CreatedAtAction("GetLaundry", "Laundry", new { id = laundryDto.Id }, laundryDto);
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.UsernameAlreadyExist)
                    return BadRequest(new ResponseDto<ApplicationUserDto>() {
                        message=ErrorMessage.UsernameAlreadyExist,
                        statusCode="400"
                    });
                
                //if you got this pointan unforseen error occurred
                return StatusCode(500);

            }
        }


        

       
    }
}
