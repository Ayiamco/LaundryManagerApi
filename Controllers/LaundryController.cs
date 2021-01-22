using Microsoft.AspNetCore.Http;
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


        public LaundryController(ILaundryRepository laundryRepository, IJwtAuthenticationManager jwtManager)
        {
            this.laundryRepository = laundryRepository;
            this.jwtManager = jwtManager;

        }

        //GET: api/laundry/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LaundryDto>> GetLaundry(Guid id)
        {
            try
            {
                var user = await laundryRepository.FindAsync(id);
                return user;
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EntityDoesNotExist)
                    return BadRequest();

                //if you got to this point an unforseen error occured
                return StatusCode(500);
            }


        }

        




    }
}
