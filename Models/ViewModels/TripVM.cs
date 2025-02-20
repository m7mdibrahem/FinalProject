using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class TripVM
    {
        public List<Company> Companies { get; set; } = new List<Company>();
        public List<City> Cities { get; set; } = new List<City>();
        public Trip Trip { get; set; } = new Trip();
    }
}
