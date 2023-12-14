using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Tourist_VietripInsum_2023.App_Start;
using Tourist_VietripInsum_2023.DesignPattern.Singleton;
using Tourist_VietripInsum_2023.DesignPattern.TemplateMethod;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    [AdminAuthorize(idPos  = "AD")]
    public class AdminController : TemplateMethodController
    {
        // GET: Admin

        TouristEntities1 db = new TouristEntities1();

        public AdminController()
        {
            var result = PrintInfo();
            Debugger.Log(1, "Logger: ", $"{result}");
        }

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
                var dd = st.HinhDaiDien;
                st.HinhDaiDien = dd;
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

                TempData["thongbao"] = "taothanhcong";
                db.NhanViens.Add(staff);
                db.SaveChanges();
                UserLogedInSingleton<NhanVien>.Instance.UpdateSigleton(db);
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
                TempData["thongbao"] = "edithanhcong";
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

        public ActionResult EditGuest(string id)
        {
            
            var guest = db.KhachHangs.Where(s => s.MaKH == id).FirstOrDefault();
            ViewBag.MaLoaiKH = new SelectList(db.LoaiKHs, "MaLoaiKH", "TenLoaiKH", guest.MaLoaiKH);
            return View(guest);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGuest(KhachHang guest, HttpPostedFileBase HinhDaiDien)
        {
            
            if (ModelState.IsValid)
            {
                var kh = db.KhachHangs.FirstOrDefault(p => p.MaKH == guest.MaKH);
                if (kh != null)
                {
                    // đổi dữ liệu thay đổi từ cũ sang mới,productdb đổi sang mới
                    kh.SDT = guest.SDT;
                    kh.MaLoaiKH = guest.MaLoaiKH;
                    kh.Username = guest.Username;
                    kh.UserPassword = guest.UserPassword;
                    kh.HoTenKH = guest.HoTenKH;
                    kh.DiaChi = guest.DiaChi;
                    kh.Email = guest.Email;
                    kh.NgaySinh = guest.NgaySinh;
                    kh.GioiTinh = guest.GioiTinh;

                    if (HinhDaiDien != null)
                    {
                        var fileName = Path.GetFileName(HinhDaiDien.FileName);
                        var path = Path.Combine(Server.MapPath("/Images"), fileName);
                        kh.HinhDaiDien = fileName;
                        HinhDaiDien.SaveAs(path);

                    }
                }
                //db.Entry(product).State = EntityState.Modified;
                //luu procduct
                db.SaveChanges();
                TempData["thongbaoguest"] = "edit";
                return RedirectToAction("ListOfCustomers");
            }
            ViewBag.LoaiKH = new SelectList(db.LoaiKHs, "MaLoaiKH", "TenLoaiKH", guest.MaLoaiKH);
            return View(guest);
        }

        public JsonResult CheckUserKH(string userdata,string makh)
        {
            System.Threading.Thread.Sleep(200);
            var ktr = db.KhachHangs.Where(x => x.Username == userdata && x.MaKH != makh ).SingleOrDefault();
          
            if (ktr != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                // Kiểm tra email có đúng định dạng
                var match = Regex.Match(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (match.Success)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        public JsonResult CheckMailKH(string usermailkh,string makh)
        {
            System.Threading.Thread.Sleep(200);
            var SeachData = db.KhachHangs.Where(x => x.Email == usermailkh && x.MaKH != makh).SingleOrDefault();

            bool isValid = IsValidEmail(usermailkh);
            if (isValid == true)
            {
                if (SeachData != null)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }
            else
            {
                return Json(2);
            }

        }
        public JsonResult CheckSDTKH(string userSDT)
        {
            System.Threading.Thread.Sleep(200);
            var ktra = db.KhachHangs.Where(x => x.SDT == userSDT).SingleOrDefault();
            if (ktra != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }

        public ActionResult DetailGuest(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang guest = db.KhachHangs.Where(s => s.MaKH == id).FirstOrDefault();
            if (guest == null)
            {
                return HttpNotFound();
            }
            return View(guest);
        }

        public ActionResult DeleteGuest(string id)
        {
            var guest = db.KhachHangs.Where(s => s.MaKH == id).FirstOrDefault();
            List<BookTour> dh = db.BookTours.Where(t => t.MaKH == guest.MaKH).ToList();
            if (dh.Count==0)
            {
                db.KhachHangs.Remove(guest);
                TempData["thongbaoguest"] = "thanhcong";
                db.SaveChanges();
               return  RedirectToAction("ListOfCustomers");
            }
            else
            {
                TempData["thongbaoxoa"] = "thatbai";
                return RedirectToAction("DetailGuest","Admin", new { id = id });
            }
        }

        public override string PrintRoutes()
        {
            return "========================" +
                "Admin Controller is running!" +
                "======================";
        }

        public override string PrintDIs()
        {
            return "=================No dependence Injection================\n";
        }
    }
}