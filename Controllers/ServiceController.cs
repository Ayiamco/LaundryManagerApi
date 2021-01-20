using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
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
    public class ServiceController : ControllerBase
    {
        private IServiceRepository repository;
        private IMapper mapper;
        public ServiceController(IServiceRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }


        [HttpGet("{id}")]
        public  ActionResult<ServiceDto> GetService(Guid id)
        {
            ServiceDto serviceDto = new ServiceDto();
            try
            {
                serviceDto = repository.GetService(id);
                return Ok(serviceDto);
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest(serviceDto);

                //if request got to this point some error occured
                return StatusCode(500);
            }
           
            
        }

        //POST: api/service/new
        [HttpPost("new")]
        public async Task<ActionResult<ServiceDto>> Create(ServiceDto serviceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                serviceDto = await repository.AddService(serviceDto);
                return CreatedAtAction("", new { id = serviceDto.Id }, serviceDto);
            }
            catch
            {
                return StatusCode(500);
            }
            

        }
    }
}
