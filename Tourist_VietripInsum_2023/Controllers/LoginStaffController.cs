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
        public ActionResult LoginStaff()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult LoginStaff(string userstaff,string pswstaff)
        {
            var data = database.Staffs.Where(s => s.Username == userstaff && s.UserPassword == pswstaff).FirstOrDefault();
            var taikhoan = database.Staffs.SingleOrDefault(s => s.Username == userstaff && s.UserPassword == pswstaff);
            if (data == null)
            {
                TempData["error"] = "Tài khoản đăng nhập không đúng";
                return View("LoginStaff");
            }
            else if (taikhoan != null)
            {
                //add session
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["user"] = taikhoan;
                if (data.IdPos.ToString() == "TM")
                {
                    return RedirectToAction("HomePageTM", "Tourmanager");
                }
                else if (data.IdPos.ToString() == "OP")
                {
                    return RedirectToAction("HomePageOP", "OrderProcessing");
                } 
                else 
                {
                    return RedirectToAction("HomePage", "Admin");
                }
            }
           
            return View();
        }
    }
}