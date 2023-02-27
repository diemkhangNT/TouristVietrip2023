using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            if (data == null)
            {
                TempData["error"] = "Tài khoản đăng nhập không đúng";
                ViewBag.test = username;
                return View("demo");
            }
            else if (taikhoan != null)
            {
                //add session
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["user"] = taikhoan;
                var user = data.IdPos.ToString();
                if (user == tm)
                {
                    return RedirectToAction("HomePageTM", "Tourmanager" );
                }
                else if (user == op)
                {
                    return RedirectToAction("HomePageOP", "OrderProcessing");
                }
                else if (user == ad)
                {
                    return RedirectToAction("HomePage", "Admin");
                }
               
            }
            return View();
        }

        public ActionResult demo()
        {
            return View();
        }
    }
}