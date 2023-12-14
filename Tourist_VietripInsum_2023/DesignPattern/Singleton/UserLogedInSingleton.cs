using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.DesignPattern.Singleton
{
    public class UserLogedInSingleton<TEntity>
        where TEntity : class
    {
        public static UserLogedInSingleton<TEntity> Instance { get; } = new UserLogedInSingleton<TEntity>();
        public List<TEntity> Users { get; } = new List<TEntity>();
        public void InitSingleton(TouristEntities1 _context)                      //apply in line 29 (GuestController) 
        {                                                                         //apply in line 22 (LoginStaffController)
            if(typeof(TEntity).IsAssignableFrom(typeof(NhanVien)) && Users.Count == 0)
            {
                var listUsers = _context.NhanViens.ToList();
                foreach(var user in listUsers)
                {
                    Users.Add(user as TEntity);
                }
            }
            else if(typeof(TEntity).IsAssignableFrom(typeof(KhachHang)) && Users.Count == 0)
            {
                var listUsers = _context.KhachHangs.ToList();
                foreach (var user in listUsers)
                {
                    Users.Add(user as TEntity);
                }
            }
        }

        public void UpdateSigleton(TouristEntities1 _context)    //apply in line 164 (AdminController)
        {
            Instance.Users.Clear();
            InitSingleton(_context);
        }
    }
}