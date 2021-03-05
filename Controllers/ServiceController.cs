using AutoMapper;
using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using LaundryApi.Entites;
using Microsoft.AspNetCore.Authorization ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Models;
using LaundryApi.Infrastructure;

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
        [HttpGet]
        public ActionResult GetLaundryServices()
        {
            var queryParam = Request.Query;
            var pageNumber = int.Parse(queryParam["page"]);
            var searchParam = Convert.ToString(queryParam["name"]);
            var userRole = HttpContext.GetUserRole();
            var services=serviceRepository.GetPage(6, HttpContext.User.Identity.Name,userRole,pageNumber, searchParam);
            return Ok(new ResponseDto<PagedList<ServiceDto>>() { statusCode="200", data=services});
        }


        //GET: api/service/{customerid}
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDto>> GetService(Guid id)
        {
            ServiceDto servicedto = new ServiceDto();
            try
            {
                servicedto = await serviceRepository.GetService(id);
                return Ok( new ResponseDto<ServiceDto>() { statusCode = "200", data = servicedto });
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest(servicedto);

                //if request got to this point some error occured
                return StatusCode(500,new ResponseDto<string>() { statusCode="500",message="server error"});
            }


        }

        //POST: api/service/new
        [HttpPost("new")]
        public async Task<ActionResult<ServiceDto>> Create(ServiceDto serviceDto)
        {
            if (!HttpContext.User.IsInRole(RoleNames.LaundryOwner) )
                return Unauthorized(new ResponseDto<string>() { statusCode="401", message="unauthorized access"});
            

            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto<string>() { statusCode="400", message="some feilds are invalid"});

            try
            {
                var userEmail = HttpContext.User.Identity.Name;
                serviceDto = await serviceRepository.CreateServiceAsync(serviceDto, userEmail);
                return CreatedAtAction(nameof(GetService), new { id = serviceDto.Id }, 
                    new ResponseDto<ServiceDto>() { statusCode="201", data=serviceDto});
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.ServiceAlreadyExist)
                    return BadRequest(new ResponseDto<ServiceDto>()
                    {
                        statusCode="400",
                        message = ErrorMessage.ServiceAlreadyExist
                    });

                //if you got to this point  unforseen happenned
                return StatusCode(500,new ResponseDto<string>() { statusCode="500"});
            }


        }


        //PATCH: api/service
        [HttpPatch]
        public async Task<ActionResult<ServiceDto>> UpdateService(ServiceDto servicedto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try 
            {
                var service=await serviceRepository.UpdateService(servicedto);
                var resp = new ResponseDto<ServiceDto>() { data = service, message = "updated suceessfully", statusCode = "200" };
                return Ok(resp);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest((new ResponseDto<ServiceDto>() { message = ErrorMessage.EntityDoesNotExist }));

                //if you get to this point something unusual occurred
                return StatusCode(500);
            }

        }

        //DELETE: api/service/{serviceid}
        [HttpDelete("{serviceid}")]
        public async Task<ActionResult> Deleteservice(Guid serviceid)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseDto<ServiceDto>() { message="service Id must be Guid"});
            try
            {
                await serviceRepository.DeleteService(serviceid);
                return Ok(new ResponseDto<string>() { statusCode="200"});
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest(new ResponseDto<ServiceDto>() { statusCode="400",message=ErrorMessage.EntityDoesNotExist});

                //if you get to this point something unusual occured
                return StatusCode(500, new ResponseDto<string>() { statusCode = "500" });
            }

        }

        //GET: api/service/all
        [HttpGet("all")]
        public ActionResult GetAllServices()
        {
            var userRole=HttpContext.GetUserRole();
            if(userRole==RoleNames.LaundryEmployee || userRole==RoleNames.LaundryOwner)
            {
                var services=serviceRepository.GetAllServices(HttpContext.User.Identity.Name, userRole);
                return Ok(new ResponseDto<IEnumerable<ServiceDto>>() { statusCode="200",data=services });
            }
            else
                return Unauthorized(new ResponseDto<string>() { statusCode="401",message="unauthorized access"});
        }

        

    }
}
