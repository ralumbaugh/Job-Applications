using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobApplications.Models;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System;

namespace JobApplications.Controllers
{
    public class HomeController : Controller
    {

        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID!=null)
            {
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

		[HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            UserWrapper Wrapper = new UserWrapper();
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(u =>u.UserId == (int)LoggedInUserID);
            Wrapper.AllCompanies = dbContext.Companies.Include(u => u.User).ToList();
            return View("Dashboard", Wrapper);
        }
		[HttpGet("NewCompany")]
        public IActionResult NewCompany()
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            UserWrapper Wrapper = new UserWrapper();
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(u =>u.UserId == (int)LoggedInUserID);
            return View("NewCompany", Wrapper);
        }
		[HttpPost("CreateCompany")]
        public IActionResult CreateCompany(UserWrapper WrappedCompany)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            WrappedCompany.CurrentCompany.UserId=(int)LoggedInUserID;
            if(ModelState.IsValid)
            {
                dbContext.Companies.Add(WrappedCompany.CurrentCompany);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return NewCompany();
        }
		[HttpGet("company/{companyid}")]
        public IActionResult IndividualCompany(int companyid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            Company CurrentCompany = dbContext.Companies.FirstOrDefault(c => c.CompanyId == companyid);
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            else if(CurrentCompany == null || CurrentCompany.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            UserWrapper Wrapper = new UserWrapper();
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(u =>u.UserId == (int)LoggedInUserID);
            Wrapper.CurrentCompany = CurrentCompany;
            return View("IndividualCompany", Wrapper);
        }
		[HttpGet("{companyid}/NewPosition")]
        public IActionResult NewPosition(int companyid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Company CurrentCompany = dbContext.Companies.FirstOrDefault(c => c.CompanyId == companyid);
            if(CurrentCompany == null || CurrentCompany.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            UserWrapper Wrapper = new UserWrapper();
            Wrapper.CurrentCompany = CurrentCompany;
            return View("NewPosition", Wrapper);
        }
		[HttpPost("CreatePosition")]
        public IActionResult CreatePosition(UserWrapper WrappedPosition)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Company CurrentCompany = dbContext.Companies.FirstOrDefault(c => c.CompanyId == WrappedPosition.CurrentPosition.CompanyId);
            if(CurrentCompany == null || CurrentCompany.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            if(ModelState.IsValid)
            {
                dbContext.Positions.Add(WrappedPosition.CurrentPosition);
                dbContext.SaveChanges();
                return IndividualCompany(WrappedPosition.CurrentCompany.CompanyId);
            }
            WrappedPosition.CurrentCompany = CurrentCompany;
            return View("NewPosition", WrappedPosition);
        }
        public IActionResult Login(LoginWrapper WrappedUser)
        {
            LoginUser user = WrappedUser.LoginUser;
            if(ModelState.IsValid)
            {
                User UserInDb = dbContext.Users.FirstOrDefault(u=> u.Email == user.Email);
                if(UserInDb == null)
                {
                    ModelState.AddModelError("LoginUser.Email", "The email/password combination is incorrect.");
                    return View("Index");
                }
                PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                var result = Hasher.VerifyHashedPassword(user, UserInDb.Password, user.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginUser.Email", "The email/password combination is incorrect.");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("LoggedInUserID", UserInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        public IActionResult Register(LoginWrapper WrappedUser)
        {
            User user = WrappedUser.NewUser;
            if(dbContext.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("NewUser.Email", "Email already in use!");
            }
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("LoggedInUserID", user.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
