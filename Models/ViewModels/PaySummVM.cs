using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class PaySummVM
    {
        public Payment Payment { get; set; } = new Payment();
        public List<int> Numbers = new List<int>();
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}
