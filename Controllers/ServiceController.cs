using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using LaundryApi.Models;
using Microsoft.AspNetCore.Authorization ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;


namespace laundryapi.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceRepository serviceRepository;
        private readonly IMapper mapper;
        public ServiceController(IServiceRepository serviceRepository, IMapper mapper)
        {
            this.serviceRepository = serviceRepository;
            this.mapper = mapper;
        }

        //GET: api/service
        //[httpget]
        //public ActionResult<IEnumerable<ServiceDto>> Index()
        //{
        //    try
        //    {
        //        var laundryservices = serviceRepository.GetAllLaundryServices();
        //        return ok(laundryservices);
        //    }
        //    catch
        //    {
        //        return statuscode(500);
        //    }
        //}


        //GET: api/service/{customerid}
        [HttpGet("{id}")]
        public ActionResult<ServiceDto> GetService(Guid id)
        {
            ServiceDto servicedto = new ServiceDto();
            try
            {
                servicedto = serviceRepository.GetService(id);
                return Ok(servicedto);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest(servicedto);

                //if request got to this point some error occured
                return StatusCode(500);
            }


        }

        //POST: api/service/new
        [HttpPost("new")]
        public async Task<ActionResult<ServiceDto>> Create(ServiceDto serviceDto)
        {
            if (!HttpContext.User.IsInRole(RoleNames.LaundryOwner))
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {

                var userEmail = HttpContext.User.Identity.Name;

                serviceDto = await serviceRepository.CreateServiceAsync(serviceDto, userEmail);
                return CreatedAtAction(nameof(GetService), new { id = serviceDto.Id }, serviceDto);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.ServiceAlreadyExist)
                    return BadRequest(new ResponseDto<ServiceDto>()
                    {
                        message = ErrorMessage.ServiceAlreadyExist
                    });

                //if you got to this point  unforseen happenned
                return StatusCode(500);
            }


        }


        //put: api/service
        [HttpPost]
        public ActionResult UpdateService(ServiceDto servicedto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                serviceRepository.UpdateService(servicedto);
                return StatusCode(200);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return StatusCode(400);

                //if you get to this point something unusual occurred
                return StatusCode(500);
            }

        }

        //delete: api/customer/{serviceid}
        [Route("{serviceid}")]
        [HttpDelete]
        public ActionResult Deleteservice(Guid serviceid)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                serviceRepository.DeleteService(serviceid);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return StatusCode(400);

                //if you get to this point something unusual occured
                return StatusCode(500);
            }

        }
    }
}
