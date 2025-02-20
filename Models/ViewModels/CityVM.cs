using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class CityVM
    {
        public City City { get; set; } = new City();
        public List<Country> Countries { get; set; } = new List<Country>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<CityCategory> CityCategories { get; set; } = new List<CityCategory>();
    }
}
