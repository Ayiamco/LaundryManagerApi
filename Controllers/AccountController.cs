using LaundryApi.Dtos;
using LaundryApi.Infrastructure;
using LaundryApi.Interfaces;
using LaundryApi.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Models;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IUnitOfWork unitOfWork;
      
        public AccountController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //Tested
        //POST: api/laundry/login
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] UserLoginDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            ResponseDto<string> response = new ResponseDto<string>()
            { 
                statusCode = "200",
                message = "login details are correct"
            };
            try
            {
                //get login resp
                LoginResponseDto resp;
                if (string.IsNullOrWhiteSpace(user.Role)) 
                    resp = unitOfWork.ManagerRepository.GetLoginResponse(user.Username, user.Password);
                else
                    resp = unitOfWork.ManagerRepository.GetLoginResponse(user.Username, user.Password,user.Role);

                string token = unitOfWork.JwtAuthenticationManager.GetToken(user,resp.UserRole);
                response.data = token;
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
                else if (e.Message == ErrorMessage.PasswordChanged)
                {
                    response.message = ErrorMessage.PasswordChanged;
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

                else
                    return StatusCode(500);
            }

        }

        //Tested
        //POST: api/account/forgotpassword
        [Route("forgotpassword")]
        [HttpPost]
        public async Task<ActionResult<string>> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                //send passwoord reset link to email
                await unitOfWork.ManagerRepository.SendPasswordReset(dto);
                return Ok(new ResponseDto<string>() { statusCode="200",message=$"password reset link has being sent to {dto.Username}"});
            }
            catch (Exception e)
            {
                ResponseDto<string> response = new ResponseDto<string>() { statusCode = "400" };
                if (e.Message == ErrorMessage.UserDoesNotExist)
                {
                    response.message = "Your search did not return any result please retry with a registered email.";
                    return BadRequest(response);
                }
                else if (e.Message == ErrorMessage.UserHasTwoRoles)
                {
                    response.message = ErrorMessage.UserHasTwoRoles;
                    return BadRequest(response);
                }

                else
                    return StatusCode(500); ;
            }
        }

       
        //POST: api/account/forgotpassword/{id}
        [HttpPost("forgotpassword/{id}")]
        public ActionResult PasswordReset([FromBody] ForgotPasswordDto dto,string id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            //check if id matches username
            if (!unitOfWork.ManagerRepository.IsPasswordResetLinkValid( id))
                return BadRequest(new ResponseDto<ForgotPasswordDto>()
                {
                    statusCode="400",
                    message = "reset link in invalid"
                });

            //reset password
            try
            {
                unitOfWork.ManagerRepository.ResetPassword(dto, id);
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
