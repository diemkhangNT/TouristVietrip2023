using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    public class ChiTietToursController : Controller
    {
        private TouristEntities1 db = new TouristEntities1();

        // GET: ChiTietTours
        public ActionResult Index()
        {
            var chiTietTours = db.ChiTietTours.Include(c => c.DiaDiemThamQuan).Include(c => c.PhuongTien).Include(c => c.Tour);
            return View(chiTietTours.ToList());
        }

        // GET: ChiTietTours/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietTour chiTietTour = db.ChiTietTours.Find(id);
            if (chiTietTour == null)
            {
                return HttpNotFound();
            }
            return View(chiTietTour);
        }

        // GET: ChiTietTours/Create
        public ActionResult Create()
        {
            ViewBag.MaDDTQ = new SelectList(db.DiaDiemThamQuans, "MaDDTQ", "MaTinh");
            ViewBag.MaPTien = new SelectList(db.PhuongTiens, "MaPTien", "TenPTien");
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "MaLTour");
            return View();
        }

        // POST: ChiTietTours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "STT,MaDDTQ,MaTour,MaPTien,TieuDe,MotaChitiet")] ChiTietTour chiTietTour)
        {
            if (ModelState.IsValid)
            {
                db.ChiTietTours.Add(chiTietTour);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDDTQ = new SelectList(db.DiaDiemThamQuans, "MaDDTQ", "MaTinh", chiTietTour.MaDDTQ);
            ViewBag.MaPTien = new SelectList(db.PhuongTiens, "MaPTien", "TenPTien", chiTietTour.MaPTien);
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "MaLTour", chiTietTour.MaTour);
            return View(chiTietTour);
        }

        // GET: ChiTietTours/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietTour chiTietTour = db.ChiTietTours.Find(id);
            if (chiTietTour == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDDTQ = new SelectList(db.DiaDiemThamQuans, "MaDDTQ", "MaTinh", chiTietTour.MaDDTQ);
            ViewBag.MaPTien = new SelectList(db.PhuongTiens, "MaPTien", "TenPTien", chiTietTour.MaPTien);
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "MaLTour", chiTietTour.MaTour);
            return View(chiTietTour);
        }

        // POST: ChiTietTours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "STT,MaDDTQ,MaTour,MaPTien,TieuDe,MotaChitiet")] ChiTietTour chiTietTour)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiTietTour).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDDTQ = new SelectList(db.DiaDiemThamQuans, "MaDDTQ", "MaTinh", chiTietTour.MaDDTQ);
            ViewBag.MaPTien = new SelectList(db.PhuongTiens, "MaPTien", "TenPTien", chiTietTour.MaPTien);
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "MaLTour", chiTietTour.MaTour);
            return View(chiTietTour);
        }

        // GET: ChiTietTours/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietTour chiTietTour = db.ChiTietTours.Find(id);
            if (chiTietTour == null)
            {
                return HttpNotFound();
            }
            return View(chiTietTour);
        }

        // POST: ChiTietTours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ChiTietTour chiTietTour = db.ChiTietTours.Find(id);
            db.ChiTietTours.Remove(chiTietTour);
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
