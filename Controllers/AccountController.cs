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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private IJwtAuthenticationManager _jwtMananager;
        private IConfiguration _configManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(IUnitOfWork unitOfWork, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _configManager = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

       
        //POST: api/laundry/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            ResponseDto<string> response = new ResponseDto<string>()
            { 
                statusCode = "200",
                message = "login details are correct"
            };

            var result = await _signInManager.PasswordSignInAsync(user.Username, user.Password, false, false);
            if (result.Succeeded)
            {
                string token = _unitOfWork.JwtAuthenticationManager.GetToken(user,RoleNames.LaundryOwner);
                response.data = token;
                return Ok(response);
            }
            else
                return StatusCode(500, result.ToString());

        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserLoginDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _userManager.CreateAsync(new ApplicationUser() { UserName=user.Username,Email=user.Username}, user.Password);
            var passwordHasher = _userManager.PasswordHasher;
            if (result.Succeeded)
            {
                return Ok();
            }
            else
                return StatusCode(500, result.ToString());

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
                await _unitOfWork.ManagerRepository.SendPasswordReset(dto);
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
            if (!_unitOfWork.ManagerRepository.IsPasswordResetLinkValid( id))
                return BadRequest(new ResponseDto<ForgotPasswordDto>()
                {
                    statusCode="400",
                    message = "reset link in invalid"
                });

            //reset password
            try
            {
                _unitOfWork.ManagerRepository.ResetPassword(dto, id);
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
