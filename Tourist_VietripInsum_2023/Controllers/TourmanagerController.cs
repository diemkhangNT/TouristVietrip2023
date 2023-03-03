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
            return View();
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
                return RedirectToAction("Index");
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
    }
}