using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class IntroVM
    {
        public List<City> Cities { get; set; } = new List<City>();
        public List<Company> Companies { get; set; } = new List<Company>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public Category Category { get; set; } = new Category();
    }
}
