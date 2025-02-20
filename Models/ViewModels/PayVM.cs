using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class PayVM
    {
        public List<Payment>Payments { get; set; }= new List<Payment>();
        public string Stat { get; set; }
    }
}
