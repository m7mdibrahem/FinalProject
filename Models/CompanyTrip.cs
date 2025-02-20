using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [PrimaryKey("CompanyId", "TripId")]
    public class CompanyTrip
    {
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public int TripId { get; set; }
        [ForeignKey("TripId")]
        public Trip Trip { get; set; }
    }
}
