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
            Wrapper.AllCompanies = dbContext.Companies.Include(u => u.User).Where(u => u.UserId == (int)LoggedInUserID).ToList();
            Wrapper.AllInterviews = dbContext.Interviews.OrderBy(d => d.Date).Include(p => p.Position).ThenInclude(c => c.Company).Where(i => i.Position.Company.UserId == (int)LoggedInUserID && DateTime.Now.CompareTo(i.Date) <= 0).ToList();
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
		[HttpGet("EditCompany/{companyid}")]
        public IActionResult EditCompany(int companyid)
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
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(u =>u.UserId == (int)LoggedInUserID);
            Wrapper.CurrentCompany = CurrentCompany;
            return View("EditCompany", Wrapper);
        }
		[HttpGet("EditPosition/{positionid}")]
        public IActionResult EditPosition(int positionid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Position CurrentPosition = dbContext.Positions.Include(c => c.Company).FirstOrDefault(c => c.PositionId == positionid);
            if(CurrentPosition == null || CurrentPosition.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            UserWrapper Wrapper = new UserWrapper();
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(u =>u.UserId == (int)LoggedInUserID);
            Wrapper.CurrentPosition = CurrentPosition;
            return View("EditPosition", Wrapper);
        }
		[HttpGet("EditInterview/{interviewid}")]
        public IActionResult EditInterview(int interviewid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Interview CurrentInterview = dbContext.Interviews.Include(p => p.Position).ThenInclude(c => c.Company).FirstOrDefault(i => i.InterviewId == interviewid);
            if(CurrentInterview == null || CurrentInterview.Position.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            UserWrapper Wrapper = new UserWrapper();
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(u =>u.UserId == (int)LoggedInUserID);
            Wrapper.CurrentInterview = CurrentInterview;
            return View("EditInterview", Wrapper);
        }
        [HttpPost("ChangeInterview")]
        public IActionResult ChangeInterview(UserWrapper WrappedInterview)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            Interview InterviewToEdit = dbContext.Interviews.Include(p => p.Position).ThenInclude(c => c.Company).FirstOrDefault(i => i.InterviewId == WrappedInterview.CurrentInterview.InterviewId);
            InterviewToEdit.Date = WrappedInterview.CurrentInterview.Date;
            InterviewToEdit.Notes = WrappedInterview.CurrentInterview.Notes;
            WrappedInterview.CurrentInterview = InterviewToEdit;
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            if(WrappedInterview.CurrentInterview.Position.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            if(DateTime.Now.CompareTo(WrappedInterview.CurrentInterview.Date)>0)
            {
                ModelState.AddModelError("CurrentInterview.Date","Please enter a time in the future");
            }
            if(ModelState.IsValid)
            {
                dbContext.Interviews.Update(InterviewToEdit);
                dbContext.SaveChanges();
                return Redirect($"position/{InterviewToEdit.Position.PositionId}");
            }
            return EditInterview(WrappedInterview.CurrentInterview.InterviewId);
        }
        [HttpPost("ChangePosition")]
        public IActionResult ChangePosition(UserWrapper WrappedPosition)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            Position PositionToEdit = dbContext.Positions.Include(c => c.Company).FirstOrDefault(p => p.PositionId == WrappedPosition.CurrentPosition.PositionId);
            PositionToEdit.Name = WrappedPosition.CurrentPosition.Name;
            PositionToEdit.Description = WrappedPosition.CurrentPosition.Description;
            PositionToEdit.Requirements = WrappedPosition.CurrentPosition.Requirements;
            PositionToEdit.Link = WrappedPosition.CurrentPosition.Link;
            PositionToEdit.Notes = WrappedPosition.CurrentPosition.Notes;
            WrappedPosition.CurrentPosition = PositionToEdit;
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            if(WrappedPosition.CurrentPosition.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            if(ModelState.IsValid)
            {
                dbContext.Positions.Update(PositionToEdit);
                dbContext.SaveChanges();
                return Redirect($"position/{PositionToEdit.PositionId}");
            }
            return EditPosition(WrappedPosition.CurrentPosition.PositionId);
        }
		[HttpGet("EditContact/{contactid}")]
        public IActionResult EditContact(int contactid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Contact CurrentContact = dbContext.Contacts.Include(c => c.Company).FirstOrDefault(c => c.ContactId == contactid);
            if(CurrentContact == null || CurrentContact.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            UserWrapper Wrapper = new UserWrapper();
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(u =>u.UserId == (int)LoggedInUserID);
            Wrapper.CurrentContact = CurrentContact;
            return View("EditContact", Wrapper);
        }
        [HttpPost("ChangeContact")]
        public IActionResult ChangeContact(UserWrapper WrappedContact)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            Contact ContactToEdit = dbContext.Contacts.Include(c => c.Company).FirstOrDefault(c => c.ContactId == WrappedContact.CurrentContact.ContactId);
            ContactToEdit.Name = WrappedContact.CurrentContact.Name;
            ContactToEdit.PhoneNumber = WrappedContact.CurrentContact.PhoneNumber;
            ContactToEdit.Email = WrappedContact.CurrentContact.Email;
            ContactToEdit.Notes = WrappedContact.CurrentContact.Notes;
            WrappedContact.CurrentContact = ContactToEdit;
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            if(WrappedContact.CurrentContact.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            if(ModelState.IsValid)
            {
                dbContext.Contacts.Update(ContactToEdit);
                dbContext.SaveChanges();
                return Redirect($"company/{ContactToEdit.Company.CompanyId}");
            }
            return EditContact(WrappedContact.CurrentContact.ContactId);
        }
		[HttpPost("ChangeCompany")]
        public IActionResult ChangeCompany(UserWrapper WrappedCompany)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            Company CompanyToEdit = dbContext.Companies.FirstOrDefault(c => c.CompanyId == WrappedCompany.CurrentCompany.CompanyId);
            CompanyToEdit.Name = WrappedCompany.CurrentCompany.Name;
            CompanyToEdit.Description = WrappedCompany.CurrentCompany.Description;
            CompanyToEdit.Notes = WrappedCompany.CurrentCompany.Notes;
            WrappedCompany.CurrentCompany = CompanyToEdit;
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            if(WrappedCompany.CurrentCompany.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            if(ModelState.IsValid)
            {
                dbContext.Companies.Update(CompanyToEdit);
                dbContext.SaveChanges();
                return Redirect($"company/{CompanyToEdit.CompanyId}");
            }
            return EditCompany(WrappedCompany.CurrentCompany.CompanyId);
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
            Company CurrentCompany = dbContext.Companies.Include(p => p.Positions).FirstOrDefault(c => c.CompanyId == companyid);
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
            Wrapper.AllContacts = dbContext.Contacts.Include(c => c.Company).Where(u => u.Company.UserId == (int)LoggedInUserID && u.CompanyId == Wrapper.CurrentCompany.CompanyId).ToList();
            return View("IndividualCompany", Wrapper);
        }
		[HttpGet("position/{positionid}")]
        public IActionResult IndividualPosition(int positionid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            Position CurrentPosition = dbContext.Positions.Include(c => c.Company).Include(i => i.Interviews).FirstOrDefault(c => c.PositionId == positionid);
            List<Interview> AllInterviews = dbContext.Interviews.OrderBy(d => d.Date).Where(p => p.PositionId == positionid).ToList();
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            else if(CurrentPosition == null || CurrentPosition.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            UserWrapper Wrapper = new UserWrapper();
            Wrapper.CurrentUser = dbContext.Users.FirstOrDefault(u =>u.UserId == (int)LoggedInUserID);
            Wrapper.CurrentPosition = CurrentPosition;
            Wrapper.AllInterviews = AllInterviews;
            return View("IndividualPosition", Wrapper);
        }
		[HttpGet("NewPosition/{companyid}")]
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
		[HttpGet("NewInterview/{interviewid}")]
        public IActionResult NewInterview(int interviewid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Position CurrentPosition = dbContext.Positions.Include(c => c.Company).FirstOrDefault(p => p.PositionId == interviewid);
            if(CurrentPosition == null || CurrentPosition.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            UserWrapper Wrapper = new UserWrapper();
            Wrapper.CurrentPosition = CurrentPosition;
            return View("NewInterview", Wrapper);
        }
		[HttpGet("NewContact/{companyid}")]
        public IActionResult NewContact(int companyid)
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
            return View("NewContact", Wrapper);
        }
        [HttpPost("CreateContact")]
        public IActionResult CreateContact(UserWrapper WrappedContact)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Company CurrentCompany = dbContext.Companies.FirstOrDefault(c => c.CompanyId == WrappedContact.CurrentContact.CompanyId);
            if(CurrentCompany == null || CurrentCompany.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            if(ModelState.IsValid)
            {
                dbContext.Contacts.Add(WrappedContact.CurrentContact);
                dbContext.SaveChanges();
                return IndividualCompany(WrappedContact.CurrentContact.Company.CompanyId);
            }
            WrappedContact.CurrentCompany = CurrentCompany;
            return View("NewContact", WrappedContact);
        }
		[HttpPost("CreateInterview")]
        public IActionResult CreateInterview(UserWrapper WrappedInterview)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Position CurrentPosition = dbContext.Positions.Include(c => c.Company).FirstOrDefault(p => p.PositionId == WrappedInterview.CurrentInterview.PositionId);
            if(CurrentPosition == null || CurrentPosition.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            if(DateTime.Now.CompareTo(WrappedInterview.CurrentInterview.Date)>0)
            {
                ModelState.AddModelError("CurrentInterview.Date","Please enter a time in the future");
            }
            if(ModelState.IsValid)
            {
                dbContext.Interviews.Add(WrappedInterview.CurrentInterview);
                dbContext.SaveChanges();
                return Redirect($"position/{CurrentPosition.PositionId}");
            }
            return NewInterview(CurrentPosition.PositionId);
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
                return IndividualCompany(CurrentCompany.CompanyId);
            }
            WrappedPosition.CurrentCompany = CurrentCompany;
            return View("NewPosition", WrappedPosition);
        }
		[HttpGet("DeleteCompany/{companyid}")]
        public IActionResult Delete(int companyid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Company CompanyToDelete = dbContext.Companies.FirstOrDefault(c => c.CompanyId == companyid);
            if(CompanyToDelete == null || CompanyToDelete.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            dbContext.Companies.Remove(CompanyToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
		[HttpGet("DeletePosition/{positionid}")]
        public IActionResult DeletePosition(int positionid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Position PositionToDelete = dbContext.Positions.Include(c => c.Company).FirstOrDefault(c => c.PositionId == positionid);
            if(PositionToDelete == null || PositionToDelete.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            dbContext.Positions.Remove(PositionToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
		[HttpGet("DeleteContact/{contactid}")]
        public IActionResult DeleteContact(int contactid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Contact ContactToDelete = dbContext.Contacts.Include(c => c.Company).FirstOrDefault(c => c.ContactId == contactid);
            if(ContactToDelete == null || ContactToDelete.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            dbContext.Contacts.Remove(ContactToDelete);
            dbContext.SaveChanges();
            return Redirect($"/company/{ContactToDelete.CompanyId}");
        }
		[HttpGet("DeleteInterview/{interviewid}")]
        public IActionResult DeleteInterview(int interviewid)
        {
            int? LoggedInUserID = HttpContext.Session.GetInt32("LoggedInUserID");
            if(LoggedInUserID==null)
            {
                return RedirectToAction("Index");
            }
            Interview InterviewToDelete = dbContext.Interviews.Include(p => p.Position).ThenInclude(c => c.Company).FirstOrDefault(i => i.InterviewId == interviewid);
            if(InterviewToDelete == null || InterviewToDelete.Position.Company.UserId != (int)LoggedInUserID)
            {
                return RedirectToAction("Dashboard");
            }
            dbContext.Interviews.Remove(InterviewToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
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
