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
            Session["tentinh"] = tinhthanh.TinhThanh.TenTinh; 

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

        [HttpPost]
        public ActionResult ListTourTinhThanh(string trangthai, string noikhoihanh, string songay, string ngaykhoihanh, string songuoi, int? songaybd, int? songaykt, int? songuoibd)
        {
            //Xử lý số ngày, số người, ngày khơi hành
            if (songay == "1 - 3 ngày")
            {
                songaybd = 1;
                songaykt = 3;
            }
            else if (songay == "4 - 7 ngày")
            {
                songaybd = 4;
                songaykt = 7;
            }
            else if (songay == "8 - 10 ngày")
            {
                songaybd = 8;
                songaykt = 10;
            }
            else if (songay == "10+ ngày")
            {
                songaybd = 10;
                songaykt = 100;
            }
            //--------------------
            if (songuoi == "1 người")
            {
                songuoibd = 1;
            }
            else if (songay == "2 - 3 người")
            {
                songuoibd = 2;
            }
            else if (songuoi == "3 - 5 người")
            {
                songuoibd = 3;
            }
            else if (songay == "5+ người")
            {
                songuoibd = 5;
            }
            //ngaykhoihanh = String.Format("{0:d/M/yyyy}", tour.NgayKhoihanh);
            //DateTime.Now.ToString("yyyy-MM-dd");
            var tours = db.Tours.Where(s => s.TrangThai == trangthai && s.NoiKhoiHanh == noikhoihanh && (s.SoNgay >= songaybd && s.SoNgay <= songaykt) && s.SoChoNull >= songuoibd);
            return View(tours.ToList());
        }




        //dia danh

        public ActionResult ListTour_DiaDiem(string id)
        {
            var iddiadiem = db.DiaDiemThamQuans.Where(d => d.MaDDTQ == id).FirstOrDefault();
            List<Tour> tour = lay_Tour(id);
            
            Session["tendiadiem"] = iddiadiem.TenDDTQ;
            return View(tour);
        }
        [HttpPost]
        public ActionResult ListTour_DiaDiem(string trangthai, string noikhoihanh, string songay, string ngaykhoihanh, string songuoi, int? songaybd, int? songaykt, int? songuoibd)
        {
            //Xử lý số ngày, số người, ngày khơi hành
            if (songay == "1 - 3 ngày")
            {
                songaybd = 1;
                songaykt = 3;
            } else if (songay == "4 - 7 ngày")
            {
                songaybd = 4;
                songaykt = 7;
            }
            else if (songay == "8 - 10 ngày")
            {
                songaybd = 8;
                songaykt = 10;
            }
            else if (songay == "10+ ngày")
            {
                songaybd = 10;
                songaykt = 100;
            }
            //--------------------
            if (songuoi == "1 người")
            {
                songuoibd = 1;
            }
            else if (songay == "2 - 3 người")
            {
                songuoibd = 2;
            }
            else if (songuoi == "3 - 5 người")
            {
                songuoibd = 3;
            }
            else if (songay == "5+ người")
            {
                songuoibd = 5;
            }
            //ngaykhoihanh = String.Format("{0:d/M/yyyy}", tour.NgayKhoihanh);
            //DateTime.Now.ToString("yyyy-MM-dd");
            var tours = db.Tours.Where(s => s.TrangThai == trangthai && s.NoiKhoiHanh == noikhoihanh && (s.SoNgay >= songaybd && s.SoNgay <= songaykt)  && s.SoChoNull >= songuoibd);
            return View(tours.ToList());
        }

        public ActionResult LienHeGuest()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LienHeGuest(PhanHoi phanHoi)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var idPH = "PHKH" + rd.Next(1, 1000);
                phanHoi.MaPhanHoi = idPH;

                phanHoi.NgayPH = DateTime.Now;
                phanHoi.TrangThai = false;
                db.PhanHois.Add(phanHoi);
                TempData["thongbao"] = "taothanhcong";
                db.SaveChanges();
            }
            else
            {
                return View();

            }
            return RedirectToAction("HomePageGuest");
           
        }
        //Anh Hau
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
    }
    }
