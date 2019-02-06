using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class User : BaseEntity
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public int? SecretQuestion1ID { get; set; }
        public SecretQuestion SecretQuestion1 { get; set; }

        public int? SecretQuestion2ID { get; set; }
        public SecretQuestion SecretQuestion2 { get; set; }

        public string SecretAnswer1 { get; set; }
        public string SecretAnswer2 { get; set; }

        public int Attempt { get; set; }
    }
}