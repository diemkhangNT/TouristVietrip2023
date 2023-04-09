using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
            List<Tour> tourChon = db.Tours.Where(n=>n.TrangThai!="Sắp ra mắt").ToList();

            var bangchon = (from diadiem in diaDiemThamQuans
                            join chitiet in chiTietTours on diadiem.MaDDTQ equals chitiet.MaDDTQ
                            join tour in tourChon on chitiet.MaTour equals tour.MaTour
                            group chitiet by chitiet.MaDDTQ into g
                            select new
                            {
                                MaDD = g.FirstOrDefault().MaDDTQ,
                                Matour= g.FirstOrDefault().MaTour

                            }).ToList();

            List<DiaDiemThamQuan> d = new List<DiaDiemThamQuan>();

            foreach (var i in bangchon)
            {
                var item = db.DiaDiemThamQuans.Where(m => m.MaDDTQ == i.MaDD).FirstOrDefault();
               d.Add(item);
                   

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
            List<DiaDiemThamQuan> diaDiemThamQuans = db.DiaDiemThamQuans.ToList();
            List<ChiTietTour> chiTietTours = db.ChiTietTours.ToList();
            List<Tour> tourChon = db.Tours.Where(n => n.TrangThai != "Sắp ra mắt").ToList();

            var bangtinh = (from diadiem in diaDiemThamQuans
                            join chitiet in chiTietTours on diadiem.MaDDTQ equals chitiet.MaDDTQ
                            join tour in tourChon on chitiet.MaTour equals tour.MaTour
                            group diadiem by diadiem.MaTinh into g
                            select new
                            {
                                MaTinh = g.FirstOrDefault().MaTinh,
                            }).ToList();

            List<TinhThanh> tinhtour = new List<TinhThanh>();

            foreach (var i in bangtinh)
            {
                var item = db.TinhThanhs.Where(m => m.MaTinh == i.MaTinh).FirstOrDefault();
                tinhtour.Add(item);
            }

            return PartialView(tinhtour);
        }

        public ActionResult Tourinfomation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tour tour = db.Tours.Where(s => s.MaTour == id).FirstOrDefault();
            Session["Matourchon"] = id;
            if (tour == null)
            {
                return HttpNotFound();
            }
            Session["mat"]=id;
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
                if(tourchon.TrangThai!="Sắp ra mắt")
                {
                    tour.Add(tourchon);
                }    
                
            }
            return tour;
        }
       
        public ActionResult ListTourTinhThanh(string id)
        {
            
            List<Tour> tour = new List<Tour>();
            List<DiaDiemThamQuan> diaDiems = db.DiaDiemThamQuans.ToList();
            List<ChiTietTour> chiTietTours = db.ChiTietTours.ToList();
            List<Tour> tours = db.Tours.ToList();
            Session["tentinh"] = id;
            var tinhthanh = (from s in diaDiems

                            join a in chiTietTours on s.MaDDTQ equals a.MaDDTQ
                            where s.MaTinh==id
                            group a by a.MaTour into g
                            select new
                            {
                                MaTour = g.FirstOrDefault().MaTour,

                            }).ToList();
            
            foreach (var item in tinhthanh)
            {
                var a = db.Tours.Where(s => s.MaTour == item.MaTour).FirstOrDefault();
                if(a.TrangThai!="Sắp ra mắt")
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
            DateTime.Now.ToString("yyyy-MM-dd");
            var tours = db.Tours.Where(s => s.TrangThai == trangthai 
            && s.NoiKhoiHanh == noikhoihanh && (s.SoNgay >= songaybd && s.SoNgay <= songaykt) 
             && s.SoChoNull >= songuoibd);
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

                var kh = db.KhachHangs.Where(k => k.SDT == phanHoi.Sdt).FirstOrDefault();
                if(kh!=null)
                {
                    phanHoi.MaKH = kh.MaKH;
                }    
                phanHoi.NgayPH = DateTime.Now;
                phanHoi.TrangThai = false;
                db.PhanHois.Add(phanHoi);
                TempData["thongbaoLH"] = "taothanhcong";
                db.SaveChanges();
            }
            else
            {
                return View();

            }
            return RedirectToAction("HomePageGuest");
           
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Bind(Include = "Email,Sdt,NoiDung")]
        public ActionResult LienHeTour(PhanHoi phanHoi,string Email,string Sdt,string NoiDung)
        {
            
                Random rd = new Random();
                var idPH = "PHKH" + rd.Next(1, 1000);
                phanHoi.MaPhanHoi = idPH;
            phanHoi.Sdt = Sdt;
            phanHoi.Email = Email;
            phanHoi.NoiDung = NoiDung;
                var kh = db.KhachHangs.Where(k => k.SDT == phanHoi.Sdt).FirstOrDefault();
                if (kh != null)
                {
                    phanHoi.MaKH = kh.MaKH;
                }
                phanHoi.NgayPH = DateTime.Now;
                phanHoi.TrangThai = false;
                db.PhanHois.Add(phanHoi);
                TempData["thongbaoLH"] = "taothanhcong";
                db.SaveChanges();
           

            
            var idt = Session["Matourchon"];
            return RedirectToAction("HomePageGuest");

        }

        public ActionResult ListTour()
        {
            List<Tour> tour = db.Tours.Where(s => s.TrangThai == "Tour nổi bật").ToList();

            return View(tour);
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

        //XULYVE-DAT

        public int SoChoTrong(string matour)
        {

            var tour = db.Tours.Where(t => t.MaTour == matour).FirstOrDefault();
            int socho = (int)tour.SoChoNull;
            List<BookTour> bt = db.BookTours.Where(t => t.MaTour == matour).ToList();
            foreach (var item in bt)
            {
                socho = socho - (int)item.SoCho;
            }
            return socho;
        }
        //public ActionResult BookTour(string id)
        //{
        //    string matour = (string)Session["Matourchon"];
        //    //ViewBag.chodamua = SoChoTrong(matour);
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookTour(BookTour booktour,string DiaChi,string SDT,string Email,string TenKH)
        {
            string matour = (string)Session["Matourchon"];
            Random rd = new Random();
            var khach = db.KhachHangs.Where(s => s.SDT == SDT).FirstOrDefault();
            if (khach == null)
            {

                KhachHang kh = new KhachHang();
                
                var idKH = "KH" + rd.Next(1, 1000);
                kh.MaKH = idKH;
                booktour.MaKH = idKH;


                kh.SDT = SDT;
                booktour.SdtKH = kh.SDT;
                kh.DiaChi = DiaChi;
                kh.Email = Email;
                kh.HoTenKH = TenKH;
                db.KhachHangs.Add(kh);
                db.SaveChanges();
            }
            else
            {
                //KhachHang khachhang = Session["TaiKhoan"] as KhachHang;
                booktour.MaKH = khach.MaKH;
                booktour.SdtKH = khach.SDT;
            }
            var idDH = "DH" + rd.Next(1, 1000);
            booktour.MaDH = idDH;
            booktour.MaTour = matour;
            booktour.NgayLap = DateTime.Now;
            booktour.TrangThaiTT = false;
            booktour.TotalPrice = 0;
            booktour.SoCho = 0;
            Session["madonhang"] = booktour.MaDH;
            db.BookTours.Add(booktour);
            db.SaveChanges();
            


            return RedirectToAction("Ticket");


        }

        public ActionResult Ticket()
        {
            string matour = (string)Session["Matourchon"];
            ViewBag.chodamua = SoChoTrong(matour);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ticket(Ve ve,string soluongdat)
        {
            var maDH = Session["madonhang"].ToString();
            var dh = db.BookTours.Where(t => t.MaDH == maDH).FirstOrDefault();
            var tour = db.Tours.Where(s => s.MaTour == dh.MaTour).FirstOrDefault();
            double tongtien = 0;
            int m = int.Parse(soluongdat);
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                //Tạo vé theo số lượng khách đặt
                for (int i = 0; i < m; i++)
                {
                    ve = new Ve();
                    var maVe = "V" + rd.Next(1, 1000);
                    ve.MaVe = maVe;
                    ve.MaDH = maDH;
                    ve.Hoten_KH = Request["HoTen_KH" + i];
                    ve.MaLVe = Request["MaLVe" + i];
                    ve.GioiTinh = Request["GioiTinh" + i];
                    ve.NgaySinh = Convert.ToDateTime(Request["NgaySinh" + i]); 
                    ve.LuuY = Request["LuuY" + i];
                    db.Ves.Add(ve);

                    if (ve.MaLVe == "TICKET01")
                    {
                        tongtien = tongtien + (int)tour.GiaNguoiLon;
                    }
                    else if (ve.MaLVe == "TICKET02")
                    {
                        tongtien = tongtien + (int)tour.GiaTreEm;
                    }
                    db.SaveChanges();

                }
                //Update tổng tiền cho đơn đặt tour
                var donhang = db.BookTours.Where(s => s.MaDH == maDH).FirstOrDefault();

                dh.TotalPrice = (decimal)tongtien;
                dh.SoCho = m;
                db.SaveChanges();
                return RedirectToAction("HomePageGuest");
            }
            return View();
        }
    }
}
