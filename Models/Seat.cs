using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Seat
    {
        public int Id { get; set; }
        public TripType Degree { get; set; }
        public Availability? availability { get; set; }
        public double Price { get; set; }
        public int Number { get; set; }
        public int TripId { get; set; }
        [ValidateNever]
        public Trip Trip { get; set; }
    }
}
