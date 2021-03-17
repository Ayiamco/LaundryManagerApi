using LaundryApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Controllers
{
    public class DashboardController : Controller
    {
        private IDashboardRepo repo;
        public DashboardController(IDashboardRepo repo)
        {
            this.repo = repo;
        }
        public IActionResult Index()
        {
            var username=HttpContext.User.Identity.Name;
            return View();
        }
    }
}
