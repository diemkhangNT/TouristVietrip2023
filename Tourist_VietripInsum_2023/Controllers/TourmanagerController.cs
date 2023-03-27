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
using System.Windows.Controls;
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
            var totalT = 0;
            totalT = db.Tours.ToList().Count;
            TempData["tongTour"] = totalT;
            var totalTT = 0;
            totalTT = db.LoaiTours.ToList().Count;
            TempData["tongLT"] = totalTT;
            var totalDD = 0;
            totalDD = db.DiaDiemThamQuans.ToList().Count;
            TempData["tongDD"] = totalDD;
            var totalTinh = 0;
            totalTinh = db.TinhThanhs.ToList().Count;
            TempData["tongTinh"] = totalTinh;
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
        //---------------------------------------------------------------//
        // Dia diem tham quan
        public ActionResult LocationManager()
        {
            var lc = db.DiaDiemThamQuans.Include(d => d.TinhThanh);
            return View(lc.ToList());
        }


        public ActionResult VistLocationsDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiaDiemThamQuan diaDiemThamQuan = db.DiaDiemThamQuans.Find(id);
            if (diaDiemThamQuan == null)
            {
                return HttpNotFound();
            }
            return View(diaDiemThamQuan);
        }

        //Tạo địa điểm tham quan
        public ActionResult CreateVisitLocations()
        {
            ViewBag.MaTinh = new SelectList(db.TinhThanhs, "MaTinh", "TenTinh");
            return View();
        }
        public void LuuImageVis(DiaDiemThamQuan t, HttpPostedFileBase ImagerVis)
        {
            #region Hình ảnh

            if (ImagerVis == null)
            {
                t.HinhMinhHoa_D = t.HinhMinhHoa_D;
            }
            else
            {

                //Xác định đường dẫn lưu file : Url tương đói => tuyệt đói
                var urlTuongdoi = "/images/";
                var urlTuyetDoi = Server.MapPath(urlTuongdoi);// Lấy đường dẫn lưu file trên server

                //Check trùng tên file => Đổi tên file  = tên file cũ (ko kèm đuôi)
                //Ảnh.jpg = > ảnh + "-" + 1 + ".jpg" => ảnh-1.jpg

                string fullDuongDan = urlTuyetDoi + ImagerVis.FileName;


                int i = 1;
                while (System.IO.File.Exists(fullDuongDan) == true)
                {
                    // 1. Tách tên và đuôi 
                    var ten = Path.GetFileNameWithoutExtension(ImagerVis.FileName);
                    var duoi = Path.GetExtension(ImagerVis.FileName);
                    // 2. Sử dụng biến i để chạy và cộng vào tên file mới
                    fullDuongDan = urlTuyetDoi + ten + "-" + i + duoi;
                    i++;
                    // 3. Check lại 
                }
                #endregion
                //Lưu file (Kiểm tra trùng file)
                ImagerVis.SaveAs(fullDuongDan);
                t.HinhMinhHoa_D = urlTuongdoi + Path.GetFileName(fullDuongDan);
            }
        }
        [HttpPost]
        public ActionResult CreateVisitLocations(DiaDiemThamQuan diaDiemThamQuan, HttpPostedFileBase ImagerVis)
        {
            if (ModelState.IsValid)
            {
                LuuImageVis(diaDiemThamQuan, ImagerVis);
                Random rd = new Random();
                var idloc = "DD" + rd.Next(1, 1000);
                diaDiemThamQuan.MaDDTQ = idloc;

                db.DiaDiemThamQuans.Add(diaDiemThamQuan);
                db.SaveChanges();
                TempData["noti"] = "add";
                return RedirectToAction("LocationManager");
            }

            ViewBag.MaTinh = new SelectList(db.TinhThanhs, "MaTinh", "TenTinh", diaDiemThamQuan.MaTinh);
            return View(diaDiemThamQuan);
        }

        //Chỉnh sửa địa điểm tham quan
        public ActionResult EditVisitLocations(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiaDiemThamQuan diaDiemThamQuan = db.DiaDiemThamQuans.Find(id);
            if (diaDiemThamQuan == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaTinh = new SelectList(db.TinhThanhs, "MaTinh", "TenTinh", diaDiemThamQuan.MaTinh);
            return View(diaDiemThamQuan);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditVisitLocations(DiaDiemThamQuan diaDiemThamQuan, HttpPostedFileBase ImagerVis)
        {
            if (ModelState.IsValid)
            {
                LuuImageVis(diaDiemThamQuan, ImagerVis);
                db.Entry(diaDiemThamQuan).State = EntityState.Modified;
                db.SaveChanges();
                TempData["noti"] = "edit";
                return RedirectToAction("LocationManager");
            }
            ViewBag.MaTinh = new SelectList(db.TinhThanhs, "MaTinh", "TenTinh", diaDiemThamQuan.MaTinh);
            return View(diaDiemThamQuan);
        }

        //Xóa địa điểm tham quan
        public ActionResult DeleteVisitLocations(string id, DiaDiemThamQuan diaDiemThamQuan)
        {
            diaDiemThamQuan = db.DiaDiemThamQuans.Where(s => s.MaDDTQ == id).FirstOrDefault();
            List<ChiTietTour> detail = db.ChiTietTours.ToList();
            var count = 0;
            foreach(var ct in detail )
            {
                if (diaDiemThamQuan.MaDDTQ == ct.MaDDTQ)
                {
                    count++;
                }
            }
            if (count > 0)
            {
                TempData["noti"] = "deletetrans-false";
                return RedirectToAction("LocationManager");
            }
            else
            {
                TempData["noti"] = "deletetrans-true";
                db.DiaDiemThamQuans.Remove(diaDiemThamQuan);
                db.SaveChanges();
                return RedirectToAction("LocationManager");
            }
            return View(diaDiemThamQuan);
        }

        //end địa điểm tham quan






        // GET: LoaiTours
        public ActionResult IndexLoaiTour()
        {
            return View(db.LoaiTours.ToList());
        }
        // GET: LoaiTours/Create
        public ActionResult CreateLoaiTour()
        {
            return View();
        }

        // POST: LoaiTours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLoaiTour([Bind(Include = "MaLTour,TenLTour,ChietKhau")] LoaiTour loaiTour)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var idltour = "LT" + rd.Next(1, 100000);
                loaiTour.MaLTour = idltour;
                db.LoaiTours.Add(loaiTour);
                db.SaveChanges();
                return RedirectToAction("IndexLoaiTour");
            }

            return View(loaiTour);
        }

        // GET: LoaiTours/Edit/5
        public ActionResult EditLoaiTour(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiTour loaiTour = db.LoaiTours.Find(id);
            if (loaiTour == null)
            {
                return HttpNotFound();
            }
            return View(loaiTour);
        }

        // POST: LoaiTours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLoaiTour([Bind(Include = "MaLTour,TenLTour,ChietKhau")] LoaiTour loaiTour)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loaiTour).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexLoaiTour");
            }
            return View(loaiTour);
        }

        // GET: LoaiTours/Delete/5
        public ActionResult DeleteLoaiTour(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiTour loaiTour = db.LoaiTours.Find(id);
            if (loaiTour == null)
            {
                return HttpNotFound();
            }
            return View(loaiTour);
        }

        // POST: LoaiTours/Delete/5
        [HttpPost, ActionName("DeleteLoaiTour")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLoaiTourConfirmed(string id)
        {
            LoaiTour loaiTour = db.LoaiTours.Find(id);
            db.LoaiTours.Remove(loaiTour);
            db.SaveChanges();
            return RedirectToAction("IndexLoaiTour");
        }



        public ActionResult CreateTours()
        {
            ViewBag.MaKS = new SelectList(db.Hotels, "MaKS", "TenKS");
            ViewBag.MaLTour = new SelectList(db.LoaiTours, "MaLTour", "TenLTour");
            //List<int> a = new List<int>();
            //ViewBag.MaKSforTinh = a;
            //DateTime today = DateTime.Today;
            // Set giá trị nhỏ nhất của datepicker là ngày hôm nay
            
            //DatePicker datePicker = new DatePicker();
            //datePicker.SetValue()= today;
            return View();
        }
        
        [HttpGet]
        public ActionResult GetValue(string value)
        {
            List<string> abc = new List<string>();
            ViewBag.Value = value;
            //var matinh = value;
            //foreach (var item in db.Hotels.ToList())
            //{
            //    if (item.MaTinh == matinh)
            //    {
            //        abc.Add(item.TenKS);
            //    }
            //}
            //ViewBag.MaKSforTinh = abc;
            return Content(value);
        }
        public void LuuImage(Tour t, HttpPostedFileBase ImagerTour)
        {
            #region Hình ảnh

            if (ImagerTour == null)
            {
                t.HinhMinhHoa_T = t.HinhMinhHoa_T;
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
            if(tour.TenTour == null || tour.GioiThieu ==null||tour.NgayKhoihanh==null ||tour.NgayTroVe==null||tour.NoiKhoiHanh==null || tour.SoChoNull == null || tour.HanChotDatVe == null || tour.GiaTreEm == null || tour.GiaNguoiLon == null)
            {
                TempData["noti"] = "null";
                return RedirectToAction("CreateTours");
            }
            else
            {
                LuuImage(tour, ImagerTour);
                Random rd = new Random();
                var idtour = "VNG" + rd.Next(1, 100000);
                tour.MaTour = idtour;
                tour.TrangThai  = "Coming soon...";
                DateTime startDate = (DateTime)tour.NgayKhoihanh;
                DateTime endDate = (DateTime)tour.NgayTroVe;

                TimeSpan span = endDate.Subtract(startDate);
                int numOfDays = (int)span.TotalDays + 1;

                tour.SoNgay = numOfDays;
                db.Tours.Add(tour);
                db.SaveChanges();
                TempData["noti"] = "oke";
                return RedirectToAction("QuanLyTour");
            }
            return View();
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
            DateTime startDate = (DateTime)tour.NgayKhoihanh;
            DateTime endDate = (DateTime)tour.NgayTroVe;

            TimeSpan span = endDate.Subtract(startDate);
            int numOfDays = (int)span.TotalDays + 1;
            db.SaveChanges();
            TempData["noti"] = "oke";
            return RedirectToAction("QuanLyTour");
        }
        
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

        public ActionResult ListDetailTour(string id)
        {
            Session["tempdata"] = id;
           // EditDetailTour(id, (string)Session["tempdata"]);
            var ctt = db.ChiTietTours.Where(s => s.MaTour == id).ToList();
            return View(ctt.OrderBy(s => s.STT));
        }

        public ActionResult CreateDetailTour(string id)
        {
            Session["maTour"] = id;
            ViewBag.MaDDTQ = new SelectList(db.DiaDiemThamQuans, "MaDDTQ", "TenDDTQ");
            ViewBag.MaPTien = new SelectList(db.PhuongTiens, "MaPTien", "TenPTien");
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "MaLTour");
            ViewBag.idtour = id;
            
            return View();
        }
        [HttpPost]
        public ActionResult CreateDetailTour([Bind(Include = "STT,MaDDTQ,MaTour,MaPTien,TieuDe,MotaChitiet")] ChiTietTour detailTour)
        {
            ViewBag.MaDDTQ = new SelectList(db.DiaDiemThamQuans, "MaDDTQ", "TenDDTQ", detailTour.MaDDTQ);
            ViewBag.MaPTien = new SelectList(db.PhuongTiens, "MaPTien", "TenPTien", detailTour.MaPTien);
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "MaLTour", detailTour.MaTour);
            if (!ModelState.IsValid)
            {
                return View();
            }
            //SaveImagerDetail(detailTour, Imager);
            //Tour tour = db.Tours.Where(s => s.MaTour == (string)Session["maTour"]).FirstOrDefault();


            detailTour.MaTour = (string)Session["maTour"];
            detailTour.MaCTT = String.Concat(detailTour.MaTour, detailTour.MaDDTQ);
            var c = (string)Session["maTour"];
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
            var chiTietTour = db.ChiTietTours.Where(s => s.MaCTT == id).FirstOrDefault() ;
            if (chiTietTour == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDDTQ = new SelectList(db.DiaDiemThamQuans, "MaDDTQ", "TenDDTQ", chiTietTour.MaDDTQ);
            ViewBag.MaPTien = new SelectList(db.PhuongTiens, "MaPTien", "TenPTien", chiTietTour.MaPTien);
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "MaLTour", chiTietTour.MaTour);
            return View(chiTietTour);
            
        }
        [HttpPost]
        public ActionResult EditDetailTour(ChiTietTour dt)
        {
            //SaveImagerDetail(dt, Imager);
            //LuuImage(tour, ImagerTour);

            dt.MaCTT = String.Concat(dt.MaTour, dt.MaDDTQ);
            db.Entry(dt).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["noti"] = "oke";
            //ViewBag.IdHotel = new SelectList(db.Hotels, "IdHotel", "NameHotel", dt.IdHotel);
            ViewBag.MaDDTQ = new SelectList(db.DiaDiemThamQuans, "MaDDTQ", "TenDDTQ", dt.MaDDTQ);
            ViewBag.MaPTien = new SelectList(db.PhuongTiens, "MaPTien", "TenPTien", dt.MaPTien);
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "MaLTour", dt.MaTour);

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
            //var detail = db.ChiTietTours.ToList();
            //for (int i = 0; i < detail.Count; i++)
            //{
            //    if (dt.MaTour == detail[i].MaTour)
            //    {
            //        db.ChiTietTours.Remove(detail[i]);
            //    }
            //}
            db.ChiTietTours.Remove(dt);
            db.SaveChanges();
            return RedirectToAction("ListDeTailTour", new RouteValueDictionary(
                                   new { controller = "Tourmanager", action = "ListDetailTour", Id = dt.MaTour }));

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}

