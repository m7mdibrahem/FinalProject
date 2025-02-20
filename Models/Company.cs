using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "this field is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "this field is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "this filed is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "this field is required")]
        public string Address { get; set; }
        [ValidateNever]
        public string ImgUrl { get; set; }
        [ValidateNever]
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
