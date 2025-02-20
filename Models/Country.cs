using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Country
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name field is required")]
        public string Name { get; set; }
        [ValidateNever]
        public string ImgUrl { get; set; }
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
