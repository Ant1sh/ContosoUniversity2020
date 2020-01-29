using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity2020.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{ErrorCode}")]
        public IActionResult Error(int ErrorCode)
        {
            return View(ErrorCode);//return the view passing the status code (404 or 500)
        }
    }
}