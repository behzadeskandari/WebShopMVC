using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WebShopMVC.Models
{
    public class Appointments
    {
        
        
        public int Id { get; set; }

        public DateTime AppointmentDate { get; set; }

        [NotMapped]
        public DateTime AppoinementTime { get; set; }

        public string CustomerName { get; set; }

        public string CustomerPhoneNumber { get; set; }

        public string CustomerEmail { get; set; }

        public bool isConfirmed { get; set; }
    }
}
