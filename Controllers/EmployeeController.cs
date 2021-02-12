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

        //POST: api/employee/add
        [Authorize(Roles ="LaundryOwner")]
        [HttpPost("add")]
        public async Task<ActionResult<ResponseDto<string>>> AddEmployee([FromBody] ForgotPasswordDto dto)
        {

            string employeeEmail = dto.Username;
            if (HttpContext.User.Identity.Name == employeeEmail)
                return BadRequest(new ResponseDto<string>() { message="employee cannot have same mail as employer",statusCode="400" });

            Task<bool> resp = employeeRepository.SendEmployeeRegistrationLink(employeeEmail, HttpContext.User.Identity.Name);
            if (resp.Result == false)
                return StatusCode(500, new ResponseDto<string>() { statusCode = "500", message = "server error" });

            return Ok(new ResponseDto<string>() { statusCode = "200", message = "registration link is been sent" });
        }
        
        [AllowAnonymous]
        //POST: api/employee/new
        [HttpPost("new")]
        public async Task<ActionResult> CreateEmployee([FromBody] NewEmployeeDto newEmployee)
        {
            ResponseDto<EmployeeDto> response = new ResponseDto<EmployeeDto>() { statusCode = "400" };
            if (!ModelState.IsValid)
                return BadRequest(response);
            
            if (newEmployee.Password != newEmployee.ConfirmPassword)
            {
                response.message = "passwords do not match";
                return BadRequest(response);
            }
                
            if (!employeeRepository.IsEmployeeInTransit(newEmployee.Username,newEmployee.LaundryId))
            {
                response.message = "Unauthorized Registration please contact your employer";
                response.statusCode = "401";
                return Unauthorized(response);
            }

            try
            {
                EmployeeDto employeeDto = await employeeRepository.CreateEmployeeAsync(newEmployee);
                return StatusCode(201,new ResponseDto<EmployeeDto>() { statusCode = "201", data = employeeDto });
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.UsernameAlreadyExist)
                {
                    response.message = ErrorMessage.UsernameAlreadyExist;
                    return BadRequest(response);
                }

                response.statusCode = "500";
                return StatusCode(500,response);

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

        [HttpGet]
        public ActionResult<ResponseDto<IEnumerable<EmployeeDtoPartial>>> GetLaundryEmployees()
        {
            try
            {
                
                if (!HttpContext.User.IsInRole(RoleNames.LaundryOwner))
                    return Unauthorized(new ResponseDto<IEnumerable<EmployeeDto>>() { message = ErrorMessage.OnlyLaundryOwnerAllowed });
                var queryParam = Request.Query;
                var pageNumber = int.Parse(queryParam["page"]);
                var employees = employeeRepository.GetPage(2,pageNumber);
                return Ok(new ResponseDto<PagedList<EmployeeDtoPartial>>() { statusCode = "200", data = employees });
            }
            catch (Exception e)
            {
                if (e.Message == ErrorMessage.NoEntityMatchesSearch)
                    return NoContent();

                return StatusCode(500);
            }
            
        }
    }
}
