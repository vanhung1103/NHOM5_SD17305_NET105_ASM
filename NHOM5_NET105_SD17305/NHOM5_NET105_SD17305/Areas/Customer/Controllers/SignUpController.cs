﻿using Microsoft.AspNetCore.Mvc;

namespace NHOM5_NET105_SD17305.Views.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SignUpController : Controller
    {
        public IActionResult SignUp()
        {
            return View();
        }
    }
}