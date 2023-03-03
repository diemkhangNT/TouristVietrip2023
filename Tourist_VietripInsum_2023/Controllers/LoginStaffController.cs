using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    public class LoginStaffController : Controller
    {
        // GET: LoginStaff
        TouristEntities1 database = new TouristEntities1();
       public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var tm = "TM"; var op = "OP"; var ad = "AD";

            var data = database.Staffs.Where(s => s.Username== username && s.UserPassword == password).FirstOrDefault();
            var taikhoan = database.Staffs.SingleOrDefault(s => s.Username == username && s.UserPassword == password);
            if (taikhoan == null)
            {
                TempData["AlertMessage"] = "Login error";
                return RedirectToAction("Login", "LoginStaff");
                //ViewBag.test = username;
            }
            else if (taikhoan != null)
            {
                //add session
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["user"] = taikhoan;
                var user = data.IdPos.ToString();
                if (user == tm)
                {
                    TempData["AlertMessage"] = "Login sucess";
                    return RedirectToAction("HomePageTM", "Tourmanager" );
                }
                else if (user == op)
                {
                    TempData["AlertMessage"] = "Login sucess";
                    return RedirectToAction("HomePageOP", "OrderProcessing");
                }
                else if (user == ad)
                {
                    TempData["AlertMessage"] = "Login sucess";
                    return RedirectToAction("HomePage", "Admin");
                }
               
            }
            else
            {
                TempData["AlertMessage"] = "Login error";
                return RedirectToAction("Login", "LoginStaff");
                ViewBag.test = username;
            }
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}