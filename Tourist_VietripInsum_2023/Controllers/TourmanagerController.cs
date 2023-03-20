using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
    [AdminAuthorize(idPos = "TM")]
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
            totalTT = db.LoaiTours.ToList().Count;
            TempData["tongLT"] = totalTT;
            return View(db.Tours.ToList());
        }

        //        // GET: Tours
        //        //Trang hiển thị danh sách loại tour, hotel, tour
        public ActionResult QuanLyTour()
        {
            List<Tour> listTour = db.Tours.Include(t => t.Hotel).Include(t => t.LoaiTour).ToList();
            //var tours = db.Tours.ToList().OrderByDescending(s => s.IdTour);
            return View(listTour);
        }

        //        //search view
        [HttpPost]
        public ActionResult QuanLyTour(string option, string search)
        {

            //if a user choose the radio button option as Subject  
            if (option == "IDTour")
            {
                //Index action method will return a view with a student records based on what a user specify the value in textbox  
                return View(db.Tours.Where(x => x.MaTour.StartsWith(search) || search == null).ToList());
            }
            else if (option == "Tourname")
            {
                return View(db.Tours.Where(x => x.TenTour.StartsWith(search) || search == null).ToList());
            }
            else
            {
                return View(db.Tours.Where(x => x.SoChoNull.Equals(search) || search == null).ToList());
            }
        }

        //Hotel
        public ActionResult HotelManager()
        {
            var hotels = db.Hotels.Include(h => h.TinhThanh);
            return View(hotels.ToList());
        }

        // GET: Hotels/Create
        public ActionResult CreateHotel()
        {
            ViewBag.MaTinh = new SelectList(db.TinhThanhs, "MaTinh", "TenTinh");
            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHotel([Bind(Include = "MaKS,MaTinh,TenKS,Sao")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                hotel.MaKS = "KS" + rd.Next(1000);
                db.Hotels.Add(hotel);
                db.SaveChanges();
                return RedirectToAction("HotelManager");
            }

            ViewBag.MaTinh = new SelectList(db.TinhThanhs, "MaTinh", "TenTinh", hotel.MaTinh);
            return View(hotel);
        }

        // GET: Hotels/Edit/5
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
            ViewBag.MaTinh = new SelectList(db.TinhThanhs, "MaTinh", "TenTinh", hotel.MaTinh);
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHotel([Bind(Include = "MaKS,MaTinh,TenKS,Sao")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hotel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HotelManager");
            }
            ViewBag.MaTinh = new SelectList(db.TinhThanhs, "MaTinh", "TenTinh", hotel.MaTinh);
            return View(hotel);
        }

        [HttpPost]
        public ActionResult DeleteHotel(string id, Hotel ht)
        {
            ht = db.Hotels.Where(s => s.MaKS == id).FirstOrDefault();
            var detail = db.Tours.ToList();
            var count = 0;
            for (int i = 0; i < detail.Count; i++)
            {
                if (ht.MaKS == detail[i].MaKS)
                {
                    count++;
                }
            }
            if (count > 0)
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
        //End hotel

        // Phuong tien
        public ActionResult CreateTrans()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTrans(PhuongTien trans)
        {
            if (!ModelState.IsValid)
            {
                return View(trans);
            }
            Random rd = new Random();
            var idTrans = "PT" + rd.Next(1, 1000);
            trans.MaPTien = idTrans;

            db.PhuongTiens.Add(trans);
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
            PhuongTien transport = db.PhuongTiens.Find(id);
            if (transport == null)
            {
                return HttpNotFound();
            }
            return View(transport);
        }

        [HttpPost]
        public ActionResult EditTrans(PhuongTien transport)
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
        public ActionResult DeleteTrans(string id, PhuongTien trans)
        {

            trans = db.PhuongTiens.Where(s => s.MaPTien == id).FirstOrDefault();
            var detail = db.ChiTietTours.ToList();
            var count = 0;
            for (int i = 0; i < detail.Count; i++)
            {
                if (trans.MaPTien == detail[i].MaPTien)
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
                db.PhuongTiens.Remove(trans);
                db.SaveChanges();
                return RedirectToAction("HotelManager");
            }

            return View(trans);
        }
        //end phuong tien








        //Chi tiết loại tour
        public ActionResult TourTypeDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiTour tourType = db.LoaiTours.Find(id);
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
        public ActionResult CreateTourType([Bind(Include = "MaLTour,TenLTour,ChietKhau")] LoaiTour tourType)
        {
            if (ModelState.IsValid)
            {
                db.LoaiTours.Add(tourType);
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
            LoaiTour tourType = db.LoaiTours.Find(id);
            if (tourType == null)
            {
                return HttpNotFound();
            }
            return View(tourType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTourType([Bind(Include = "MaLTour,TenLTour,ChietKhau")] LoaiTour tourType)
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
            LoaiTour tourType = db.LoaiTours.Find(id);
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
            LoaiTour tourType = db.LoaiTours.Find(id);
            db.LoaiTours.Remove(tourType);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");
        }

        //        //Chi tiết khách sạn
        //public ActionResult HotelDetails(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Hotel hotel = db.Hotels.Find(id);
        //    if (hotel == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(hotel);
        //}

        ////        //Tạo khách sạn
        //[HttpGet]
        //public ActionResult CreateHotel()
        //{
        //    return View();
        //}

        ////public JsonResult AddHotel(Hotel hotel, string namehotel, string id, string leveltour)
        ////{

        ////    Random rd = new Random();
        ////    var idHotel = "H" + rd.Next(1, 1000);
        ////    hotel.MaKS = idHotel;
        ////    hotel.TenKS = namehotel;
        ////    hotel.Sao = leveltour;
        ////    //hotel.AddressHotel = staddress + ", " + districtaddress + ", " + cityaddress;

        ////    db.Hotels.Add(hotel);
        ////    db.SaveChanges();

        ////    return new JsonResult("save");
        ////}
        //[HttpPost]
        //public ActionResult CreateHotel(Hotel hotel, string cityaddress, string districtaddress, string staddress)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(hotel);
        //    }
        //    Random rd = new Random();
        //    var idHotel = "H" + rd.Next(1, 1000);
        //    hotel.MaKS = idHotel;



        //    hotel.TenKS = staddress + ", " + districtaddress + ", " + cityaddress;

        //    db.Hotels.Add(hotel);
        //    TempData["noti"] = "add";
        //    db.SaveChanges();
        //    return RedirectToAction("HotelManager");

        //}

        ////Sửa hotel
        //public ActionResult EditHotel(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Hotel hotel = db.Hotels.Find(id);
        //    if (hotel == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(hotel);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditHotel([Bind(Include = "MaKS")] Hotel hotel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        TempData["noti"] = "edit";
        //        db.Entry(hotel).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("HotelManager");
        //    }
        //    return View(hotel);
        //}

        ////Xóa khách sạn
        //public ActionResult DetailHotel(string id)
        //{
        //    return View(db.Hotels.Where(s => s.MaKS == id).FirstOrDefault());
        //}





        //        //// POST: Hotels/Delete/5
        //        //[HttpPost, ActionName("DeleteHotel")]
        //        //[ValidateAntiForgeryToken]
        //        //public ActionResult DeleteHotelConfirmed(string id)
        //        //{
        //        //    Hotel hotel = db.Hotels.Find(id);
        //        //    db.Hotels.Remove(hotel);
        //        //    db.SaveChanges();
        //        //    return RedirectToAction("QuanLyTour");
        //        //}

        //        //Chi tiết địa điểm tham quan

        //public ActionResult TransportManager()
        //{
        //    var ts = db.PhuongTiens.ToList().OrderByDescending(s => s.MaPTien);
        //    return View(ts.ToList());
        //}








        //public ActionResult DetailTrans(string id)
        //{
        //    return View(db.Transports.Where(s => s.IdTrans == id).FirstOrDefault());
        //}







        //public ActionResult LocationManager()
        //{
        //    var lc = db.VistLocations.ToList().OrderByDescending(s => s.IdVistLocat);
        //    return View(lc.ToList());
        //}


        //public ActionResult VistLocationsDetails(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    VistLocation vistLocation = db.VistLocations.Find(id);
        //    if (vistLocation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(vistLocation);
        //}

        //        //Tạo địa điểm tham quan
        //        public ActionResult CreateVisitLocations()
        //        {
        //            return View();
        //        }

        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public ActionResult CreateVisitLocations([Bind(Include = "IdVistLocat,NameVist,ImageLocation,Des_Location,Loca_address")] VistLocation vistLocation)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                db.VistLocations.Add(vistLocation);
        //                db.SaveChanges();
        //                return RedirectToAction("LocationManager");
        //            }
        //            return View(vistLocation);
        //        }

        //        //Chỉnh sửa địa điểm tham quan
        //        public ActionResult EditVisitLocations(string id)
        //        {
        //            if (id == null)
        //            {
        //                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //            }
        //            VistLocation vistLocation = db.VistLocations.Find(id);
        //            if (vistLocation == null)
        //            {
        //                return HttpNotFound();
        //            }
        //            return View(vistLocation);
        //        }
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public ActionResult EditVisitLocations([Bind(Include = "IdVistLocat,NameVist,ImageLocation,Des_Location,Loca_address")] VistLocation vistLocation)
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                db.Entry(vistLocation).State = EntityState.Modified;
        //                db.SaveChanges();
        //                return RedirectToAction("LocationManager");
        //            }
        //            return View(vistLocation);
        //        }

        //        //Xóa địa điểm tham quan
        //        public ActionResult DeleteVisitLocations(string id)
        //        {
        //            if (id == null)
        //            {
        //                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //            }
        //            VistLocation vistLocation = db.VistLocations.Find(id);
        //            if (vistLocation == null)
        //            {
        //                return HttpNotFound();
        //            }
        //            return View(vistLocation);
        //        }





        //        public ActionResult DeleteManager(string id)
        //        {
        //            return View();
        //        }

        //        //tour


        public ActionResult CreateTours()
        {
            ViewBag.MaKS = new SelectList(db.Hotels, "MaKS", "TenKS");
            ViewBag.MaLTour = new SelectList(db.LoaiTours, "MaLTour", "TenLTour");
            return View();
        }

        public void LuuImage(Tour t, HttpPostedFileBase ImagerTour)
        {
            #region Hình ảnh

            if (ImagerTour == null)
            {
                t.HinhMinhHoa_T = "/images/profile-user.png";
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
                t.HinhMinhHoa_T = urlTuongdoi + Path.GetFileName(fullDuongDan);
            }
        }

        [HttpPost]
        public ActionResult CreateTours(Tour tour, HttpPostedFileBase ImagerTour)
        {
            if (!ModelState.IsValid)
            {
                return View(tour);
            }
            LuuImage(tour, ImagerTour);
            Random rd = new Random();
            var idtour = "Tour" + rd.Next(1, 1000);
            tour.MaTour = idtour;
            tour.TourNoiBat = true;

            //TimeSpan? songay = tour.NgayTroVe - tour.NgayKhoihanh;
            //tour.SoNgay = Convert.ToInt32(songay);
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
            Tour tour = db.Tours.Where(s => s.MaTour == id).FirstOrDefault();
            if (tour == null)
            {
                return HttpNotFound();
            }
            return View(tour);
        }

        [HttpGet]
        public ActionResult EditTour(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tour tour = db.Tours.Find(id);
            if (tour == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaKS = new SelectList(db.Hotels, "MaKS", "TenKS", tour.Hotel.TenKS);
            ViewBag.MaLTour = new SelectList(db.LoaiTours, "MaLTour", "TenLTour", tour.LoaiTour.TenLTour);
            return View(tour);
        }
        [HttpPost]
        public ActionResult EditTour(string id, HttpPostedFileBase ImagerTour, Tour tour)
        {
            LuuImage(tour, ImagerTour);
            db.Entry(tour).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["noti"] = "oke";
            return RedirectToAction("QuanLyTour");
        }
        //public ActionResult EditTour([Bind(Include = "MaTour,MaLTour,MaKS,TenTour,GioiThieu,HinhMinhHoa_T,NgayKhoihanh,NgayTroVe,SoNgay,NoiKhoiHanh,SoChoNull,GiaTreEm,GiaNguoiLon,TourNoiBat,HanChotDatVe")] Tour tour)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(tour).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MaKS = new SelectList(db.Hotels, "MaKS", "MaTinh", tour.MaKS);
        //    ViewBag.MaLTour = new SelectList(db.LoaiTours, "MaLTour", "TenLTour", tour.MaLTour);
        //    return View(tour);
        //}


        //        public ActionResult Demo()
        //        {
        //            return View();
        //        }
        public ActionResult DeleteTour(string id, Tour tour)
        {
            tour = db.Tours.Where(s => s.MaTour == id).FirstOrDefault();
            var detail = db.ChiTietTours.ToList();
            for (int i = 0; i < detail.Count; i++)
            {
                if (tour.MaTour == detail[i].MaTour)
                {
                    db.ChiTietTours.Remove(detail[i]);
                }
            }
            db.Tours.Remove(tour);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");

        }

        //        //public JsonResult DeleteTour(string id)
        //        //{

        //        //    bool result;
        //        //    Tour tour = db.Tours.Where(s => s.IdTour == id).FirstOrDefault();
        //        //    DetailTour detailTour = db.DetailTours.Where(s => s.IdTour == id).FirstOrDefault();
        //        //    if (tour != null)
        //        //    {
        //        //        db.Tours.Remove(tour);
        //        //        if (detailTour != null)
        //        //        {
        //        //            db.DetailTours.Remove(detailTour);
        //        //        }


        //        //        db.SaveChanges();
        //        //        result = true;
        //        //    }
        //        //    else
        //        //    {
        //        //        result = false;
        //        //    }


        //        //    return Json(result, JsonRequestBehavior.AllowGet);



        //        //}

        //public void SaveImagerDetail(ChiTietTour dt, HttpPostedFileBase img)
        //{
        //    #region Hình ảnh

        //    if (img == null)
        //    {
        //        dt. = "/images/profile-user.png";
        //    }
        //    else
        //    {
        //        //Xác định đường dẫn lưu file : Url tương đói => tuyệt đói
        //        var urlTuongdoi = "/images/";
        //        var urlTuyetDoi = Server.MapPath(urlTuongdoi);// Lấy đường dẫn lưu file trên server

        //        //Check trùng tên file => Đổi tên file  = tên file cũ (ko kèm đuôi)
        //        //Ảnh.jpg = > ảnh + "-" + 1 + ".jpg" => ảnh-1.jpg

        //        string fullDuongDan = urlTuyetDoi + img.FileName;


        //        int i = 1;
        //        while (System.IO.File.Exists(fullDuongDan) == true)
        //        {
        //            // 1. Tách tên và đuôi 
        //            var ten = Path.GetFileNameWithoutExtension(img.FileName);
        //            var duoi = Path.GetExtension(img.FileName);
        //            // 2. Sử dụng biến i để chạy và cộng vào tên file mới
        //            fullDuongDan = urlTuyetDoi + ten + "-" + i + duoi;
        //            i++;
        //            // 3. Check lại 
        //        }
        //        #endregion
        //        //Lưu file (Kiểm tra trùng file)
        //        img.SaveAs(fullDuongDan);
        //        dt.Image = urlTuongdoi + Path.GetFileName(fullDuongDan);



        //    }

        //}

        //DetailTour

        public ActionResult ListDetailTour(string id)
        {
            Session["tempdata"] = id;
            return View(db.ChiTietTours.Where(s => s.MaTour == id).ToList());
        }

        public ActionResult CreateDetailTour(string id)
        {
            ViewBag.IdHotel = new SelectList(db.Hotels, "IdHotel", "NameHotel");
            ViewBag.IdTrans = new SelectList(db.PhuongTiens, "IdTrans", "NameTrans");
            ViewBag.IdVistLocat = new SelectList(db.TinhThanhs, "IdVistLocat", "NameVist");
            ViewBag.idtour = id;

            return View();
        }
        [HttpPost]
        public ActionResult CreateDetailTour(string id, ChiTietTour detailTour, HttpPostedFileBase Imager)
        {
            ViewBag.IdHotel = new SelectList(db.Hotels, "IdHotel", "NameHotel");
            ViewBag.IdTrans = new SelectList(db.PhuongTiens, "IdTrans", "NameTrans");
            ViewBag.IdVistLocat = new SelectList(db.TinhThanhs, "IdVistLocat", "NameVist");
            if (!ModelState.IsValid)
            {
                return View(detailTour);
            }
            //SaveImagerDetail(detailTour, Imager);
            Tour tour = db.Tours.Where(s => s.MaTour == id).FirstOrDefault();

            Random rd = new Random();
            var idDetail = rd.Next(1, 1000);
            detailTour.STT = idDetail;

            detailTour.MaTour = (string)Session["tempdata"];

            var c = (string)Session["tempdata"];
            db.ChiTietTours.Add(detailTour);
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
            ChiTietTour dt = db.ChiTietTours.Find(id);
            if (dt == null)
            {
                return HttpNotFound();
            }
            //ViewBag.IdHotel = new SelectList(db.Hotels, "IdHotel", "NameHotel", dt.);
            ViewBag.IdTrans = new SelectList(db.PhuongTiens, "IdTrans", "NameTrans", dt.MaPTien);
            ViewBag.IdVistLocat = new SelectList(db.DiaDiemThamQuans, "IdVistLocat", "NameVist", dt.MaDDTQ);


            return View(dt);
        }
        [HttpPost]
        public ActionResult EditDetailTour(string id, HttpPostedFileBase Imager, ChiTietTour dt)
        {
            //SaveImagerDetail(dt, Imager);
            //LuuImage(tour, ImagerTour);
            db.Entry(dt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["noti"] = "oke";
            //ViewBag.IdHotel = new SelectList(db.Hotels, "IdHotel", "NameHotel", dt.IdHotel);
            ViewBag.IdTrans = new SelectList(db.PhuongTiens, "IdTrans", "NameTrans", dt.MaPTien);
            ViewBag.IdVistLocat = new SelectList(db.DiaDiemThamQuans, "IdVistLocat", "NameVist", dt.MaDDTQ);


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
            ChiTietTour dt = db.ChiTietTours.Where(s => s.MaDDTQ == id).FirstOrDefault();
            if (dt == null)
            {
                return HttpNotFound();
            }
            return View(dt);
        }

        public ActionResult DeleteDetailTour(string id, ChiTietTour dt)
        {

            dt = db.ChiTietTours.Where(s => s.MaDDTQ == id).FirstOrDefault();
            db.ChiTietTours.Remove(dt);
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
        public ActionResult Profile(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien staff = db.NhanViens.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }



    }
}

