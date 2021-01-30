using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Infrastructure;
using LaundryApi.Entites;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using  LaundryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LaundryController : Controller
    {
        private readonly ILaundryRepository laundryRepository;
        private readonly IJwtAuthenticationManager jwtManager;
        private readonly IManagerRepository managerRepository;


        public LaundryController(ILaundryRepository laundryRepository, IJwtAuthenticationManager jwtManager, IManagerRepository managerRepository)
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
                        message = ErrorMessage.UserDoesNotExist,
                        status = "400"
                    });

                //if you got to this point an unforseen error occured
                return StatusCode(500);
            }


        }

        //POST: api/laundry/new
        [HttpPost("new")]
        [AllowAnonymous]
        public async Task<ActionResult<LaundryDto>> RegisterLaundry([FromBody] NewLaundryDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            
            try
            {
                //save new laundry to database
                LaundryDto laundryDto = await laundryRepository.CreateLaundryAsync(user);
                return CreatedAtAction("GetLaundry", "Laundry", new { id = laundryDto.Id }, new ResponseDto<LaundryDto>() {data=laundryDto,status="201" });
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UsernameAlreadyExist)
                    return BadRequest(new ResponseDto<LaundryDto>()
                    {
                        message = ErrorMessage.UsernameAlreadyExist,
                        status = "400"
                    });

                //if you got this pointan unforseen error occurred
                return StatusCode(500);

            }
        }

        //DELETE: api/laundry/{laundryId}
        [HttpDelete("delete/{laundryId}")]
        public async Task<ActionResult> DeleteLaundry(Guid laundryId)
        {
            try
            {
                await laundryRepository.DeleteLaundry(laundryId);
                return Ok(new ResponseDto<LaundryDto>() { message = "laundry deleted successfully.", status = "200" });
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<LaundryDto>() { message = ErrorMessage.UserDoesNotExist, status = "400" });

                return StatusCode(500);
            }
            
        }

        //PATCH: api/laundry
        [HttpPatch("update")]
        public async Task<ActionResult> UpdateLaundry([FromBody]LaundryDto laundry)
        {
            try
            {
                var data=await laundryRepository.UpdateLaundry(laundry);
                return Ok(new ResponseDto<LaundryDto>() { message = "laundry updated successfully.", status = "200",data=data });
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<LaundryDto>() { message = ErrorMessage.UserDoesNotExist, status = "400" });

                return StatusCode(500);
            }
            
        }







    }
}
