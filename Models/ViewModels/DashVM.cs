using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class DashVM
    {
        public int Booked { get; set; }
        public int Done { get; set; }
        public int Cancel { get; set; }
        public double TotalPrice { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public List<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
        public List<City> Cities { get; set; } = new List<City>();
        public List<double> Percentages { get; set; }=new List<double>();
    }
}
