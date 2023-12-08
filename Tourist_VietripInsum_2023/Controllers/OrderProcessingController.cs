using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tourist_VietripInsum_2023.App_Start;
using Tourist_VietripInsum_2023.common;
using Tourist_VietripInsum_2023.DesignPattern.Singleton;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    [AdminAuthorize(idPos = "OP")]
    public class OrderProcessingController : Controller
    {
        TouristEntities1 db = new TouristEntities1();

        public ActionResult HomePageOP()
        {
            var donhang = db.BookTours.Count();
            TempData["TongDonDat"] = donhang;
            var ph = db.PhanHois.Count(s => s.TieuDe == "Tư vấn đơn đặt tour");
            TempData["phanhoi"] = ph;
            var dsDonHangMoi = db.BookTours.OrderByDescending(dhang => dhang.NgayLap).ToList();
        
            return View(dsDonHangMoi);
        }

        public ActionResult ManageTourOrders(string search)
        {
            if (search == null)
            {
                return View(db.BookTours.ToList());
            }
            else
            {
                ViewBag.SearchInfo = search;
                return View(db.BookTours.Where(s => s.MaDH.ToLower().Contains(search) == true || s.SdtKH.ToLower().Contains(search) == true).ToList());
            }
        }

        public ActionResult TourList()
        {
            List<Tour> listTour = db.Tours.Include(t => t.LoaiTour).Where(s=>s.TrangThai!="Sắp ra mắt").ToList();
            return View(listTour);
        }

        public ActionResult CusDetailsTour(string id)
        {
            TempData["MaTour"] = id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<BookTour> booking = db.BookTours.Where(s => s.MaTour == id).ToList();
            List<Ve> ve = new List<Ve>();
            foreach (var i in booking)
            {
                var maDH = i.MaDH;
                ve = db.Ves.Where(s => s.MaDH == maDH).ToList();
            }
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(ve);
        }

        public ActionResult ListOfCustomers(string search)
        {
            if (search == null)
            {
                return View(db.KhachHangs.ToList());
            }
            else
            {
                return View(db.KhachHangs.Where(s => s.MaKH.ToLower().Contains(search) == true || s.SDT.ToLower().Contains(search) == true || s.HoTenKH.ToLower().Contains(search) == true).ToList());
            }
        }

        public ActionResult CusDetail(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            KhachHang khachHang = db.KhachHangs.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        public ActionResult EditCusInfo(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHangs.Find(id);
            Session["maKH"] = id;
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaLoaiKH = new SelectList(db.LoaiKHs, "MaLoaiKH", "TenLoaiKH", khachHang.MaLoaiKH);
            return View(khachHang);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCusInfo(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kh).State = EntityState.Modified;
                db.SaveChanges();

                var bookTour = db.BookTours.Where(s => s.MaKH == kh.MaKH).ToList();
                foreach (var item in bookTour)
                {
                    item.SdtKH = kh.SDT;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
                TempData["success"] = "Cập nhật thông tin khách hàng thành công";
                return RedirectToAction("ListOfCustomers");
            }
            ViewBag.LoaiKH = new SelectList("MaLoaiKH", "MaLoaiKH", kh.MaLoaiKH);
            return View(kh);
        }

        public ActionResult CheckCusInfo(string search)
        {
            var info = db.KhachHangs.FirstOrDefault(s => s.SDT == search);
            if (search == null)
            {
                return View();
            }
            else
            {
                if (info == null)
                {
                    ViewBag.Info = info;
                    ViewBag.SDT = search;
                    Session["SDT"] = search;
                    return View();
                }
                else
                {
                    ViewBag.Info = info;
                    ViewBag.SDT = search;
                    Session["SDT"] = search;
                    return View(info);
                }
            }
        }
        [HttpPost, ActionName("CheckCusInfo")]
        public ActionResult CreateCustomer([Bind(Include = "MaKH,SDT,MaLoaiKH,Username,UserPassword,HoTenKH,DiaChi,Email,NgaySinh,GioiTinh,HinhDaiDien")] KhachHang khachHang)
        {
            Random random = new Random();
            var idCus = "GS" + random.Next(1,  9+ random.Next(10, 90));

            var checkMail = db.KhachHangs.FirstOrDefault(s => s.Email == khachHang.Email);

            if (checkMail != null)
            {
                ModelState.AddModelError(string.Empty, "Đã có email trong hệ thống !!!");
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(khachHang.HoTenKH) == true ||
                    khachHang.NgaySinh == null ||
                    string.IsNullOrEmpty(khachHang.DiaChi) == true ||
                    string.IsNullOrEmpty(khachHang.Email) == true)
                {
                    TempData["trong"] = "Vui lòng nhập đầy đủ thông tin";
                    return View();
                }
                else
                {
                    khachHang.MaKH = idCus;
                    khachHang.MaLoaiKH = "TH";
                    khachHang.Username = null;
                    khachHang.UserPassword = null;
                    khachHang.HinhDaiDien = null;
                    khachHang.TongTienDat = null;
                    
                    db.KhachHangs.Add(khachHang);
                    db.SaveChanges();
                    //UserLogedInSingleton<KhachHang>.Instance.UpdateSigleton(db);
                }
            }
            return RedirectToAction("CreateOrder");
        }

        public JsonResult CheckEmailAvailability(string mail)
        {
            System.Threading.Thread.Sleep(200);

            var mailCus = db.KhachHangs.Where(x => x.Email == mail).SingleOrDefault();
            if (mailCus != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult CheckNoPhoneAvailability(string noPhone)
        {
            System.Threading.Thread.Sleep(200);

            var checkNoPhone = db.KhachHangs.Where(x => x.SDT == noPhone).SingleOrDefault();
            if (checkNoPhone != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        [HttpGet]
        public ActionResult GetInfoTour(string id)
        {
            // Lấy thông tin tương ứng với id từ CSDL
            var infoTour = db.Tours.Find(id);
            if (infoTour == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(infoTour.SoChoNull, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult CreateOrder()
        {
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "TenTour");
            return View();
        }
        [HttpPost]
        public ActionResult CreateOrder(BookTour donHang)
        {
            Random random = new Random();
            var maDH = "DH" + random.Next(1, 9)+ random.Next(10, 50)+ random.Next(51, 99);
            var user = (Tourist_VietripInsum_2023.Models.NhanVien)HttpContext.Session["user"];
            if (ModelState.IsValid)
            {
                if (donHang.MaTour == null)
                {
                    TempData["ErrorTour"] = "Vui lòng chọn tour!!!";
                    return RedirectToAction("CreateOrder");
                }
                if (donHang.SoCho < 1 || donHang.SoCho == null)
                {
                    TempData["ErrorSoCho"] = "Số chỗ phải lớn hơn 1!!!";
                    return RedirectToAction("CreateOrder");
                }
                var tour = db.Tours.Find(donHang.MaTour);
                if (donHang.SoCho > tour.SoChoNull)
                {
                    TempData["ErrorSoChoMax"] = "Số chỗ vượt quá số chỗ còn trống";
                    return RedirectToAction("CreateOrder");
                }
                else
                {
                    donHang.MaDH = maDH;
                    Session["MaDH"] = donHang.MaDH;
                    donHang.MaNVLap = user.MaNV;
                    donHang.NgayLap = System.DateTime.Now;
                    donHang.TrangThaiTT = false;
                    donHang.TotalPrice = null;
                    donHang.HinhThucThanhToan = donHang.HinhThucThanhToan;

                    var sdt = Session["SDT"].ToString();
                    var info = db.KhachHangs.FirstOrDefault(s => s.SDT == sdt);

                    donHang.SdtKH = sdt;
                    donHang.MaKH = info.MaKH;

                    if(tour.SoChoNull<donHang.SoCho)
                    {
                        TempData["ErrorSoChoTrong"] = "Đã hết chổ đặt !!!";
                        return RedirectToAction("CreateOrder");
                    }    
                    //Update số chỗ trong tour
                    tour.SoChoNull -= donHang.SoCho;

                    db.BookTours.Add(donHang);
                    db.SaveChanges();
                    Session["SoCho"] = donHang.SoCho;
                }
            }
            return RedirectToAction("CreateTickets");
        }
        //public double TinhChietKhau(double tienbandau,string lt)
        //{
            
        //    var loaitour = db.LoaiTours.Where(s => s.MaLTour == lt).FirstOrDefault();
        //    double pricesale = (double)(tienbandau * (1 - loaitour.ChietKhau));
        //    return pricesale;
        //}

        public double TinhCKKhach(double tienbandau,string makh)
        {
            var khach = db.KhachHangs.Where(s => s.MaKH == makh).FirstOrDefault();
            var lkh = db.LoaiKHs.FirstOrDefault(s => s.MaLoaiKH == khach.MaLoaiKH);
            double pricesale = (double)(tienbandau * (1 - lkh.ChietKhau));
            return pricesale;
        }
        [HttpGet]
        public ActionResult CreateTickets()
        {
            ViewBag.MaLVe = new SelectList(db.LoaiVes, "MaLoaiVe", "TenLVe");
            return View();
        }
        [HttpPost]
        public ActionResult CreateTickets(Ve ve)
        {
            var maDH = Session["MaDH"].ToString();

            //Tìm kiếm tour
            var booking = db.BookTours.FirstOrDefault(s => s.MaDH == maDH);
            var tour = db.Tours.FirstOrDefault(s => s.MaTour == booking.MaTour);

            Random random = new Random();
            int socho = (int)Session["SoCho"];

            for (int i = 0; i < socho; i++)
            {
                if (string.IsNullOrEmpty(Request["HoTenKH_" + i]) == true ||
                        string.IsNullOrEmpty(Request["GioiTinh_" + i]) == true ||
                        string.IsNullOrEmpty(Request["NgaySinh_" + i]) == true)
                {
                    TempData["errorTrong" + i] = "Vui lòng nhập đầy đủ thông tin";
                    return View(ve);
                }
            }

            double tongtien = 0;
            if (ModelState.IsValid)
            {
                //Tạo vé theo số lượng khách đặt
                for (int i = 0; i < socho; i++)
                {
                    ve = new Ve();
                    var maVe = "V" + random.Next(1, 1000);
                    ve.MaVe = maVe;
                    ve.MaDH = maDH;
                    ve.Hoten_KH = Request["HoTenKH_" + i];
                    ve.MaLVe = Request["MaLVe_" + i];
                    ve.GioiTinh = Request["GioiTinh_" + i];
                    ve.NgaySinh = Convert.ToDateTime(Request["NgaySinh_" + i]);
                    ve.LuuY = Request["LuuY_" + i];
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
               
                var updateBT = db.BookTours.Find(ve.MaDH);
                double tienbandau = TinhCKKhach(tongtien, booking.MaKH);
                updateBT.TotalPrice = tienbandau;
                db.SaveChanges();
                return RedirectToAction("OrderingInfo");
            }
            return RedirectToAction("OrderingInfo");
        }

        public ActionResult OrderingInfo()
        {
            BookTour infoOrder = db.BookTours.Find(Session["MaDH"]);
            //var infoOrder = db.BookTours.FirstOrDefault(s => s.MaDH == Session["MaDH"].ToString());
            return View(infoOrder);
        }

        [HttpGet]
        public ActionResult EditOrdering(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTour bookTour = db.BookTours.Find(id);
            if (bookTour == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaTour = new SelectList(db.Tours, "MaTour", "TenTour", bookTour.MaTour);
            return View(bookTour);
        }
        [HttpPost]
        public ActionResult EditOrdering(BookTour donHang)
        {
            if (ModelState.IsValid)
            {
                if (donHang.MaTour == null)
                {
                    TempData["ErrorTour"] = "Vui lòng chọn tour!!!";
                    return RedirectToAction("CreateOrder");
                }
                if (donHang.SoCho < 1)
                {
                    TempData["ErrorSoCho"] = "Số chỗ phải lớn hơn 1!!!";
                    return RedirectToAction("EditOrdering");
                }
                var tour = db.Tours.Find(donHang.MaTour);
                if (donHang.SoCho > tour.SoChoNull)
                {
                    TempData["ErrorSoChoMax"] = "Số chỗ vượt quá số chỗ còn trống";
                    return RedirectToAction("EditOrdering");
                }
                else
                {
                    db.Entry(donHang).State = EntityState.Modified;
                    db.SaveChanges();
                    if (donHang.TrangThaiTT == true)
                    {
                        TongtienDAT(donHang.MaKH);
                    }
                    return RedirectToAction("CreateTickets");
                }

            }
            Session["MaDHBook"] = donHang.MaDH;
            return View();
        }
        //Hàm tính tổng tiền đặt
        public ActionResult TongtienDAT(string makh)
        {
            var dh = db.BookTours.Where(s => s.MaKH == makh).ToList();
            var kh = db.KhachHangs.Where(s => s.MaKH == makh).FirstOrDefault();
            kh.TongTienDat = 0.0;
            foreach (var item in dh)
            {
                if (item.TrangThaiTT == true)
                {
                    kh.TongTienDat += item.TotalPrice;
                }
            }
            if (kh.TongTienDat >= 15000000)
            {
                kh.MaLoaiKH = "TT";
            }
            else if (kh.TongTienDat >= 50000000)
            {
                kh.MaLoaiKH = "VIP";
            }
            db.Entry(kh).State = EntityState.Modified;
            db.SaveChanges();
            return View();
        }
        public ActionResult DeleteOrdering(string id)
        {
            var ve = db.Ves.Where(s => s.MaDH == id).ToList();
            db.Ves.RemoveRange(ve);
            db.SaveChanges();

            var donHang = db.BookTours.Find(id);
            var tour = db.Tours.Find(donHang.MaTour);
            tour.SoChoNull += donHang.SoCho;
            db.BookTours.Remove(donHang);
            db.SaveChanges();
            return RedirectToAction("CheckCusInfo");
        }

        public ActionResult BookTourDetail(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BookTour bookTour = db.BookTours.Find(id);
            Session["MaBT"] = bookTour.MaDH;
            TempData["MaBT"] = bookTour.MaDH;

            if (id == null)
            {
                return HttpNotFound();
            }
            return View(bookTour);
        }
        [HttpPost]
        public ActionResult BookTourDetail(BookTour bookTour)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bookTour).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Cập nhật trạng thái thành công";
                return RedirectToAction("BookTourDetail");
            }
            return View();
        }

       
        //[HttpPost]
        //public ActionResult Bill()
        //{
        //    return View();
        //}

        [HttpGet]
        public ActionResult TicketEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ve ve = db.Ves.Find(id);
            if (ve == null)
            {
                return HttpNotFound();
            }
            return View(ve);
        }
        [HttpPost]
        public ActionResult TicketEdit(Ve ve)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ve).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Sửa vé thành công";
                return RedirectToAction("BookTourDetail/" + Session["MaBT"]);
            }
            return View();
        }

        //public ActionResult DeleteOrder(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BookTour donHang = db.BookTours.Find(id);
        //    if (donHang == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(donHang);
        //}
        //[HttpPost, ActionName("DeleteOrder")]
        //[ValidateAntiForgeryToken]

        public ActionResult DeleteBooking(string id, BookTour donHang)
        {
            try
            {
                donHang = db.BookTours.Where(s => s.MaDH == id).FirstOrDefault();
                if (donHang.TrangThaiTT == false)
                {
                    var ve = db.Ves.Where(s => s.MaDH == id).ToList();
                    db.Ves.RemoveRange(ve);
                    db.SaveChanges();

                    var tour = db.Tours.Find(donHang.MaTour);
                    tour.SoChoNull += donHang.SoCho;
                    db.BookTours.Remove(donHang);
                    db.SaveChanges();

                    TempData["bookingtour"] = "deletebooking";
                    return RedirectToAction("Booking");
                }
                else
                {
                    TempData["bookingtour"] = "loi";
                    return RedirectToAction("UpdateBooking", new RouteValueDictionary(
                                      new { controller = "OrderProcessing", action = "BookTourDeTail", Id = donHang.MaDH }));
                }

            }
            catch
            {
                return Content("err");
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult FeedBack()
        {
            return View(db.PhanHois.Where(s=>s.TieuDe == "Tư vấn đơn đặt tour").ToList());
        }

        public ActionResult DeleteFeedBack(string id)
        {
            PhanHoi phanHoi = db.PhanHois.Find(id);
            db.PhanHois.Remove(phanHoi);
            db.SaveChanges();
            return RedirectToAction("FeedBack");
        }

        public ActionResult FeedBackEdit(string id, PhanHoi phanHoi)
        {
            TempData["maFB"] = id;
            //PhanHoi phanHoi = db.PhanHois.Find(id);
            if (ModelState.IsValid)
            {
                db.Entry(phanHoi).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "Cập nhật phản hồi thành công";
                return RedirectToAction("FeedBack");
            }
            return RedirectToAction("FeedBack");
        }

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

        //THEO DOI HOA DON
        public ActionResult Booking()
        {
            List<BookTour> listbooktour = db.BookTours.ToList();
            return View(listbooktour);
        }

        public ActionResult UpdateBooking(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BookTour bookTour = db.BookTours.Find(id);

            Session["madonhangmail"] = id;
            if (id == null)
            {
                return HttpNotFound();
            }
            return View(bookTour);
        }
        
        [HttpPost]
        public ActionResult CapnhatBooking(BookTour bookTour)
        {
            if (ModelState.IsValid)
            {

                db.Entry(bookTour).State = EntityState.Modified;
                db.SaveChanges();
                if (bookTour.TrangThaiTT==true)
                {
                    string content = System.IO.File.ReadAllText(Server.MapPath("/Content/template/mailOrder.html"));
                    var kh = db.KhachHangs.Where(s => s.MaKH == bookTour.MaKH).FirstOrDefault();
                    var t = db.Tours.Where(s => s.MaTour == bookTour.MaTour).FirstOrDefault();
                    content = content.Replace("{{TenKH}}", kh.HoTenKH);
                    content = content.Replace("{{SDT}}", kh.SDT);
                    content = content.Replace("{{EmailKH}}", kh.Email);
                    content = content.Replace("{{Diachi}}", kh.DiaChi);

                    DateTime ngaydi = (DateTime)t.NgayKhoihanh;
                    TimeSpan aInterval = new System.TimeSpan(0, 1, 1, 0);
                    DateTime newTime = ngaydi.Subtract(aInterval);

                    content = content.Replace("{{MaTour}}", t.MaTour);
                    content = content.Replace("{{TenTour}}", t.TenTour);
                    content = content.Replace("{{ngaykhoihanh}}", t.NgayKhoihanh.ToString());
                    content = content.Replace("{{ngayve}}", t.NgayTroVe.ToString());
                    content = content.Replace("{{gianguoilon}}", String.Format("{0:00.0}", t.GiaNguoiLon.ToString()));
                    content = content.Replace("{{giatreem}}", String.Format("{0:00.0}", t.GiaTreEm.ToString()));

                    string hinhthuc = "";
                    if (bookTour.HinhThucThanhToan == true)
                    {
                        hinhthuc = "Chuyển khoản";
                    }
                    else
                    {
                        hinhthuc = "Thanh toán tại văn phòng";
                    }
                    content = content.Replace("{{hinhthuc}}", hinhthuc);
                    content = content.Replace("{{ngaydat}}", bookTour.NgayLap.ToString());
                    content = content.Replace("{{total}}", String.Format("{0:00.0}",bookTour.TotalPrice.ToString()));
                    content = content.Replace("{{MaDonHang}}", bookTour.MaDH);
               

                    //Gui mail
                    var toEmail = ConfigurationManager.AppSettings["toEmailAddress"].ToString();
                    new MailHelp().SendMail(kh.Email, "Thông báo", content);
                   

                }

                TempData["bookingtour"] = "editbookingtc";
                return RedirectToAction("UpdateBooking/" + bookTour.MaDH);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendMailCancel()
        {
            var madh = Session["madonhangmail"];
            string content = System.IO.File.ReadAllText(Server.MapPath("/Content/template/mailcancel.html"));

            var dh = db.BookTours.Where(s => s.MaDH == madh).FirstOrDefault();
            var kh = db.KhachHangs.Where(s => s.MaKH == dh.MaKH).FirstOrDefault();


            content = content.Replace("{{TenKH}}", kh.HoTenKH);
            content = content.Replace("{{MaDonHang}}", dh.MaDH);
            content = content.Replace("{{MaTour}}", dh.MaTour);
            content = content.Replace("{{NgayDat}}", dh.NgayLap.ToString());

            //Gui mail
            var toEmail = ConfigurationManager.AppSettings["toEmailAddress"].ToString();
            new MailHelp().SendMail(kh.Email, "Thông báo", content);
             TempData["cancelmail"] = "cancel";
            return RedirectToAction("UpdateBooking", new RouteValueDictionary(
                                  new { controller = "OrderProcessing", action = "BookTourDeTail", Id = madh }));

        }
        //XOA VE

        public ActionResult DeleteTicket(string id)
        {
            try
            {
                Ve ve = db.Ves.Where(v => v.MaVe == id).FirstOrDefault();
                var donhang = db.BookTours.Where(s => s.MaDH == ve.MaDH).FirstOrDefault();
                Session["matim"] = ve.MaDH;
                var tour = db.Tours.Where(s => s.MaTour == donhang.MaTour).FirstOrDefault();
                if(donhang.SoCho<=1)
                {
                    TempData["thongbao"] = "donhang1ve";
                    return RedirectToAction("BookTourDeTail", new RouteValueDictionary(
                                      new { controller = "OrderProcessing", action = "BookTourDeTail", Id = Session["matim"] }));
                }

                double tienve = 0;
                if (ve.MaLVe == "TICKET01")
                {
                   tienve=(double)tour.GiaNguoiLon;
                }
                else if (ve.MaLVe == "TICKET02")
                {
                    tienve = (double)tour.GiaTreEm;
                }
                tour.SoChoNull++;
                donhang.SoCho--;
                double tienvegiam = TinhCKKhach(tienve, donhang.MaKH);
                donhang.TotalPrice = donhang.TotalPrice - tienvegiam;
              
                db.Ves.Remove(ve);
                db.SaveChanges();
                TempData["ve"] = "xoave";

                return RedirectToAction("BookTourDeTail", new RouteValueDictionary(
                                       new { controller = "OrderProcessing", action = "BookTourDeTail", Id = Session["matim"] }));


            }
            catch
            {
                return Content("err");
            }


        }

        //public ActionResult AddTicket(string madh)
        //{
        //    return View();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTicket(string soluongdat, string madh)
        {
           
            var dh = db.BookTours.Where(t => t.MaDH == madh).FirstOrDefault();
            var tour = db.Tours.Where(s => s.MaTour == dh.MaTour).FirstOrDefault();
            double tongtien = 0;
            double pricesale = 0;
            int m = int.Parse(soluongdat);
            if (m == 0)
            {
                TempData["ve"] = "trong";
                return RedirectToAction("BookTourDeTail", new RouteValueDictionary(
                                       new { controller = "OrderProcessing", action = "BookTourDeTail", Id = madh }));
            }
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                //Tạo vé theo số lượng khách đặt
                for (int i = 1; i <= m; i++)
                {
                    Ve ve = new Ve();
                    var maVe = "V" + madh + rd.Next(1, 9) + rd.Next(1, 9) + rd.Next(1, 9) + rd.Next(1, 9);
                    ve.MaVe = maVe;
                    ve.MaDH = madh;
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
                    tour.SoChoNull--;
                    db.SaveChanges();

                }
                //Update tổng tiền cho đơn đặt tour
                var donhang = db.BookTours.Where(s => s.MaDH == madh).FirstOrDefault();
                double tienbandau = TinhCKKhach(tongtien, donhang.MaKH);
                dh.TotalPrice = dh.TotalPrice+tienbandau;
                dh.SoCho =dh.SoCho+ m;
                db.SaveChanges();
                TempData["ve"] = "themve";
                return RedirectToAction("BookTourDeTail", new RouteValueDictionary(
                                       new { controller = "OrderProcessing", action = "BookTourDeTail", Id = madh }));
            }
            return View();
        }

        [HttpGet]
        public ActionResult Bill(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BookTour bookTour = db.BookTours.Find(id);


            if (id == null)
            {
                return HttpNotFound();
            }
            return View(bookTour);
            
        }
    }
}