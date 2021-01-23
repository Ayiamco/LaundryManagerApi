using LaundryApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpPost("new")]
        public IActionResult CreateEmployee([FromBody] NewEmployeeDto newEmployee)
        {
            return RedirectToAction("RegisterEmployee","Account", new { newEmployee});
        }
    }
}
