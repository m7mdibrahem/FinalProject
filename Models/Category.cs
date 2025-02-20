using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Name field is required")]
        public string Name { get; set; }
        public ICollection<CityCategory> CityCategories { get; set; } = new List<CityCategory>();
    }
}
