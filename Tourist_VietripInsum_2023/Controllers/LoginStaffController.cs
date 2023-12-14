using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tourist_VietripInsum_2023.common;
using Tourist_VietripInsum_2023.DesignPattern.Repository;
using Tourist_VietripInsum_2023.DesignPattern.Singleton;
using Tourist_VietripInsum_2023.DesignPattern.TemplateMethod;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    public class LoginStaffController : TemplateMethodController
    {
        TouristEntities1 database = new TouristEntities1();
        IBaseRepository _baseRepository = new BaseRepository();

        public LoginStaffController()
        {
            UserLogedInSingleton<NhanVien>.Instance.InitSingleton(database);

            var result = PrintInfo();
            Debugger.Log(1, "Logger: ", $"{result}");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var tm = "TM"; var op = "OP"; var ad = "AD";

            var data = database.NhanViens.Where(s => s.Username == username && s.UserPassword == password).FirstOrDefault();
            var taikhoan = UserLogedInSingleton<NhanVien>.Instance.Users.SingleOrDefault(s => s.Username == username && s.UserPassword == password);
            if (taikhoan == null)
            {
                TempData["error"] = "err";
                return View("Login");
            }
            else if (taikhoan != null)
            {
                //add session
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["user"] = taikhoan;
                var user = data.MaCV.ToString();
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
                //ViewBag.test = username;
            }
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string mail)
        {
            string newpass = _baseRepository.RandomPassword(8);
            var kh = database.KhachHangs.FirstOrDefault(t => t.Email == mail);
            kh.UserPassword = newpass;
            database.SaveChanges();
            string content = System.IO.File.ReadAllText(Server.MapPath("/Content/template/MailQuenMK.html"));
            content = content.Replace("{{email}}", mail);
            content = content.Replace("{{username}}", kh.Username);
            content = content.Replace("{{matkhau}}", newpass);


            ////Gui mail
            var toEmail = ConfigurationManager.AppSettings["toEmailAddress"].ToString();
            new MailHelp().SendMail(kh.Email, "Thông tin tài khoản", content);
            TempData["noti"] = "capnhat";
            return RedirectToAction("Login");
        }
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            UserLogedInSingleton<NhanVien>.Instance.Users.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public override string PrintRoutes()
        {
            return "========================" +
                "Login Staff Controller is running!" +
                "======================";
        }

        public override string PrintDIs()
        {
            return "=================No dependence Injection================\n";
        }
    }
}