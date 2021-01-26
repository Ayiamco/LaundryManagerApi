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
    public class AccountController : ControllerBase
    {

        private readonly IManagerRepository managerRepository;
        private readonly IJwtAuthenticationManager jwtManager;

        public AccountController(IManagerRepository managerRepository, IJwtAuthenticationManager jwtManager)
        {
            this.managerRepository = managerRepository;
            this.jwtManager = jwtManager;

        }

        //POST: api/laundry/login
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] UserLoginDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = new ResponseDto<string>() {  };
            try
            {
                //get login resp
                LoginResponseDto resp;
                if (string.IsNullOrWhiteSpace(user.Role)) 
                    resp = managerRepository.GetLoginResponse(user.Username, user.Password);
                else
                    resp = managerRepository.GetLoginResponse(user.Username, user.Password,user.Role);

                //get jwt token
                string token = jwtManager.GetToken(user,resp.UserRole);

                //create response body
                response.statusCode = "200";
                response.message = "login details are correct";
                response.data = token;
                if (resp.UserRole==RoleNames.LaundryOwner)
                    response.role = RoleNames.LaundryOwner;
                else if(resp.UserRole==RoleNames.LaundryEmployee)
                    response.role = RoleNames.LaundryEmployee;
                else
                    response.role = RoleNames.Admin;

                return Ok(response);
            }
            catch(Exception e)
            {

                response.statusCode = "400";
                if (e.Message == ErrorMessage.InCorrectPassword)
                {
                    response.message = ErrorMessage.InCorrectPassword;
                    return BadRequest(response);
                }
                else if (e.Message == ErrorMessage.UserDoesNotExist)
                {
                    response.message = ErrorMessage.UserDoesNotExist;
                    return BadRequest(response);
                }
                else if (e.Message == ErrorMessage.UserHasTwoRoles)
                {
                    response.message = ErrorMessage.UserHasTwoRoles;
                    return BadRequest(response);
                }
                 

                //if you get to this point something unforseen occured
                return StatusCode(500);
            }

        }


        //POST: api/account/forgotpassword
        [Route("forgotpassword")]
        [HttpPost]
        public async Task<ActionResult<string>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                //send passwoord reset link to email
                await managerRepository.SendPasswordReset(dto.Username);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<ForgotPasswordDto>()
                    {
                        message = ErrorMessage.UserDoesNotExist,
                        statusCode="400"
                    }) ;

                //if you get to this point something unforseen happened
                return StatusCode(500);
            }
            

            //return response
            return Ok();
        }

       
        //POST: api/account/forgotpassword/{id}
        [Route("forgotpassword/{id}")]
        [HttpPost]
        public ActionResult PasswordReset([FromBody] ForgotPasswordDto dto,string id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            //check if id matches username
            if (!managerRepository.IsPasswordResetLinkValid(dto.Username, id))
                return BadRequest(new ResponseDto<ForgotPasswordDto>()
                {
                    statusCode="400",
                    message = "reset link in invalid"
                });

            //reset password
            try
            {
                managerRepository.ResetPassword(dto, id);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.LinkExpired)
                {
                    return BadRequest(new ResponseDto<ForgotPasswordDto>()
                    {
                        message = ErrorMessage.LinkExpired,
                        statusCode = "400"
                    });
                }
                    
            }
            return Ok();
        } 


        

       
    }
}
