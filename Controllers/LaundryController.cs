﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Infrastructure;
using LaundryApi.Models;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using static LaundryApi.Infrastructure.HelperMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class LaundryController : Controller
    {
        private readonly ILaundryRepository laundryRepository;
        private readonly IJwtAuthenticationManager jwtManager;
        private readonly IManagerRepository managerRepository;


        public LaundryController(ILaundryRepository laundryRepository, IJwtAuthenticationManager jwtManager,IManagerRepository managerRepository)
        {
            this.laundryRepository = laundryRepository;
            this.jwtManager = jwtManager;
            this.managerRepository = managerRepository;

        }

        //GET: api/laundry/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LaundryDto>> GetLaundry(Guid id)
        {
            try
            {
                var user = await laundryRepository.FindLaundryAsync(id);
                return user;
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<LaundryDto>() { 
                        message=ErrorMessage.UserDoesNotExist,
                        statusCode="400"
                    });
                
                //if you got to this point an unforseen error occured
                return StatusCode(500);
            }


        }

        //POST: api/laundry/register
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
                LaundryDto laundryDto = await managerRepository.CreateLaundryAsync(user);
                return CreatedAtAction("GetLaundry", "Laundry", new { id = laundryDto.Id }, laundryDto);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UsernameAlreadyExist)
                    return BadRequest(new ResponseDto<ApplicationUserDto>()
                    {
                        message = ErrorMessage.UsernameAlreadyExist,
                        statusCode = "400"
                    });

                //if you got this pointan unforseen error occurred
                return StatusCode(500);

            }
        }







    }
}
