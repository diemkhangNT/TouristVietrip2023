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
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase fileImage)
        {
            if (fileImage != null || fileImage.ContentLength > 0)
            {
                string _FileName = Path.GetFileName(fileImage.FileName);
                ViewBag.nd = Path.GetFileName(fileImage.FileName);
                string path = Path.Combine(Server.MapPath("/images/"), _FileName);
                if (System.IO.File.Exists(path))
                {
                    //nếu hình ảnh đã tồn tại, thì xóa ảnh cũ, cập nhật lại ảnh mới
                    System.IO.File.Delete(path);
                    fileImage.SaveAs(path);
                }
                else
                {
                    fileImage.SaveAs(path);
                }
            }
            return RedirectToAction("CreateStaff");
        }
        [HttpPost]
        public ActionResult CreateStaff(Staff staff,HttpPostedFileBase file)
        {
            Random rd = new Random();
            var idstaff = "ST" + rd.Next(1, 1000);
            staff.IdStaff = idstaff;

            

            


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
        public ActionResult EditStaff(string id,Staff nv)
        {
            db.Entry(nv).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Staffmanager");
        }
        public ActionResult DeleteStaff(string id)
        {
            return View(db.Staffs.Where(s => s.IdStaff == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeleteStaff(string id, Staff st)
        {
            try
            {
                
                st = db.Staffs.Where(s => s.IdStaff == id).FirstOrDefault();
                db.Staffs.Remove(st);
                db.SaveChanges();
                return RedirectToAction("Staffmanager");

            }
            catch
            {
                return Content("err");
            }
        }
    }
}