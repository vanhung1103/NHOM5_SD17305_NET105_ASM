﻿using Microsoft.AspNetCore.Mvc;

namespace NHOM5_NET105_SD17305.Views.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}