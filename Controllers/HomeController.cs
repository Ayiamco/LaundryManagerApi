using LaundryApi.Dtos;
using LaundryApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LaundryApi.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private IDashboardRepo repo;
        public HomeController(IDashboardRepo repo)
        {
            this.repo = repo;
        }
        // GET: api/<HomeController>
        [HttpGet]
        public ActionResult Index()
        {
            var username = HttpContext.User.Identity.Name;
            var data = repo.GetDashboardData(username);
            return Ok(new ResponseDto<DashboardDto> { data = data, statusCode = "200" });

        }

        
    }
}
