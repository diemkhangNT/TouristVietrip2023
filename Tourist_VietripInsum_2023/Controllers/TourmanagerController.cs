using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
            totalT = db.Tours.ToList().Count;
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
        public ActionResult CreateHotel()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHotel([Bind(Include = "IdHotel,LevelHotel,NameHotel,AddressHotel,ImgHotel")] Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                db.Hotels.Add(hotel);
                db.SaveChanges();
                return RedirectToAction("QuanLyTour");
            }

            return View(hotel);
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
                db.Entry(hotel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QuanLyTour");
            }
            return View(hotel);
        }

        //Xóa khách sạn
        public ActionResult DeleteHotel(string id)
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
        // POST: Hotels/Delete/5
        [HttpPost, ActionName("DeleteHotel")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteHotelConfirmed(string id)
        {
            Hotel hotel = db.Hotels.Find(id);
            db.Hotels.Remove(hotel);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");
        }

        //Chi tiết địa điểm tham quan
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
                return RedirectToAction("QuanLyTour");
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
                return RedirectToAction("QuanLyTour");
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

        // POST: VistLocations/Delete/5
        [HttpPost, ActionName("DeleteVisitLocations")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteVisitLocationsConfirmed(string id)
        {
            VistLocation vistLocation = db.VistLocations.Find(id);
            db.VistLocations.Remove(vistLocation);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");
        }
        //Tạo tour
        //public ActionResult CreateTour()
        //{
        //    ViewBag.Id_detailTour = new SelectList(db.Schedules, "IdSchedule", "IdHotel");
        //    ViewBag.Id_TypeTour = new SelectList(db.TourTypes, "IdType", "TypeName");
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateTour([Bind(Include = "IdTour,Id_TypeTour,Id_detailTour,ImagerTour,Departure,ReturnDay,TimeTour,DeparturePlace,NumberAvailable,Price,DeadlineOrder")] Tour tour)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Tours.Add(tour);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Id_detailTour = new SelectList(db.Schedules, "IdSchedule", "IdHotel", tour.Id_detailTour);
        //    ViewBag.Id_TypeTour = new SelectList(db.TourTypes, "IdType", "TypeName", tour.Id_TypeTour);
        //    return View(tour);
        //}

        public ActionResult CreateTours()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult CreateTours(Tour tour)
        {

            Random rd = new Random();
            var idtour = "T" + rd.Next(1, 1000);
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
        public ActionResult CreateSchedule(string id)
        {
            return View(db.Schedules.Where(s => s.IdSchedule == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult CreateSchedule(Schedule schedules, Tour tour, string id)
        {
            tour = db.Tours.Where(s => s.IdTour == id).FirstOrDefault();

            Random rd = new Random();
            var idschedule = "S" + rd.Next(1, 1000);
            schedules.IdSchedule = idschedule;

            var idtour = tour.IdTour;


            db.Schedules.Add(schedules);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");
        }


        //Sửa tour
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
            ViewBag.Id_detailTour = new SelectList(db.Schedules, "IdSchedule", "IdHotel", tour.Id_detailTour);
            ViewBag.Id_TypeTour = new SelectList(db.TourTypes, "IdType", "TypeName", tour.Id_TypeTour);
            return View(tour);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTour([Bind(Include = "IdTour,Id_TypeTour,Id_detailTour,ImagerTour,Departure,ReturnDay,TimeTour,DeparturePlace,NumberAvailable,Price,DeadlineOrder")] Tour tour)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tour).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QuanLyTour");
            }
            ViewBag.Id_detailTour = new SelectList(db.Schedules, "IdSchedule", "IdHotel", tour.Id_detailTour);
            ViewBag.Id_TypeTour = new SelectList(db.TourTypes, "IdType", "TypeName", tour.Id_TypeTour);
            return View(tour);
        }

        //Xóa tour
        public ActionResult DeleteTour(string id)
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
            return View(tour);
        }

        // POST: Tours/Delete/5
        [HttpPost, ActionName("DeleteTour")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTourConfirmed(string id)
        {
            Tour tour = db.Tours.Find(id);
            db.Tours.Remove(tour);
            db.SaveChanges();
            return RedirectToAction("QuanLyTour");
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
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }
        
        

    }
}