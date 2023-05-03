using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Tourist_VietripInsum_2023.common;
using Tourist_VietripInsum_2023.Models;
using PagedList;
using System.Web.Routing;
using System.Text.RegularExpressions;

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
       
        public ActionResult ListTourTinhThanh(string id,int? page)
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
                if(a.TrangThai!="Sắp ra mắt" && a.SoChoNull>0)
                {
                    tour.Add(a);
                }    
                
            }
            int pageSize = 9;
            int pageNum = (page ?? 1);
            return View(tour.ToPagedList(pageNum,pageSize));
        }

        [HttpPost]
        public ActionResult ListTourTinhThanh(string trangthai, int? page, string noikhoihanh, string songay, string ngaykhoihanh, string songuoi, int? songaybd, int? songaykt, int? songuoibd)
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
            else if (songuoi == "2 - 3 người")
            {
                songuoibd = 2;
            }
            else if (songuoi == "3 - 5 người")
            {
                songuoibd = 3;
            }
            else if (songuoi == "5+ người")
            {
                songuoibd = 5;
            }
            //ngaykhoihanh = String.Format("{0:d/M/yyyy}", tour.NgayKhoihanh);
            DateTime.Now.ToString("yyyy-MM-dd");
            int pageSize = 9;
            int pageNum = (page ?? 1);
            List<Tour> toura = new List<Tour>();
            List<DiaDiemThamQuan> diaDiems = db.DiaDiemThamQuans.ToList();
            List<ChiTietTour> chiTietTours = db.ChiTietTours.ToList();
            List<Tour> dstours = db.Tours.ToList();
            string id = (string)Session["tentinh"];
            var tinhthanh = (from s in diaDiems

                             join a in chiTietTours on s.MaDDTQ equals a.MaDDTQ
                             where s.MaTinh == id
                             group a by a.MaTour into g
                             select new
                             {
                                 MaTour = g.FirstOrDefault().MaTour,

                             }).ToList();

            foreach (var item in tinhthanh)
            {
                var a = db.Tours.Where(s => s.MaTour == item.MaTour).FirstOrDefault();
                if (a.TrangThai != "Sắp ra mắt" && a.SoChoNull > 0)
                {
                    toura.Add(a);
                }

            }
            if (noikhoihanh == "---Tất cả---")
            {
                var toursearchall = toura.ToList();
                var toursall = toursearchall.ToPagedList(pageNum, pageSize);
                return View(toursall);
            }
            //List<Tour> tour = Lay_DiaDiem();
            var toursearch = toura.Where(s => s.NoiKhoiHanh == noikhoihanh && (s.SoNgay >= songaybd && s.SoNgay <= songaykt) && s.SoChoNull >= songuoibd).ToList();
            var tours = toursearch.ToPagedList(pageNum, pageSize);
            return View(tours);
        }




        //dia danh

        public ActionResult ListTour_DiaDiem(string id,int? page)
        {
            if (id != null)
            {
                Session["madiadiem"] = id;
                var iddiadiem = db.DiaDiemThamQuans.Where(d => d.MaDDTQ == id).FirstOrDefault();
                List<Tour> tour = lay_Tour(id);
                int pageSize = 9;
                int pageNum = (page ?? 1);
                Session["tendiadiem"] = iddiadiem.TenDDTQ;
                return View(tour.ToPagedList(pageNum, pageSize));
            }
            else
            {
                id = (string)Session["madiadiem"];
                var iddiadiem = db.DiaDiemThamQuans.Where(d => d.MaDDTQ == id).FirstOrDefault();
                List<Tour> tour = lay_Tour(id);
                int pageSize = 9;
                int pageNum = (page ?? 1);
                Session["tendiadiem"] = iddiadiem.TenDDTQ;
                return View(tour.ToPagedList(pageNum, pageSize));
            }
            
        }
        [HttpPost]
        public ActionResult ListTour_DiaDiem(string trangthai, int? page, string noikhoihanh, string songay, string ngaykhoihanh, string songuoi, int? songaybd, int? songaykt, int? songuoibd)
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
            else if (songuoi == "2 - 3 người")
            {
                songuoibd = 2;
            }
            else if (songuoi == "3 - 5 người")
            {
                songuoibd = 3;
            }
            else if (songuoi == "5+ người")
            {
                songuoibd = 5;
            }
            //ngaykhoihanh = String.Format("{0:d/M/yyyy}", tour.NgayKhoihanh);
            DateTime.Now.ToString("yyyy-MM-dd");
            int pageSize = 9;
            int pageNum = (page ?? 1);
            if (noikhoihanh == "---Tất cả---")
            {
                List<Tour> tourall = lay_Tour((string)Session["madiadiem"]);
                var toursearchall = tourall.ToList();
                var toursall = toursearchall.ToPagedList(pageNum, pageSize);
                return View(toursall);
            }
            List<Tour> tour = lay_Tour((string)Session["madiadiem"]);
            var toursearch = tour.Where(s => s.NoiKhoiHanh == noikhoihanh && (s.SoNgay >= songaybd && s.SoNgay<=songaykt) && s.SoChoNull >= songuoibd).ToList();
      
            var tours = toursearch.ToPagedList(pageNum, pageSize);
            return View(tours);
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

        public ActionResult ListTour(int? page)
        {
            int pageSize = 9;
            int pageNum = (page ?? 1);
            List<Tour> tour = db.Tours.Where(s => s.TrangThai == "Tour nổi bật" && s.SoChoNull>0).ToList();
            return View(tour.ToPagedList(pageNum,pageSize));
        }

        [HttpPost]
        public ActionResult ListTour(string trangthai, int? page, string noikhoihanh, string songay, string ngaykhoihanh, string songuoi, int? songaybd, int? songaykt, int? songuoibd)
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
            else if (songuoi == "2 - 3 người")
            {
                songuoibd = 2;
            }
            else if (songuoi == "3 - 5 người")
            {
                songuoibd = 3;
            }
            else if (songuoi == "5+ người")
            {
                songuoibd = 5;
            }
            //ngaykhoihanh = String.Format("{0:d/M/yyyy}", tour.NgayKhoihanh);
            DateTime.Now.ToString("yyyy-MM-dd");
            int pageSize = 9;
            int pageNum = (page ?? 1);
            if (noikhoihanh == "---Tất cả---")
            {
                List<Tour> tourall = db.Tours.Where(s => s.TrangThai == "Tour nổi bật" && s.SoChoNull > 0).ToList(); ;
                var toursearchall = tourall.ToList();
                var toursall = toursearchall.ToPagedList(pageNum, pageSize);
                return View(toursall);
            }
            List<Tour> tour = db.Tours.Where(s => s.TrangThai == "Tour nổi bật" && s.SoChoNull > 0).ToList();
            var toursearch = tour.Where(s => s.NoiKhoiHanh == noikhoihanh && (s.SoNgay >= songaybd && s.SoNgay <= songaykt) && s.SoChoNull >= songuoibd).ToList();

            var tours = toursearch.ToPagedList(pageNum, pageSize);
            return View(tours);
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
            int socho = 0;
            var tour = db.Tours.Where(t => t.MaTour == matour).FirstOrDefault();
            if(tour.SoChoNull == null || tour.SoChoNull == 0)
            {
                socho = 0;
            }
            else
            {
                socho = (int)tour.SoChoNull;
                List<BookTour> bt = db.BookTours.Where(t => t.MaTour == matour).ToList();
                foreach (var item in bt)
                {
                    socho = socho - (int)item.SoCho;
                }
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
            KhachHang khdn = Session["UserKH"] as KhachHang;
            if(khdn!=null)
            {
                booktour.MaKH = khdn.MaKH;
                booktour.SdtKH = khdn.SDT;
                Session["TaiKhoan"] = khdn.MaKH;


            }    
            else
            {

                var khach = db.KhachHangs.Where(s => s.SDT == SDT).FirstOrDefault();
                if (khach == null)
                {
                    //khong dang nhap va khong phia khach trong he thong
                    KhachHang kh = new KhachHang();
                    var idKH = "GS" + rd.Next(1, 1000);
                    kh.MaKH = idKH;
                    booktour.MaKH = idKH;
                    kh.SDT = SDT;
                    booktour.SdtKH = kh.SDT;
                    kh.DiaChi = DiaChi;
                    kh.Email = Email;
                    kh.HoTenKH = TenKH;
                    kh.MaLoaiKH = "TH";
                    Session["TaiKhoan"] = kh;
                    db.KhachHangs.Add(kh);
                    db.SaveChanges();
                }
                else
                {
                    //khong dang nhap nhung la khach trong he thong
                    Session["TaiKhoan"] = khach;
                    booktour.MaKH = khach.MaKH;
                    booktour.SdtKH = khach.SDT;
                    khach.DiaChi = DiaChi;
                    khach.Email = Email;
                    khach.HoTenKH = TenKH;
                }
            }    
            var idDH = "DH" + rd.Next(1, 1000);
            booktour.MaDH = idDH;
            booktour.MaTour = matour;
            booktour.NgayLap = DateTime.Now;
            booktour.TrangThaiTT = false;
            booktour.XacNhanDH = false;
            var khachhang = db.KhachHangs.Where(s => s.SDT == SDT).FirstOrDefault();
            booktour.TotalPrice = 0.0;
            booktour.SoCho = 0;
            booktour.XacNhanDH = false;
            Session["madonhang"] = booktour.MaDH;
            
            db.BookTours.Add(booktour);
            db.SaveChanges();
            
            return RedirectToAction("Ticket");
        }

        public ActionResult Ticket()
        {
            string matour = (string)Session["Matourchon"];
            var tour = db.Tours.Where(t => t.MaTour == matour).FirstOrDefault();
            ViewBag.chodamua = tour.SoChoNull;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ticket(string soluongdat, bool thanhtoan)
        {
            var maDH = Session["madonhang"].ToString();
            var dh = db.BookTours.Where(t => t.MaDH == maDH).FirstOrDefault();
            var tour = db.Tours.Where(s => s.MaTour == dh.MaTour).FirstOrDefault();
            var ves = db.Ves.Where(s => s.MaDH == maDH).ToList();
            if(ves.Count() > 0)
            {
                foreach(var item in ves)
                {
                    db.Ves.Remove(item);
                    tour.SoChoNull += 1;
                }
            }
            double tongtien = 0;
            int m = 0;
            //null
            if (soluongdat == "")
            {
                TempData["noti"] = "errornull";
                return RedirectToAction("Ticket", "Guest");
            }
            else
            {
                m = int.Parse(soluongdat);
                if (m == 0)
                {
                    TempData["noti"] = "errornull";
                    return RedirectToAction("Ticket", "Guest");
                }
            }
            if (ModelState.IsValid)
            {
                Random rd = new Random();

                //Tạo vé theo số lượng khách đặt
                for (int i = 1; i <= m; i++)
                {
                    Ve ve = new Ve();
                    var maVe = "V" + maDH + rd.Next(1, 9) + rd.Next(10, 20) + rd.Next(100, 500);
                    ve.MaVe = maVe;
                    ve.MaDH = maDH;
                    ve.Hoten_KH = Request["HoTen_KH" + i];
                    ve.MaLVe = Request["MaLVe" + i];
                    ve.GioiTinh = Request["GioiTinh" + i];
                    var test = Request["NgaySinh" + i];
                    
                    ve.NgaySinh = DateTime.Parse(test);
                    ve.LuuY = Request["LuuY" + i];
                    

                    if (ve.MaLVe == "TICKET01")
                    {
                        tongtien = tongtien + (int)tour.GiaNguoiLon;
                    }
                    else if (ve.MaLVe == "TICKET02")
                    {
                        tongtien = tongtien + (int)tour.GiaTreEm;
                    }
                    db.Ves.Add(ve);
                    //db.SaveChanges();

                }
                //Update tổng tiền cho đơn đặt tour
                //var donhang = db.BookTours.Where(s => s.MaDH == maDH).FirstOrDefault();
                var kh = db.KhachHangs.Where(s => s.MaKH == dh.MaKH).FirstOrDefault();
                dh.TotalPrice = (int)tongtien - (tongtien * kh.LoaiKH.ChietKhau);
                dh.HinhThucThanhToan = thanhtoan;
                dh.SoCho = m;
                tour.SoChoNull -= dh.SoCho;
                db.SaveChanges();
                TongtienDAT(kh.MaKH);
                TempData["noti"] = "success";
                return RedirectToAction("Payment");
            }
            return View();
        }

        //Confirm payment view
        public ActionResult Payment()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(string id)
        {
            //Email
            string content = System.IO.File.ReadAllText(Server.MapPath("/Content/template/mailconn.html"));
            var dh = db.BookTours.Where(s => s.MaDH == id).FirstOrDefault();
            var kh = db.KhachHangs.Where(s => s.MaKH == dh.MaKH).FirstOrDefault();
            var t = db.Tours.Where(s => s.MaTour == dh.MaTour).FirstOrDefault();
            content = content.Replace("{{TenKH}}", kh.HoTenKH);
            content = content.Replace("{{Phoneno}}", dh.MaKH);
            content = content.Replace("{{MaDH}}", dh.MaDH);
            content = content.Replace("{{Email}}", kh.Email);
            content = content.Replace("{{Address}}", dh.MaKH);
            string hinhthuc = "";
            if(dh.HinhThucThanhToan==true)
            {
                hinhthuc = "Chuyển khoản";
            }
            else
            {
                hinhthuc = "Thanh toán tại văn phòng";
            }
            DateTime ngaydat = (DateTime)dh.NgayLap;
            DateTime hanthanhtoan = ngaydat.AddDays(1);
            content = content.Replace("{{hinhthuc}}", hinhthuc);
            content = content.Replace("{{ngaydat}}", ngaydat.ToString());
            content = content.Replace("{{hanthanhtoan}}", hanthanhtoan.ToString());
            content = content.Replace("{{MaTour}}", t.MaTour);
            content = content.Replace("{{TenTour}}", t.TenTour);
            content = content.Replace("{{ngaykhoihanh}}", t.NgayKhoihanh.ToString());
            content = content.Replace("{{noikhoihanh}}", t.NoiKhoiHanh);
            content = content.Replace("{{hanchotve}}", t.HanChotDatVe.ToString());
            content = content.Replace("{{total}}", dh.TotalPrice.ToString());

            ////Gui mail
            var toEmail = ConfigurationManager.AppSettings["toEmailAddress"].ToString();
            new MailHelp().SendMail(kh.Email, "Xác nhận đặt tour", content);
            TempData["noti"] = "success";
            return RedirectToAction("HomePageGuest", "Guest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuyVe(string id)
        {
            BookTour booktour = db.BookTours.Where(s => s.MaDH == id).FirstOrDefault();
            var ves = db.Ves.Where(s => s.MaDH == id).ToList();
            var tour = db.Tours.Where(s => s.MaTour == booktour.MaTour).FirstOrDefault();
            tour.SoChoNull += booktour.SoCho;
            foreach (var item in ves)
            {
                db.Ves.Remove(item);
            }
            db.BookTours.Remove(booktour);
            db.SaveChanges();
            return RedirectToAction("HomePageGuest", "Guest");
        }

        //Hàm tính tổng tiền đặt
        public ActionResult TongtienDAT(string makh)
        {
            var dh = db.BookTours.Where(s => s.MaKH == makh).ToList();
            var kh = db.KhachHangs.Where(s => s.MaKH == makh).FirstOrDefault();
            kh.TongTienDat = 0.0;
            foreach(var item in dh)
            {
                if(item.TrangThaiTT == true)
                {
                    kh.TongTienDat += item.TotalPrice;
                }
            }
            if(kh.TongTienDat >= 15000000)
            {
                kh.MaLoaiKH = "TT";
            }else if(kh.TongTienDat >= 50000000)
            {
                kh.MaLoaiKH = "VIP";
            }
            db.Entry(kh).State = EntityState.Modified;
            db.SaveChanges();
            return View();
        }
 
        public JsonResult LoadMore(int skip, int take)
        {
            var data = "Tui là Dĩm Khang nè!";
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        //--------------------------------------------------------------Customer------------------------------------------------------
        public void LuuImageCus(KhachHang t, HttpPostedFileBase ImagerCus)
        {
            #region Hình ảnh

            if (ImagerCus == null)
            {
                t.HinhDaiDien = t.HinhDaiDien;
            }
            else
            {
                //Xác định đường dẫn lưu file : Url tương đói => tuyệt đói
                var urlTuongdoi = "/images/";
                var urlTuyetDoi = Server.MapPath(urlTuongdoi);// Lấy đường dẫn lưu file trên server

                //Check trùng tên file => Đổi tên file  = tên file cũ (ko kèm đuôi)
                //Ảnh.jpg = > ảnh + "-" + 1 + ".jpg" => ảnh-1.jpg

                string fullDuongDan = urlTuyetDoi + ImagerCus.FileName;

                int i = 1;
                while (System.IO.File.Exists(fullDuongDan) == true)
                {
                    // 1. Tách tên và đuôi 
                    var ten = Path.GetFileNameWithoutExtension(ImagerCus.FileName);
                    var duoi = Path.GetExtension(ImagerCus.FileName);
                    // 2. Sử dụng biến i để chạy và cộng vào tên file mới
                    fullDuongDan = urlTuyetDoi + ten + "-" + i + duoi;
                    i++;
                    // 3. Check lại 
                }
                #endregion
                //Lưu file (Kiểm tra trùng file)
                ImagerCus.SaveAs(fullDuongDan);
                t.HinhDaiDien = urlTuongdoi + Path.GetFileName(fullDuongDan);
            }
        }

        public ActionResult SignupGuest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignupGuest([Bind(Include = "MaKH,SDT,MaLoaiKH,Username,UserPassword,HoTenKH,DiaChi,Email,NgaySinh,GioiTinh,HinhDaiDien,TongTienDat")] KhachHang khachHang, string ConfirmPassword, HttpPostedFileBase ImagerCus)
        {
            var email = db.KhachHangs.Count(s => s.Email == khachHang.Email);
            var username = db.KhachHangs.Count(s => s.Username == khachHang.Username);

            if ((email >= 1))
            {
                TempData["errorInfo"] = "Email is already taken. Please choose a different email!";
                khachHang.Email = "";
                return View(khachHang);
            }
            if ((username >= 1))
            {
                TempData["errorInfo"] = "Username is already taken. Please choose a different username!";
                khachHang.Username = "";
                return View(khachHang);
            }
            if (khachHang.UserPassword.Length < 6)
            {
                TempData["errorPass"] = "Password must be over 6 characters!";
                return View(khachHang);
            }
            if (ModelState.IsValid)
            {
                if (ConfirmPassword != khachHang.UserPassword)
                {
                    TempData["errorMK"] = "Confirm password not valid!";
                    return View(khachHang);
                }
                else if (ConfirmPassword == khachHang.UserPassword)
                {
                    Random rd = new Random();
                    
                    KhachHang kh = db.KhachHangs.Where(s => s.SDT == khachHang.SDT).FirstOrDefault();
                    if (kh != null)
                    {
                        LuuImageCus(khachHang, ImagerCus);
                        kh.HinhDaiDien = khachHang.HinhDaiDien;
                        kh.Username = khachHang.Username;
                        kh.UserPassword = khachHang.UserPassword;
                        kh.HoTenKH = khachHang.HoTenKH;
                        kh.DiaChi = khachHang.DiaChi;
                        kh.Email = khachHang.Email;
                        kh.NgaySinh = khachHang.NgaySinh;
                        kh.GioiTinh = khachHang.GioiTinh;
                        db.Entry(kh).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("LoginGuest");
                    }

                    LuuImageCus(khachHang, ImagerCus);
                    var makh = "KH" + rd.Next(100, 10000);
                    khachHang.MaKH = makh;
                    khachHang.MaLoaiKH = "TH";
                    db.KhachHangs.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("LoginGuest");
                }
            }
            return View(khachHang);
        }
        //----------------------check---------------------
        public JsonResult CheckEmailAvailability(string Email)
        {
            System.Threading.Thread.Sleep(200);

            var mailCus = db.KhachHangs.Where(x => x.Email == Email).FirstOrDefault();
            if (mailCus != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult CheckUserNameAvailability(string userName)
        {
            System.Threading.Thread.Sleep(200);

            var userNameCus = db.KhachHangs.Where(x => x.Username == userName).FirstOrDefault();
            if (userNameCus != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        //--------------------end check-------------------
        public ActionResult LoginGuest()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginGuest(string username, string password)
        {
            var data = db.KhachHangs.Where(s => s.Username == username && s.UserPassword == password).FirstOrDefault();
            var taikhoan = db.KhachHangs.SingleOrDefault(s => s.Username == username && s.UserPassword == password);
            if (taikhoan == null)
            {
                TempData["error"] = "err";
                return View("LoginGuest");
            }
            else if (taikhoan != null)
            {
                //add session
                db.Configuration.ValidateOnSaveEnabled = false;
                Session["UserKH"] = taikhoan;
                return RedirectToAction("HomePageGuest", "Guest");
            }
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Clear();//remove session
            FormsAuthentication.SignOut();
            return RedirectToAction("HomePageGuest");
        }

        //THONGTINCANHAN
        public static bool IsValidEmail(string email)
        {
            try
            {
                // Kiểm tra email có đúng định dạng
                var match = Regex.Match(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (match.Success)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        public JsonResult CheckMail(string usermail, string makh)
        {
            System.Threading.Thread.Sleep(200);
            var SeachData = db.KhachHangs.Where(x => x.Email == usermail && x.MaKH!=makh).SingleOrDefault();

            bool isValid = IsValidEmail(usermail);
            if(isValid==true)
            {
                if (SeachData != null)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }    
            else
            {
                return Json(2);
            }

        }

        public JsonResult CheckSDT(string userSDT,string makh)
        {
            System.Threading.Thread.Sleep(200);
            var phone = db.KhachHangs.Where(x => x.SDT == userSDT && x.MaKH!=makh).SingleOrDefault();

            if (phone != null)
            {
                return Json(2);
            }
            else
            {
                if(userSDT.Length==10)
                    return Json(0);
                return Json(1);
            }

        }
        public JsonResult KtraPass(string pass1, string pass2)
        {
            System.Threading.Thread.Sleep(200);
            if (pass1 != pass2)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }
        public ActionResult Thongtinkhachhang(string id)
        {

            var kh = db.KhachHangs.Where(s => s.MaKH == id).FirstOrDefault();
            return View(kh);
        }
        [HttpPost]
        public ActionResult Thongtinkhachhang(KhachHang khach,HttpPostedFileBase HinhDaiDien, string imgnv)
        {
            if (ModelState.IsValid)
            {
                if (HinhDaiDien != null)
                {
                    var fileName = Path.GetFileName(HinhDaiDien.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);

                    khach.HinhDaiDien = fileName;
                    //Save vào Images Folder
                    HinhDaiDien.SaveAs(path);

                }
                else
                {
                    khach.HinhDaiDien = imgnv;
                }
                Session["UserKH"] =khach;
                TempData["noti"] = "editkhach";
                db.Entry(khach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Thongtinkhachhang", new RouteValueDictionary(
                                   new { controller = "Guest", action = "Thongtinkhachhang", Id = khach.MaKH }));

            }
            return View(khach);
        }
        public ActionResult Dondat( string id)
        {
            List<BookTour> dsDon = db.BookTours.Where(s => s.MaKH == id ).ToList();
            return View();
        }

        public ActionResult TourBookingHistory()
        {
            if (Session["UserKH"] == null)
            {
                return RedirectToAction("LoginGuest");
            }
            return View();
        }

        public ActionResult NewOrderPlaced()
        {
            if (Session["UserKH"] == null)
            {
                return RedirectToAction("LoginGuest");
            }
            return View();
        }

        public ActionResult CancelBookTour(string id)
        {
            TempData["DeleteSuccess"] = "success";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTour bookTour = db.BookTours.Find(id);
            if (bookTour == null)
            {
                return HttpNotFound();
            }
            return View(bookTour);
        }
        [HttpPost, ActionName("CancelBookTour")]
        public ActionResult CancelBookTourConfirmed(string id)
        {
            var userKH = (Tourist_VietripInsum_2023.Models.KhachHang)HttpContext.Session["UserKH"];
            var ve = db.Ves.Where(s => s.MaDH == id).ToList();
            db.Ves.RemoveRange(ve);
            db.SaveChanges();

            var bookTour = db.BookTours.Find(id);
            var tour = db.Tours.Find(bookTour.MaTour);
            tour.SoChoNull += bookTour.SoCho;
            db.BookTours.Remove(bookTour);
            db.SaveChanges();
            TempData["DeleteSuccess"] = "Deletesuccess";
            return RedirectToAction("NewOrderPlaced/" + userKH.MaKH);
        }
    }
}
