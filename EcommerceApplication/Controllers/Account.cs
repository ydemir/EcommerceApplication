﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EcommerceApplication.Models;
using EcommerceApplication.ViewModels.Account;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceApplication.Controllers
{
    public class Account : Controller
    {
        private readonly UserManager<Customer> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<Customer> _signInManager;
        public Account(UserManager<Customer> userManager,RoleManager<ApplicationRole> roleManager, SignInManager<Customer> signManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Register Settings

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    UserName = registerVM.UserName,
                    Email = registerVM.Email,
                };
                var result = await _userManager.CreateAsync(customer, registerVM.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("SiteUser").Result)
                    {
                        ApplicationRole role = new ApplicationRole();
                        role.Name = "SiteUser";

                        IdentityResult roleResult = _roleManager.CreateAsync(role).Result;

                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Somethings went wrong !");
                            return View(registerVM);
                        }
                    }
                    _userManager.AddToRoleAsync(customer, "SiteUser").Wait();
                    await _signInManager.SignInAsync(customer, isPersistent: false);
                    return RedirectToAction("Login", "Account");

                }
            }
            return View(registerVM);
        }
        #endregion



    }
}
