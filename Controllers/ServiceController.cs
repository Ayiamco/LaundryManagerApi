using AutoMapper;
using LaundryApi.Dtos;
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
    public class ServiceController : ControllerBase
    {
        private readonly  IServiceRepository serviceRepository;
        private readonly IMapper mapper;
        public ServiceController(IServiceRepository serviceRepository, IMapper mapper)
        {
            this.serviceRepository = serviceRepository;
            this.mapper = mapper;
        }

        //GET: api/service
        [HttpGet]
        public ActionResult<IEnumerable<ServiceDto>> Index()
        {
            try
            {
                var laundryServices=serviceRepository.GetAllLaundryServices();
                return Ok(laundryServices);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //GET: api/service/{customerId}
        [HttpGet("{id}")]
        public  ActionResult<ServiceDto> GetService(Guid id)
        {
            ServiceDto serviceDto = new ServiceDto();
            try
            {

                serviceDto = serviceRepository.GetService(id);
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
            if (! HttpContext.User.IsInRole(RoleNames.LaundryOwner))
                return Unauthorized();
            
            if (!ModelState.IsValid)
                return BadRequest();
            
            try
            {

                var userEmail = HttpContext.User.Identity.Name;
                
                serviceDto = await serviceRepository.CreateServiceAsync(serviceDto,userEmail);
                return CreatedAtAction(nameof(GetService), new { id = serviceDto.Id }, serviceDto);
            }
            catch
            {
                return StatusCode(500);
            }
            

        }


        //PUT: api/customer
        [HttpPut]
        public ActionResult UpdateService(ServiceDto serviceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                serviceRepository.UpdateService(serviceDto);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return StatusCode(400);
                
                //if you get to this point something unusual occurred
                return StatusCode(500);
            }
            
        }

        //DELETE: api/customer/{serviceId}
        [Route("{serviceId}")]
        [HttpDelete]
        public ActionResult DeleteService(Guid serviceId)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                serviceRepository.DeleteService(serviceId);
                return StatusCode(204);
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return StatusCode(400);

                //if you get to this point something unusual occured
                return StatusCode(500);
            }
           
        }
    }
}
