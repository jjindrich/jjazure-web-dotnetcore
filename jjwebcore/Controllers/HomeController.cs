using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using jjwebcore.Models;
using System.Net;

namespace jjwebcore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Test()
        {
            ViewData["Message"] = "Test page.";

            System.IO.File.WriteAllText("/home/test.txt", "Test");

            var host = Dns.GetHostName();            
            ViewData["Host"] = host;

            System.IO.File.WriteAllText(string.Format("/home/host_{0}.txt", host), host);

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
