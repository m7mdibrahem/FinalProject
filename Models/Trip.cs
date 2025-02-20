using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Trip
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double Price { get; set; }
        public double Miles { get; set; }
        public DateOnly? ReturnDate { get; set; }
        [Required]
        public DateOnly? DepartDate { get; set; }
        public int First { get; set; }
        public int Business { get; set; }
        public int Premium { get; set; }
        public int Economy { get; set; }
        public string? Status { get; set; }
        public int CompanyId { get; set; }
        [ValidateNever]
        public Company Company { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
