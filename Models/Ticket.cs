using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public int TripId { get; set; }
        [ForeignKey("TripId")]
        public Trip Trip { get; set; }
        public int First { get; set; }
        public int Business { get; set; }
        public int Premium { get; set; }
        public int Economy { get; set; }
        public string? Description { get; set; }
        public double price { get; set; }
    }
}
