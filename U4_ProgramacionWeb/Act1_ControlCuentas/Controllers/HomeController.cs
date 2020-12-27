using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Act1_ControlCuentas.Controllers
{
    
    public class HomeController : Controller
    {
        [Authorize(Roles ="Seguidor")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IniciarSesion()
        {
            return View();
        }
        [HttpPost]
        public IActionResult IniciarSesion( string username, string password)
        {
            return View();
        }

    }
}
