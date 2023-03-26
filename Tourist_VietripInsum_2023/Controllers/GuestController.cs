using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    public class GuestController : Controller
    {
        TouristEntities1 db = new TouristEntities1();
        // GET: Guest
        public ActionResult HomePageGuest()
        {
            return View();
        }

        private List<DiaDiemThamQuan> Lay_DiaDiem()
        {
            List<DiaDiemThamQuan> diaDiemThamQuans = db.DiaDiemThamQuans.ToList();
            List<ChiTietTour> chiTietTours = db.ChiTietTours.ToList();

            var nhanvien = (from s in diaDiemThamQuans

                            join a in chiTietTours on s.MaDDTQ equals a.MaDDTQ
                            group s by s.MaDDTQ into g
                            select new
                            {
                                MaDD = g.FirstOrDefault().MaDDTQ,

                            }).ToList();
            List<DiaDiemThamQuan> d = new List<DiaDiemThamQuan>();

            foreach (var i in nhanvien)
            {
                foreach (var item in diaDiemThamQuans)
                {
                    if (item.MaDDTQ == i.MaDD)
                    {
                        d.Add(item);
                    }
                }

            }
            return d;
        }
        public ActionResult DiaDiemTour()
        {
            List<DiaDiemThamQuan> diadiemtim = Lay_DiaDiem();
            return PartialView(diadiemtim);
        }
        public ActionResult DiaDiemPartial()
        {

            List<DiaDiemThamQuan> diadiemtim = Lay_DiaDiem();

            List<TinhThanh> tinhThanhs = new List<TinhThanh>();

            foreach(var item in diadiemtim)
            {
                var c = db.TinhThanhs.Where(t => t.MaTinh == item.MaTinh).FirstOrDefault();
                tinhThanhs.Add(c);
            }    
            
            return PartialView(tinhThanhs);
        }

        public ActionResult Tourinfomation(string id)
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

        //List tour tinh thanh

       
        private List<Tour> lay_Tour(string id)
        {
            List<Tour> tour = new List<Tour>();
            List<ChiTietTour> ct = db.ChiTietTours.Where(c => c.MaDDTQ == id).ToList();
            foreach (var item in ct)
            {
                var tourchon = db.Tours.Where(t => t.MaTour == item.MaTour).FirstOrDefault();
                tour.Add(tourchon);
            }
            return tour;
        }
       
        public ActionResult ListTourTinhThanh(string id)
        {
            var tinhthanh = db.DiaDiemThamQuans.Where(t => t.MaTinh == id).FirstOrDefault();
            List<Tour> tour = new List<Tour>();
            List<DiaDiemThamQuan> diaDiems = db.DiaDiemThamQuans.Where(c => c.MaTinh == id).ToList();
            

            foreach (var item in diaDiems)
            {
                List<Tour> t = lay_Tour(item.MaDDTQ);
                foreach (var a in t)
                {
                    
                    tour.Add(a);
                }
            }


            return View(tour);
        }




        //dia danh

        public ActionResult ListTour_DiaDiem(string id)
        {
            var iddiadiem = db.DiaDiemThamQuans.Where(d => d.MaDDTQ == id).FirstOrDefault();
            List<Tour> tour = lay_Tour(id);
            
            Session["tendiadiem"] = iddiadiem.TenDDTQ;
            return View(tour);
        }


    }
    }
