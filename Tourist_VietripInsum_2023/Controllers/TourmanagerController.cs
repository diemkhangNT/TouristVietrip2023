using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
        //Trang hiển thị danh sách loại tour, hotel, tour
        public ActionResult QuanLyTour()
        {
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
        [HttpPost, ActionName("DeleteTourType")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTourTypeConfirmed(string id)
        {
            TourType tourType = db.TourTypes.Find(id);
            db.TourTypes.Remove(tourType);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");
        }

        //Chi tiết khách sạn
        public ActionResult HotelDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel hotel = db.Hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        //Tạo khách sạn
        [HttpGet]
        public ActionResult CreateHotel()
        {
            return View();
        }

        //public JsonResult AddHotel(Hotel hotel,string cityaddress,string districtaddress,string staddress,string namehotel, string id,int leveltour)
        //{

        //        Random rd = new Random();
        //        var idHotel = "H" + rd.Next(1, 1000);
        //        hotel.IdHotel = idHotel;
        //        hotel.NameHotel = namehotel;
        //        hotel.LevelHotel = leveltour;
        //        hotel.AddressHotel = staddress + ", " + districtaddress + ", " + cityaddress;

        //        db.Hotels.Add(hotel);
        //        db.SaveChanges();

        //    return new JsonResult("save");




        //}
        [HttpPost]
        public ActionResult CreateHotel(Hotel hotel, string cityaddress, string districtaddress, string staddress)
        {
            if (!ModelState.IsValid)
            {
                return View(hotel);
            }
            Random rd = new Random();
                var idHotel = "H" + rd.Next(1, 1000);
                hotel.IdHotel = idHotel;
               
                
              
                hotel.AddressHotel = staddress + ", " + districtaddress + ", " + cityaddress;

                db.Hotels.Add(hotel);
            TempData["noti"] = "add";
            db.SaveChanges();
                return RedirectToAction("HotelManager");
          
        }

        //Sửa hotel
        public ActionResult EditHotel(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hotel hotel = db.Hotels.Find(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHotel([Bind(Include = "IdHotel,LevelHotel,NameHotel,AddressHotel,ImgHotel")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                TempData["noti"] = "edit";
                db.Entry(hotel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HotelManager");
            }
            return View(hotel);
        }

        //Xóa khách sạn
       public ActionResult DetailHotel (string id)
        {
            return View(db.Hotels.Where(s => s.IdHotel == id).FirstOrDefault());
        }


        [HttpPost]
        public ActionResult DeleteHotel(string id, Hotel ht)
        {
            ht = db.Hotels.Where(s => s.IdHotel == id).FirstOrDefault();
            var detail = db.DetailTours.ToList();
            var count = 0;
            for (int i = 0; i < detail.Count; i++)
            {
                if (ht.IdHotel == detail[i].IdHotel)
                {
                    count++;
                }
            }
            if(count>0)
            {
                TempData["noti"] = "delete-false";
                return RedirectToAction("HotelManager");
            }
            else
            {
                TempData["noti"] = "delete-true";
                db.Hotels.Remove(ht);
                db.SaveChanges();
                return RedirectToAction("HotelManager");
            }
            return View(ht);
            

        }


        //// POST: Hotels/Delete/5
        //[HttpPost, ActionName("DeleteHotel")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteHotelConfirmed(string id)
        //{
        //    Hotel hotel = db.Hotels.Find(id);
        //    db.Hotels.Remove(hotel);
        //    db.SaveChanges();
        //    return RedirectToAction("QuanLyTour");
        //}

        //Chi tiết địa điểm tham quan

        public ActionResult TransportManager()
        {
            var ts = db.Transports.ToList().OrderByDescending(s => s.IdTrans);
            return View(ts.ToList());
        }

        public ActionResult CreateTrans()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTrans(Transport trans)
        {
            if (!ModelState.IsValid)
            {
                return View(trans);
            }
            Random rd = new Random();
            var idTrans = "T" + rd.Next(1, 1000);
            trans.IdTrans = idTrans;

            db.Transports.Add(trans);
            TempData["noti"] = "addtrans";
            db.SaveChanges();
            return RedirectToAction("HotelManager");

        }

        public ActionResult EditTrans(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transport transport = db.Transports.Find(id);
            if (transport == null)
            {
                return HttpNotFound();
            }
            return View(transport);
        }
        [HttpPost]
        
        public ActionResult EditTrans(Transport transport)
        {
            if (ModelState.IsValid)
            {
                TempData["noti"] = "edittrans";
                db.Entry(transport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HotelManager");
            }
            return View(transport);
        }


        [HttpPost]
        public ActionResult DeleteTrans( string id, Transport trans)
        {
            
            trans = db.Transports.Where(s => s.IdTrans == id).FirstOrDefault();
            var detail = db.DetailTours.ToList();
            var count = 0;
            for (int i = 0; i < detail.Count; i++)
            {
                if (trans.IdTrans == detail[i].IdTrans)
                {
                    count++;
                }
            }
            if (count > 0)
            {
                TempData["noti"] = "deletetrans-false";
                return RedirectToAction("HotelManager");
            }
            else
            {
                TempData["noti"] = "deletetrans-true";
                db.Transports.Remove(trans);
                db.SaveChanges();
                return RedirectToAction("HotelManager");
            }
           
            return View(trans);


        }

        public ActionResult DetailTrans(string id)
        {
            return View(db.Transports.Where(s => s.IdTrans == id).FirstOrDefault());
        }







        public ActionResult LocationManager()
        {
            var lc = db.VistLocations.ToList().OrderByDescending(s => s.IdVistLocat);
            return View(lc.ToList());
        }


        public ActionResult VistLocationsDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VistLocation vistLocation = db.VistLocations.Find(id);
            if (vistLocation == null)
            {
                return HttpNotFound();
            }
            return View(vistLocation);
        }

        //Tạo địa điểm tham quan
        public ActionResult CreateVisitLocations()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVisitLocations([Bind(Include = "IdVistLocat,NameVist,ImageLocation,Des_Location,Loca_address")] VistLocation vistLocation)
        {
            if (ModelState.IsValid)
            {
                db.VistLocations.Add(vistLocation);
                db.SaveChanges();
                return RedirectToAction("LocationManager");
            }
            return View(vistLocation);
        }

        //Chỉnh sửa địa điểm tham quan
        public ActionResult EditVisitLocations(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VistLocation vistLocation = db.VistLocations.Find(id);
            if (vistLocation == null)
            {
                return HttpNotFound();
            }
            return View(vistLocation);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVisitLocations([Bind(Include = "IdVistLocat,NameVist,ImageLocation,Des_Location,Loca_address")] VistLocation vistLocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vistLocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LocationManager");
            }
            return View(vistLocation);
        }

        //Xóa địa điểm tham quan
        public ActionResult DeleteVisitLocations(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VistLocation vistLocation = db.VistLocations.Find(id);
            if (vistLocation == null)
            {
                return HttpNotFound();
            }
            return View(vistLocation);
        }






     //Hotel
     public ActionResult HotelManager(string id)
        {
            var ht = db.Hotels.ToList().OrderByDescending(s => s.IdHotel);
            return View(ht.ToList());
        }


     
        public ActionResult DeleteManager(string id)
        {
            return View();
        }

        //tour

        
        public ActionResult CreateTours()
        {
            return View();
        }

        public void LuuImage(Tour t, HttpPostedFileBase ImagerTour)
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
            if (!ModelState.IsValid)
            {
                return View(tour);
            }
            LuuImage(tour, ImagerTour);
                Random rd = new Random();
                var idtour = "Tour" + rd.Next(1, 1000);
                tour.IdTour = idtour;
                db.Tours.Add(tour);
                db.SaveChanges();
            TempData["noti"] = "oke";
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
            LuuImage(tour, ImagerTour);
            db.Entry(tour).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["noti"] = "oke";
            return RedirectToAction("QuanLyTour");
        }

        public ActionResult Demo()
        {
            return View();
        }
        public ActionResult DeleteTour(string id, Tour tour)
        {
            tour = db.Tours.Where(s => s.IdTour == id).FirstOrDefault();
            var detail = db.DetailTours.ToList();
            for(int i=0;i<detail.Count;i++)
            {
                if(tour.IdTour==detail[i].IdTour)
                {
                    db.DetailTours.Remove(detail[i]);
                }    
            }    
            db.Tours.Remove(tour);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");

        }

        //public JsonResult DeleteTour(string id)
        //{

        //    bool result;
        //    Tour tour = db.Tours.Where(s => s.IdTour == id).FirstOrDefault();
        //    DetailTour detailTour = db.DetailTours.Where(s => s.IdTour == id).FirstOrDefault();
        //    if (tour != null)
        //    {
        //        db.Tours.Remove(tour);
        //        if (detailTour != null)
        //        {
        //            db.DetailTours.Remove(detailTour);
        //        }


        //        db.SaveChanges();
        //        result = true;
        //    }
        //    else
        //    {
        //        result = false;
        //    }


        //    return Json(result, JsonRequestBehavior.AllowGet);



        //}

        public void SaveImagerDetail(DetailTour dt, HttpPostedFileBase img)
        {
            #region Hình ảnh

            if (img == null)
            {
                dt.Image = "/images/profile-user.png";
            }
            else
            {
                //Xác định đường dẫn lưu file : Url tương đói => tuyệt đói
                var urlTuongdoi = "/images/";
                var urlTuyetDoi = Server.MapPath(urlTuongdoi);// Lấy đường dẫn lưu file trên server

                //Check trùng tên file => Đổi tên file  = tên file cũ (ko kèm đuôi)
                //Ảnh.jpg = > ảnh + "-" + 1 + ".jpg" => ảnh-1.jpg

                string fullDuongDan = urlTuyetDoi + img.FileName;


                int i = 1;
                while (System.IO.File.Exists(fullDuongDan) == true)
                {
                    // 1. Tách tên và đuôi 
                    var ten = Path.GetFileNameWithoutExtension(img.FileName);
                    var duoi = Path.GetExtension(img.FileName);
                    // 2. Sử dụng biến i để chạy và cộng vào tên file mới
                    fullDuongDan = urlTuyetDoi + ten + "-" + i + duoi;
                    i++;
                    // 3. Check lại 
                }
                #endregion
                //Lưu file (Kiểm tra trùng file)
                img.SaveAs(fullDuongDan);
                dt.Image = urlTuongdoi + Path.GetFileName(fullDuongDan);



            }
          
        }

        //DetailTour

        public ActionResult ListDetailTour(string id)
        {
            Session["tempdata"] = id;
            return View(db.DetailTours.Where(s => s.IdTour == id).ToList());
        }

        public ActionResult CreateDetailTour(string id)
        {
            ViewBag.IdHotel = new SelectList(db.Hotels, "IdHotel", "NameHotel");
            ViewBag.IdTrans = new SelectList(db.Transports, "IdTrans", "NameTrans");
            ViewBag.IdVistLocat = new SelectList(db.VistLocations, "IdVistLocat", "NameVist");
            ViewBag.idtour = id;

            return View();
        }
        [HttpPost]
        public ActionResult CreateDetailTour(string id,DetailTour detailTour,HttpPostedFileBase Imager)
        {
            ViewBag.IdHotel = new SelectList(db.Hotels, "IdHotel", "NameHotel");
            ViewBag.IdTrans = new SelectList(db.Transports, "IdTrans", "NameTrans");
            ViewBag.IdVistLocat = new SelectList(db.VistLocations, "IdVistLocat", "NameVist");
            if (!ModelState.IsValid)
            {
                return View(detailTour);
            }
            SaveImagerDetail(detailTour, Imager);
            Tour tour = db.Tours.Where(s => s.IdTour == id).FirstOrDefault();

            Random rd = new Random();
            var idDetail = "DT" + rd.Next(1, 1000);
            detailTour.IdSchedule = idDetail;

            detailTour.IdTour = (string)Session["tempdata"];
            
            var c= (string)Session["tempdata"]; 
            db.DetailTours.Add(detailTour);
            db.SaveChanges();
            return RedirectToAction("ListDeTailTour", new RouteValueDictionary(
                                   new { controller = "Tourmanager", action = "ListDetailTour", Id = c }));


        }

        public ActionResult EditDetailTour(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DetailTour dt = db.DetailTours.Find(id);
            if (dt == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdHotel = new SelectList(db.Hotels, "IdHotel", "NameHotel", dt.IdHotel);
            ViewBag.IdTrans = new SelectList(db.Transports, "IdTrans", "NameTrans", dt.IdTrans);
            ViewBag.IdVistLocat = new SelectList(db.VistLocations, "IdVistLocat", "NameVist", dt.IdVistLocat);
            

            return View(dt);
        }
        [HttpPost]
        public ActionResult EditDetailTour(string id, HttpPostedFileBase Imager, DetailTour dt)
        {
            SaveImagerDetail(dt, Imager);
            //LuuImage(tour, ImagerTour);
            db.Entry(dt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["noti"] = "oke";
            ViewBag.IdHotel = new SelectList(db.Hotels, "IdHotel", "NameHotel",dt.IdHotel);
            ViewBag.IdTrans = new SelectList(db.Transports, "IdTrans", "NameTrans",dt.IdTrans);
            ViewBag.IdVistLocat = new SelectList(db.VistLocations, "IdVistLocat", "NameVist",dt.IdVistLocat);
            

            var c = (string)Session["tempdata"];
            return RedirectToAction("ListDeTailTour", new RouteValueDictionary(
                                   new { controller = "Tourmanager", action = "ListDetailTour", Id = c }));
        }

        public ActionResult DetailTourDetail(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetailTour dt = db.DetailTours.Where(s => s.IdSchedule == id).FirstOrDefault();
            if (dt == null)
            {
                return HttpNotFound();
            }
            return View(dt);
        }

        public ActionResult DeleteDetailTour(string id, DetailTour dt)
        {
            
            dt = db.DetailTours.Where(s => s.IdSchedule == id).FirstOrDefault();
            db.DetailTours.Remove(dt);
            TempData["messageAlert"] = "Đã xóa staff";
            db.SaveChanges();
            var c = (string)Session["tempdata"];
            return RedirectToAction("ListDeTailTour", new RouteValueDictionary(
                                   new { controller = "Tourmanager", action = "ListDetailTour", Id = c }));

        }





        
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        // GET: Staffs/Details/5
      

    }
}