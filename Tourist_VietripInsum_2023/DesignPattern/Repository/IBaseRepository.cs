using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tourist_VietripInsum_2023.Models;

namespace Tourist_VietripInsum_2023.DesignPattern.Repository
{
    public interface IBaseRepository
    {
        string RandomPassword(int numberUpTo);
        void TongtienDAT(string makh, TouristEntities1 db); // to update Customer type
    }
}
