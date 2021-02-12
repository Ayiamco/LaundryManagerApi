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
        private IUnitOfWork unitOfWork;

        public LaundryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork; 
        }

        //GET: api/laundry/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LaundryDto>> GetLaundry(Guid id)
        {
            try
            {
                var user = await unitOfWork.LaundryRepository.FindLaundryAsync(id);
                return user;
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<LaundryDto>() {
                        message = ErrorMessage.UserDoesNotExist,
                        statusCode = "400"
                    });

                //if you got to this point an unforseen error occured
                return StatusCode(500);
            }


        }

        //Tested
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
                LaundryDto laundryDto = await unitOfWork.LaundryRepository.CreateLaundryAsync(user);
                return CreatedAtAction("GetLaundry", "Laundry", new { id = laundryDto.Id }, new ResponseDto<LaundryDto>() {data=laundryDto,statusCode="201" });
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UsernameAlreadyExist)
                    return BadRequest(new ResponseDto<LaundryDto>()
                    {
                        message = ErrorMessage.UsernameAlreadyExist,
                        statusCode = "400"
                    });
                return StatusCode(500,new ResponseDto<string>() { statusCode="500",message= "server error" });

            }
        }

        //DELETE: api/laundry/{laundryId}
        [HttpDelete("delete/{laundryId}")]
        public async Task<ActionResult> DeleteLaundry(Guid laundryId)
        {
            try
            {
                await unitOfWork.LaundryRepository.DeleteLaundry(laundryId);
                return Ok(new ResponseDto<LaundryDto>() { message = "laundry deleted successfully.", statusCode = "200" });
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<LaundryDto>() { message = ErrorMessage.UserDoesNotExist, statusCode = "400" });

                return StatusCode(500);
            }
            
        }

        //PATCH: api/laundry
        [HttpPatch("update")]
        public async Task<ActionResult> UpdateLaundry([FromBody]LaundryDto laundry)
        {
            try
            {
                var data=await unitOfWork.LaundryRepository.UpdateLaundry(laundry);
                return Ok(new ResponseDto<LaundryDto>() { message = "laundry updated successfully.", statusCode = "200",data=data });
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<LaundryDto>() { message = ErrorMessage.UserDoesNotExist, statusCode = "400" });

                return StatusCode(500);
            }
            
        }







    }
}
