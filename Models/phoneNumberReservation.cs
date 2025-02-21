using System;

namespace Van_Rise_Intern_App.Models
{
    public class PhoneNumberReservation
    {
        public int ID { get; set; }
        public int Client_id { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BED { get; set; } // Beginning Effective Date
        public DateTime? EED { get; set; } // Ending Effective Date (nullable)
        public string ClientName { get; set; } // Optional, used when retrieving reservations with client details
    }
}
