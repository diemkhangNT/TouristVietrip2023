using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tourist_VietripInsum_2023.DesignPattern.Prototype
{
    public interface ITransportPrototype
    {
        ITransportPrototype Clone(); //apply in line 269 (TourmanagerController)
    }
}
