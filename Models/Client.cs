using System;

namespace Van_Rise_Intern_App.Models
{
    public enum ClientType
    {
        Individual = 1,
        Organization = 2
    }

    public class Client
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ClientType Type { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
