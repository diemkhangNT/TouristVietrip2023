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
        public ActionResult DiaDiemPartial()
        {
            var tinhThanhs = db.TinhThanhs.ToList();
            return PartialView(tinhThanhs);
        }

        public ActionResult Tourinfomation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tour tour = db.Tours.Where(s => s.MaTour == id).FirstOrDefault();
            //var diadiem = db.ChiTietTours.Where(s => s.MaTour == id).ToList();
            //List<PhuongTien> pt = new List<PhuongTien>();
            //foreach (var item in diadiem)
            //{
            //    pt.Add(item.MaPTien);


            //}
            if (tour == null)
            {
                return HttpNotFound();
            }
            return View(tour);
        }

        //List tour tinh thanh

        public List<Tour> Laytour(string id)
        {
            var tinhthanh = db.DiaDiemThamQuans.Where(t => t.MaTinh == id).FirstOrDefault();

            List<ChiTietTour> ct = db.ChiTietTours.Where(t => t.MaDDTQ == tinhthanh.MaDDTQ).ToList();

            List<Tour> listtour = new List<Tour>();
            foreach (var item in ct)
            {
                if(item.STT==1)
                {
                    var t = db.Tours.Where(m => m.MaTour == item.MaTour).FirstOrDefault();
                    listtour.Add(t);
                }    
            }
            return listtour;
        }

        public List<Tour> Lay_tourtheo(string id)
        {
            List<Hotel> hotels = db.Hotels.Where(t => t.MaTinh == id).ToList();
            var tinhthanh = db.TinhThanhs.Where(t => t.MaTinh == id).FirstOrDefault();
            Session["tentinh"] = tinhthanh.TenTinh;
            List<Tour> listtour = new List<Tour>();
            foreach (var item in hotels)
            {

                var t = db.Tours.Where(m => m.MaKS == item.MaKS).FirstOrDefault();
                listtour.Add(t);

            }
            return listtour;
        }
        public ActionResult ListTour(string id)
        {
            //var tinhthanh = db.DiaDiemThamQuans.Where(t => t.MaTinh == id).FirstOrDefault();

            var listtourTinh = Lay_tourtheo(id);
            return View(listtourTinh);
        }
        //dia danh

        public ActionResult List_Tour(string id)
        {
            var tinhthanh = db.DiaDiemThamQuans.Where(t => t.MaDDTQ == id).FirstOrDefault();
            var listtourDD = Lay_tourtheo(tinhthanh.MaTinh);
            return View(listtourDD);
        }

        public ActionResult LienHe()
        {
            return View();
        }
        public ActionResult VeChungToi()
        {
            return View();
        }
        public ActionResult TinTuc()
        {
            return View();
        }
        public ActionResult ThanhToan()
        {
            return View();
        }
        public ActionResult DangNhap() 
        {
            return View();
        }
        public ActionResult DangKy()
        {
            return View();
        }
        public ActionResult ThongTinTaiKhoan()
        {
            return View();
        }
    }
    }
