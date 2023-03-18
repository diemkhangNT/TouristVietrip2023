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

        public JsonResult CheckUsernameAvailability(string userdata)
        {
            System.Threading.Thread.Sleep(200);
            var SeachData = db.NhanViens.Where(x => x.Username == userdata).SingleOrDefault();
            if (SeachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }
        //public JsonResult checkmail(string userdata)
        //{
        //    System.Threading.Thread.Sleep(200);
        //    var SeachDatas = db.Staffs.Where(x => x.StaffEmail == userdata).SingleOrDefault();
        //    if (SeachDatas != null)
        //    {
        //        return Json(1);
        //    }
        //    else
        //    {
        //        return Json(0);
        //    }

        //}


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
            LuuAnh(staff, Avatar);
            Random rd = new Random();
            var idstaff = "ST" + rd.Next(1, 1000);
            staff.MaNV= idstaff;

            var pas = "123456";
            staff.UserPassword = pas;

            TempData["noti"] = "oke";
            db.NhanViens.Add(staff);
            db.SaveChanges();
            return RedirectToAction("ListOfStaff");            
        }
        //[HttpPost]
        //public JsonResult CheckUsername(string username)
        //{

        //    bool isValid = !db.Staffs.ToList().Exists(p => p.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
        //    return Json(isValid);
        //}

        //public JsonResult IsAlreadySigned(string UserName)
        //{

        //    return Json(IsUserAvailable(UserName));

        //}
        //public bool IsUserAvailable(string EmailId)
        //{
        //    List<Staff> lis = db.Staffs.ToList();
        //    var c = db.Staffs.Where(x => x.Username == EmailId).SingleOrDefault();
        //    bool status = false;
        //    bool flg= false;
        //    for (int i=0;i<lis.Count;i++)
        //    {

        //        if (c.Username==lis[i].Username)
        //        {
        //            //Already registered  
        //            status = false;
        //        }
        //        else //Available to use  
        //            status = true;

        //    }
        //    flg = status;

        //    return flg;
        //}

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
        public ActionResult EditStaff(string id, NhanVien nv, HttpPostedFileBase Avatar)
        {
            LuuAnh(nv, Avatar);
            db.Entry(nv).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["noti"] = "oke";
            return RedirectToAction("ListOfStaff");
        }
        
        public ActionResult DeleteStaff(string id, NhanVien st)
        {
            st = db.NhanViens.Where(s => s.MaNV == id).FirstOrDefault();
            db.NhanViens.Remove(st);
            TempData["messageAlert"] = "Đã xóa staff";
            db.SaveChanges();
            return RedirectToAction("ListOfStaff");
        }
    }
}