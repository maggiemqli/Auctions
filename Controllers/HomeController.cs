using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auctions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace Auctions.Controllers
{
    public class HomeController : Controller
    {

        private AuctionsContext _dbContext;
        public HomeController(AuctionsContext context)
        {
            _dbContext = context;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult RegisterUser(RegisterViewModel model)
        {
            PasswordHasher<RegisterViewModel> hasher = new PasswordHasher<RegisterViewModel>();
            if(_dbContext.users.Where(u => u.username == model.username).SingleOrDefault() != null)
            { ModelState.AddModelError("username", "Username is taken."); }

            if(ModelState.IsValid)
            {
                User newUser = new User()
                {
                    username = model.username,
                    first_name = model.first_name,
                    last_name = model.last_name,
                    password = hasher.HashPassword(model, model.password),
                    wallet_balance = 1000                        
                };
                User theUser = _dbContext.Add(newUser).Entity;
                _dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", theUser.user_id);
                return RedirectToAction("Index", "Dashboard");
            }
            return View("Index");
        }


    [HttpPost("login")]
        public IActionResult LoginUser(LoginViewModel model)
        {
            User user = _dbContext.users.SingleOrDefault(u => u.username == model.LogUsername);
            PasswordHasher<LoginViewModel> Hasher = new PasswordHasher<LoginViewModel>();

            if(user == null)
            {
                ModelState.AddModelError("LogUsername", "LogUsername does not exist.");
            }
            else if(0 == Hasher.VerifyHashedPassword(model, user.password, model.LogPassword))
            { 
                ModelState.AddModelError("LogPassword", "Password is invalid.");
            }
            if(ModelState.IsValid)
            {
                HttpContext.Session.SetInt32("UserId", user.user_id);
                return RedirectToAction("Index","Dashboard");
            }
            return View("Index");
        }


        [HttpGet("success")]
        public IActionResult Success()
        {
            return View("Success");
        } 

        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
