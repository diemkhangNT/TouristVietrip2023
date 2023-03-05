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

        public ActionResult Staffmanager()
        {
            var staff = db.Staffs.ToList().OrderByDescending(s => s.IdStaff);
            return View(staff.ToList());
        }

        
        public ActionResult CreateStaff ()
        {
            return View();
        }
       
        public void LuuAnh(Staff st, HttpPostedFileBase Avatar)
        {
            #region Hình ảnh

            if(Avatar==null)
            {
                st.Avatar = "/images/profile-user.png";
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
                st.Avatar = urlTuongdoi + Path.GetFileName(fullDuongDan);
            }    
        }

        [HttpPost]
        public ActionResult CreateStaff(Staff staff,HttpPostedFileBase Avatar)
        {
            //if(favatar.FileName!="")
            // {
            //     staff.Avatar = favatar.FileName;

            //     string ten = Path.GetFileNameWithoutExtension(favatar.FileName);
            //     string morong = Path.GetExtension(favatar.FileName);
            //     string tendaydu = ten + DateTime.Now.ToString("yyMMddHHmmssff") + morong;
            //     favatar.SaveAs(Path.Combine(Server.MapPath("~/images"), tendaydu));
            // }    
            LuuAnh(staff, Avatar);
            Random rd = new Random();
            var idstaff = "ST" + rd.Next(1, 1000);
            staff.IdStaff = idstaff;

            var pas = "123456";
            staff.UserPassword = pas;

            


            db.Staffs.Add(staff);
            db.SaveChanges();
            return RedirectToAction("Staffmanager");
        }
        public ActionResult DetailStaff(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff st = db.Staffs.Where(s => s.IdStaff == id).FirstOrDefault();
            if (st == null)
            {
                return HttpNotFound();
            }
            return View(st);
        }

        public ActionResult EditStaff(string id)
        {
            return View(db.Staffs.Where(s => s.IdStaff == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditStaff(string id,Staff nv, HttpPostedFileBase Avatar)
        {
            LuuAnh(nv, Avatar);
            db.Entry(nv).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Staffmanager");
        }
        
       
        public ActionResult DeleteStaff(string id, Staff st)
        {
           
                st = db.Staffs.Where(s => s.IdStaff == id).FirstOrDefault();
                db.Staffs.Remove(st);
                TempData["messageAlert"] = "Đã xóa staff";             
                db.SaveChanges();
                return RedirectToAction("Staffmanager");

           
        }
    }
}