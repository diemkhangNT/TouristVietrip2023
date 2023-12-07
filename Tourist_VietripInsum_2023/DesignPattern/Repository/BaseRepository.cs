using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.DesignPattern.Repository
{
    public class BaseRepository : IBaseRepository
    {
        public string RandomPassword(int numberUpTo)
        {
            var chars = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            var password = string.Empty;
            var random = new Random();
            for (var i = 0; i < numberUpTo; i++)
            {
                var x = random.Next(1, chars.Length);
                if (!password.Contains(chars.GetValue(x).ToString()))
                {
                    password += chars.GetValue(x);
                }
                else
                {
                    i--;
                }
            }

            return password;
        }

        public void TongtienDAT(string makh, TouristEntities1 db)
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
        }
    }
}