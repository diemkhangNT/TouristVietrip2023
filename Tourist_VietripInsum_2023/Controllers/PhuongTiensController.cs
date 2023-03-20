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
    public class PhuongTiensController : Controller
    {
        private TouristEntities1 db = new TouristEntities1();

        // GET: PhuongTiens
        public ActionResult Index()
        {
            return View(db.PhuongTiens.ToList());
        }

        // GET: PhuongTiens/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhuongTien phuongTien = db.PhuongTiens.Find(id);
            if (phuongTien == null)
            {
                return HttpNotFound();
            }
            return View(phuongTien);
        }

        // GET: PhuongTiens/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PhuongTiens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaPTien,TenPTien,HangXe,ChiTiet")] PhuongTien phuongTien)
        {
            if (ModelState.IsValid)
            {
                db.PhuongTiens.Add(phuongTien);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(phuongTien);
        }

        // GET: PhuongTiens/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhuongTien phuongTien = db.PhuongTiens.Find(id);
            if (phuongTien == null)
            {
                return HttpNotFound();
            }
            return View(phuongTien);
        }

        // POST: PhuongTiens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaPTien,TenPTien,HangXe,ChiTiet")] PhuongTien phuongTien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phuongTien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phuongTien);
        }

        // GET: PhuongTiens/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhuongTien phuongTien = db.PhuongTiens.Find(id);
            if (phuongTien == null)
            {
                return HttpNotFound();
            }
            return View(phuongTien);
        }

        // POST: PhuongTiens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            PhuongTien phuongTien = db.PhuongTiens.Find(id);
            db.PhuongTiens.Remove(phuongTien);
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
