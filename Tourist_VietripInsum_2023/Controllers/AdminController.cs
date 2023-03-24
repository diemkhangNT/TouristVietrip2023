using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tourist_VietripInsum_2023.App_Start;

using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    [AdminAuthorize(idPos  = "AD")]
    public class AdminController : Controller
    {
        // GET: Admin

        TouristEntities1 db = new TouristEntities1();


        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult ListOfStaff()
        {
            var listTourManagers = db.NhanViens.ToList();
            return View(listTourManagers);
        }

        public ActionResult ListOfCustomers()
        {
            var listCustomer = db.KhachHangs.ToList();
            return View(listCustomer);
        }


        public JsonResult CheckUsernameAvailability(string userdata,string usermail)
        {
            System.Threading.Thread.Sleep(200);
            var SeachData = db.NhanViens.Where(x => x.Username == userdata).SingleOrDefault();
            var mailuser = db.NhanViens.Where(x => x.Email == usermail).SingleOrDefault();
            if (SeachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }
        public JsonResult CheckEmailAvailability(string usermail)
        {
            System.Threading.Thread.Sleep(200);
            
            var mailuser = db.NhanViens.Where(x => x.Email == usermail).SingleOrDefault();
            if (mailuser != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }
        public JsonResult CheckSDTAvailability(string userSDT)
        {
            System.Threading.Thread.Sleep(200);

            var SDTuser = db.NhanViens.Where(x => x.Sdt == userSDT).SingleOrDefault();
            if (SDTuser != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }

        public ActionResult CreateStaff ()
        {
            return View();
        }
       
        public void LuuAnh(NhanVien st, HttpPostedFileBase Avatar)
        {
            #region Hình ảnh

            if(Avatar==null)
            {
                st.HinhDaiDien = "/images/profile-user.png";
            }
            else
            {
                
                //Xác định đường dẫn lưu file : Url tương đói => tuyệt đói
                var urlTuongdoi = "/images/";
                var urlTuyetDoi = Server.MapPath(urlTuongdoi);// Lấy đường dẫn lưu file trên server

                //Check trùng tên file => Đổi tên file  = tên file cũ (ko kèm đuôi)
                //Ảnh.jpg = > ảnh + "-" + 1 + ".jpg" => ảnh-1.jpg

                string fullDuongDan = urlTuyetDoi + Avatar.FileName;


                int i = 1;
                while (System.IO.File.Exists(fullDuongDan) == true)
                {
                    // 1. Tách tên và đuôi 
                    var ten = Path.GetFileNameWithoutExtension(Avatar.FileName);
                    var duoi = Path.GetExtension(Avatar.FileName);
                    // 2. Sử dụng biến i để chạy và cộng vào tên file mới
                    fullDuongDan = urlTuyetDoi + ten + "-" + i + duoi;
                    i++;
                    // 3. Check lại 
                }
                #endregion
                //Lưu file (Kiểm tra trùng file)
                Avatar.SaveAs(fullDuongDan);
                st.HinhDaiDien = urlTuongdoi + Path.GetFileName(fullDuongDan);
            }    
        }

        [HttpPost]
        public ActionResult CreateStaff(NhanVien staff,HttpPostedFileBase Avatar,string id)
        {

            var usernamecheck = db.NhanViens.FirstOrDefault(k => k.Username == staff.Username);
            var std = db.NhanViens.FirstOrDefault(k => k.Sdt == staff.Sdt);
            var emailcheck = db.NhanViens.FirstOrDefault(k => k.Email == staff.Email);
            if (usernamecheck != null)
            { ModelState.AddModelError(string.Empty, "Đã có tên đăng nhập !!!");
            }
            if (std != null)
            {
                ModelState.AddModelError(string.Empty, "Đã có SDT trong hệ thống !!!");
            }
            if (emailcheck != null)
            {
                ModelState.AddModelError(string.Empty, "Đã có email trong hệ thống !!!");
            }

            if (ModelState.IsValid)
            {
                LuuAnh(staff, Avatar);
                Random rd = new Random();
                var idstaff = "ST" + rd.Next(1, 1000);
                staff.MaNV = idstaff;

                var pas = "123456";
                staff.UserPassword = pas;

                TempData["messageAlert"] = "createoke";
                db.NhanViens.Add(staff);
                db.SaveChanges();
            }
            else
            {
                return View();

            }

           
            
            return RedirectToAction("ListOfStaff");            
        }
        

        public ActionResult DetailStaff(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien st = db.NhanViens.Where(s => s.MaNV == id).FirstOrDefault();
            if (st == null)
            {
                return HttpNotFound();
            }
            return View(st);
        }

        public ActionResult EditStaff(string id)
        {
            return View(db.NhanViens.Where(s => s.MaNV == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditStaff( NhanVien nv, HttpPostedFileBase Avatar)
        {
            if (ModelState.IsValid)
            {

                LuuAnh(nv, Avatar);
                db.Entry(nv).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["messageAlert"] = "editoke";
                return RedirectToAction("ListOfStaff");
            }
            return View(nv);

        }
        
        public ActionResult DeleteStaffs(string id)
        {
            var st = db.NhanViens.Where(s => s.MaNV == id).FirstOrDefault();
            if(st!=null)
            {
                db.NhanViens.Remove(st);
                TempData["messageAlert"] = "deletestaff";
                db.SaveChanges();
                
            }
            else
            {
                TempData["messageAlert"] = "khongcostaff";
            }
            return RedirectToAction("ListOfStaff");

        }
    }
}