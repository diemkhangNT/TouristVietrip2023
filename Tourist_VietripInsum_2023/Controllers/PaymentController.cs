﻿using DemoVNPay.Others;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tourist_VietripInsum_2023.common;
using Tourist_VietripInsum_2023.DesignPattern.TemplateMethod;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.Controllers
{
    public class PaymentController : TemplateMethodController
    {
        TouristEntities1 db = new TouristEntities1();

        public PaymentController()
        {
            var result = PrintInfo();
            Debugger.Log(1, "Logger: ", $"{result}");
        }

        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Payment()
        {
            var madh = (string)Session["madonhang"];
            var donhang = db.BookTours.Where(s => s.MaDH == madh).FirstOrDefault();
            var amount = Convert.ToString(donhang.TotalPrice)+"00";

            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", amount); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng " + madh); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        TempData["messageSC"] = "Thanh toan thanh cong hoa don " + orderId + " | Ma giao dich: " + vnpayTranId;
                        var madh = (string)Session["madonhang"];
                        var donhang = db.BookTours.Where(s => s.MaDH == madh).FirstOrDefault();
                        donhang.TrangThaiTT = true;
                        db.Entry(donhang).State = EntityState.Modified;
                        db.SaveChanges();
                        //Update tổng tiền đặt
                        var kh = db.KhachHangs.Where(s => s.MaKH == donhang.MaKH).FirstOrDefault();
                        kh.TongTienDat = kh.TongTienDat + donhang.TotalPrice;
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
                        //gửi mail
                        //Email thanh toán online
                        string content = System.IO.File.ReadAllText(Server.MapPath("/Content/template/mailconn.html"));
                        
                        var t = db.Tours.Where(s => s.MaTour == donhang.MaTour).FirstOrDefault();

                        content = content.Replace("{{TenKH}}", kh.HoTenKH);
                        content = content.Replace("{{Phoneno}}", kh.SDT);
                        content = content.Replace("{{MaDH}}", donhang.MaDH);
                        content = content.Replace("{{Email}}", kh.Email);
                        content = content.Replace("{{Address}}", kh.DiaChi);
                        string hinhthuc = "Chuyển khoản";


                        DateTime ngaydat = (DateTime)donhang.NgayLap;
                        DateTime hanthanhtoan = ngaydat.AddDays(1);
                        DateTime ngaydi = (DateTime)t.NgayKhoihanh;
                        TimeSpan aInterval = new System.TimeSpan(0, 1, 1, 0);
                        DateTime newTime = ngaydi.Subtract(aInterval);
                        content = content.Replace("{{hinhthuc}}", hinhthuc);
                        content = content.Replace("{{ngaydat}}", ngaydat.ToString());
                        content = content.Replace("{{hanthanhtoan}}", hanthanhtoan.ToString());
                        content = content.Replace("{{MaTour}}", t.MaTour);
                        content = content.Replace("{{TenTour}}", t.TenTour);
                        content = content.Replace("{{ngaykhoihanh}}", t.NgayKhoihanh.ToString());
                        content = content.Replace("{{noikhoihanh}}", t.NoiKhoiHanh);
                        content = content.Replace("{{ngayve}}", t.NgayTroVe.ToString());
                        content = content.Replace("{{hanchotve}}", t.HanChotDatVe.ToString());
                        content = content.Replace("{{total}}", donhang.TotalPrice.ToString());
                        content = content.Replace("{{nguoilon}}", t.GiaNguoiLon.ToString());
                        content = content.Replace("{{treem}}", t.GiaTreEm.ToString());
                        content = content.Replace("{{tieude}}", "XÁC NHẬN ĐẶT TOUR - THANH TOÁN QUA VNPAY THÀNH CÔNG");
                        content = content.Replace("{{noidung}}", "SaigonTravels xác nhận quý khách đã đăng ký tour thành công. ");

                        ////Gui mail
                        var toEmail = ConfigurationManager.AppSettings["toEmailAddress"].ToString();
                        new MailHelp().SendMail(kh.Email, "Xác nhận thanh toán tour thành công", content);
                        return RedirectToAction("HomePageGuest","Guest");
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        TempData["messageF"] = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        return RedirectToAction("PaymentFailed");
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }

        public ActionResult PaymentSuccess()
        {
            return View();
        }

        public ActionResult PaymentFailed()
        {
            return View();
        }

        public override string PrintRoutes()
        {
            return "========================" +
                "Payment Controller is running!" +
                "======================";
        }

        public override string PrintDIs()
        {
            return "=================No dependence Injection================\n";
        }
    }
}