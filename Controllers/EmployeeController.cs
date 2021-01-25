using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using LaundryApi.Models;
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
    public class EmployeeController : Controller
    {
        private readonly IManagerRepository managerRepository;
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeController(IManagerRepository managerRepository,IEmployeeRepository employeeRepository )
        {
            this.managerRepository = managerRepository;
            this.employeeRepository = employeeRepository;

        }

        //GET: api/employee/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(Guid id)
        {
            try
            {
                var employee = await employeeRepository.FindEmployeeAsync(id);
                return employee;
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<EmployeeDto>()
                    {
                        message = ErrorMessage.UserDoesNotExist,
                        statusCode = "400"
                    });

                //if you got to this point an unforseen error occured
                return StatusCode(500);
            }


        }



        //POST: api/employee/new
        [HttpPost("new")]
        public async Task<ActionResult> CreateEmployee([FromBody] NewEmployeeDto newEmployee)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!HttpContext.User.IsInRole(RoleNames.LaundryOwner))
                return Unauthorized(new ResponseDto<EmployeeDto>
                {
                    message = "User is not a laundry owner",
                    statusCode = "401"
                });
            if (newEmployee.Password != newEmployee.ConfirmPassword)
                return BadRequest(new ResponseDto<EmployeeDto>
                {
                    message = "Password do not match",
                    statusCode = "400"
                });

            try
            {
                //Tag employee to laundry owner 
                newEmployee.LaundryId = managerRepository.GetUserByUsername(HttpContext.User.Identity.Name).Id;

                //save new employee to database
                EmployeeDto employeeDto = await managerRepository.CreateEmployeeAsync(newEmployee,HttpContext.User.Identity.Name);
                return CreatedAtAction("GetEmployee", "Employee", new { id = employeeDto.Id }, employeeDto);
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
