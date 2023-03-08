using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tourist_VietripInsum_2023.App_Start;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    [AdminAuthorize (idPos = "TM")]
    public class TourmanagerController : Controller
    {
        private TouristEntities1 db = new TouristEntities1();
        // GET: Tourmanager

        public ActionResult HomePageTM()
        {
            //tính tổng số kho
            var totalT = 0;
            //totalT = db.Tours.ToList().Count;
            totalT = 0;
            TempData["tongTour"] = totalT;
            //tính tổng sản phẩm sắp hết hàng
            var totalTT = 0;
            totalTT = db.TourTypes.ToList().Count;
            TempData["tongLT"] = totalTT;

            ////tính tổng số sản phẩm
            //var totalsp = db.VistLocations.ToList().Count;
            //TempData["Tongsp1"] = totalsp;
            ////tính tổng số Phiếu nhập xuất hàng
            //var tongpn = 0;
            //var tongpx = 0;
            //var pnx = database.PhieuNhapXuats.ToList();
            //foreach (var item in pnx)
            //{
            //    string str = item.MaPhieu.Substring(0, 2);
            //    if (str == "PX")
            //    {
            //        tongpx = tongpx + 1;
            //    }
            //    else
            //    {
            //        tongpn = tongpn + 1;
            //    }
            //}
            //TempData["Tongpn"] = tongpn;
            //TempData["Tongpx"] = tongpx;
            ////tính tổng sản phẩm tồn kho
            //var total3 = 0;
            //foreach (var item in dssphh)
            //{
            //    if (item.TinhTrang == "Tồn kho")
            //    {
            //        total3 = total3 + 1;
            //    }
            //}
            //TempData["TongSPTK"] = total3;

            return View(db.Tours.ToList());
        }

        // GET: Tours
        //Trang hiển thị danh sách loại tour, tour
        public ActionResult QuanLyTour()
        {
            //var tours = db.Tours.Include(t => t.Schedule).Include(t => t.TourType);
            //var tourstype = db.TourTypes.ToList();
            var tours = db.Tours.ToList().OrderByDescending(s => s.IdTour);
            return View(tours.ToList());
        }

        //Chi tiết loại tour
        public ActionResult TourTypeDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TourType tourType = db.TourTypes.Find(id);
            if (tourType == null)
            {
                return HttpNotFound();
            }
            return View(tourType);
        }

        //Tạo loại tour
        public ActionResult CreateTourType()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTourType([Bind(Include = "IdType,TypeName,Discount")] TourType tourType)
        {
            if (ModelState.IsValid)
            {
                db.TourTypes.Add(tourType);
                db.SaveChanges();
                return RedirectToAction("QuanLyTour");
            }
            return View(tourType);
        }

        //Sửa loại tour
        public ActionResult EditTourType(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TourType tourType = db.TourTypes.Find(id);
            if (tourType == null)
            {
                return HttpNotFound();
            }
            return View(tourType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTourType([Bind(Include = "IdType,TypeName,Discount")] TourType tourType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tourType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QuanLyTour");
            }
            return View(tourType);
        }

        //Xóa loại tour
        public ActionResult DeleteTourType(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TourType tourType = db.TourTypes.Find(id);
            if (tourType == null)
            {
                return HttpNotFound();
            }
            return View(tourType);
        }

        

     //Hotel
     public ActionResult HotelManager(string id)
        {
            var ht = db.Hotels.ToList().OrderByDescending(s => s.IdHotel);
            return View(ht.ToList());
        }
        public ActionResult CreateHotel(string id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateHotel(Hotel hotel,HttpPostedFileBase Image )
        {
            //LuuAnh(detail, Image);
            Random rd = new Random();
            var idhotel = "H" + rd.Next(1, 1000);
            hotel.IdHotel = idhotel;

            




            db.Hotels.Add(hotel);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");
        }
        public ActionResult EditHotel(string id)
        {
            return View();
        }
        public ActionResult DeleteManager(string id)
        {
            return View();
        }

        public ActionResult CreateTours()
        {
            return View();
        }

        public void LuuAnh(Tour t, HttpPostedFileBase ImagerTour)
        {
            #region Hình ảnh

            if (ImagerTour == null)
            {
                t.ImagerTour = "/images/profile-user.png";
            }
            else
            {

                //Xác định đường dẫn lưu file : Url tương đói => tuyệt đói
                var urlTuongdoi = "/images/";
                var urlTuyetDoi = Server.MapPath(urlTuongdoi);// Lấy đường dẫn lưu file trên server

                //Check trùng tên file => Đổi tên file  = tên file cũ (ko kèm đuôi)
                //Ảnh.jpg = > ảnh + "-" + 1 + ".jpg" => ảnh-1.jpg

                string fullDuongDan = urlTuyetDoi + ImagerTour.FileName;


                int i = 1;
                while (System.IO.File.Exists(fullDuongDan) == true)
                {
                    // 1. Tách tên và đuôi 
                    var ten = Path.GetFileNameWithoutExtension(ImagerTour.FileName);
                    var duoi = Path.GetExtension(ImagerTour.FileName);
                    // 2. Sử dụng biến i để chạy và cộng vào tên file mới
                    fullDuongDan = urlTuyetDoi + ten + "-" + i + duoi;
                    i++;
                    // 3. Check lại 
                }
                #endregion
                //Lưu file (Kiểm tra trùng file)
                ImagerTour.SaveAs(fullDuongDan);
                t.ImagerTour = urlTuongdoi + Path.GetFileName(fullDuongDan);
            }
        }

        [HttpPost]
        public ActionResult CreateTours(Tour tour,HttpPostedFileBase ImagerTour)
        {
            LuuAnh(tour, ImagerTour);
            Random rd = new Random();
            var idtour = "Tour" + rd.Next(1, 1000);
            tour.IdTour = idtour;

           



           
            db.Tours.Add(tour);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");
        }

        //Chi tiết tour
        public ActionResult TourDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tour tour = db.Tours.Where(s => s.IdTour == id).FirstOrDefault();
            if (tour == null)
            {
                return HttpNotFound();
            }
            return View(tour);
        }
        
        
        public ActionResult EditTour(string id)
        {
            return View(db.Tours.Where(s => s.IdTour == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditTour(string id,HttpPostedFileBase ImagerTour, Tour tour)
        {
            LuuAnh(tour, ImagerTour);
            db.Entry(tour).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["noti"] = "oke";
            return RedirectToAction("QuanLyTour");
        }

        //Xóa tour
        public ActionResult DeleteTour(string id,Tour tour,DetailTour detailTour)
        {
            tour = db.Tours.Where(s => s.IdTour == id).FirstOrDefault();
            detailTour = db.DetailTours.Where(s => s.IdTour == id).FirstOrDefault();
            db.Tours.Remove(tour);
            if(detailTour!=null)
            {
                db.DetailTours.Remove(detailTour);
            }    
           
            TempData["messageAlert"] = "";
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");
        }
        //DetailTour

        public ActionResult ListDetailTour(string id)
        {
            var dt = db.DetailTours.Where(s => s.IdTour == id).ToList();
            return View(dt);
        }

        public ActionResult CreateDetailTour(string id)
        {
            return View(db.DetailTours.Where(s => s.IdTour == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult CreateDetailTour(string id,DetailTour detailTour,Tour tour)
        {
            tour = db.Tours.Where(s => s.IdTour == id).FirstOrDefault();

            Random rd = new Random();
            var idDetail = "DTOUR"+ rd.Next(1, 1000);
            detailTour.IdSchedule = idDetail;
            detailTour.IdTour = "Tour552";

            db.DetailTours.Add(detailTour);
            db.SaveChanges();
            return RedirectToAction("TourDetail");
        }





        // POST: Tours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Tour tour = db.Tours.Find(id);
            db.Tours.Remove(tour);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Profile()
        {
            return View(); 
        }
        // GET: Staffs/Details/5
      

    }
}