using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using LaundryApi.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LaundryApi.Infrastructure.HelperMethods;
using LaundryApi.Models;
using LaundryApi.Infrastructure;

namespace LaundryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IManagerRepository managerRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IRepositoryHelper repositoryHelper;

        public EmployeeController(IManagerRepository managerRepository, IEmployeeRepository employeeRepository, IRepositoryHelper repositoryHelper)
        {
            this.managerRepository = managerRepository;
            this.employeeRepository = employeeRepository;
            this.repositoryHelper = repositoryHelper;
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
                //add the laundry Id to the employeeDto 
                newEmployee.LaundryId =repositoryHelper.GetLaundryByUsername(HttpContext.User.Identity.Name).Id;

                //save new employee to database
                EmployeeDto employeeDto = await employeeRepository.CreateEmployeeAsync(newEmployee);

                //return response
                return CreatedAtAction(nameof(GetEmployee), new { id = employeeDto.Id }, employeeDto);
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UsernameAlreadyExist)
                    return BadRequest(new ResponseDto<EmployeeDto>()
                    {
                        message = ErrorMessage.UsernameAlreadyExist,
                        statusCode = "400"
                    });

                //if you got this pointan unforseen error occurred
                return StatusCode(500);

            }
        }

        //DELETE: api/employee/delete/{employeeId}
        [HttpDelete("delete/{employeeId}")]
        public async Task<ActionResult> DeleteEmployee (Guid employeeId)
        {
            try
            {
                if (HttpContext.GetUserRole() != RoleNames.LaundryOwner)
                    return Unauthorized(new ResponseDto<string>() { message = "user must be a laundry owner", statusCode = "401" });

                await employeeRepository.DeleteEmployee(employeeId, HttpContext.User.Identity.Name);
                return Ok(new ResponseDto<string>() { message = "employee was successfully deleted.", statusCode = "200" });
            }
            catch(Exception e)
            {
                if (e.Message == ErrorMessage.EmployeeNotOwnedByUser)
                    return Unauthorized(new ResponseDto<string>() { message = ErrorMessage.EmployeeNotOwnedByUser, statusCode = "401" });
                else if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<string>() { message="Employee does not exist",statusCode="400"});

                return StatusCode(500);

            }
            
        }


        //PATCH: api/employee
        [HttpPatch("update")]
        public async Task<ActionResult> UpdateEmployee ([FromBody] EmployeeDto employee)
        {
            try
            {
                if (HttpContext.GetUserRole() != RoleNames.LaundryOwner)
                    return Unauthorized(new ResponseDto<string>() { message = "user must be a laundry owner", statusCode = "401" });

                var data=await employeeRepository.UpdateEmployee(employee, HttpContext.User.Identity.Name);
                return Ok(new ResponseDto<EmployeeDto>() { message = "employee was successfully update.", statusCode = "200",data=data });
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.EmployeeNotOwnedByUser)
                    return Unauthorized(new ResponseDto<string>() { message = ErrorMessage.EmployeeNotOwnedByUser, statusCode = "401" });
                else if (e.Message == ErrorMessage.UserDoesNotExist)
                    return BadRequest(new ResponseDto<string>() { message = "Employee does not exist", statusCode = "400" });

                return StatusCode(500);

            }
        }
    }
}
