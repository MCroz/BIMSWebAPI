using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class Complainant : BaseEntity
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }

        public int? ResidentID { get; set; }
        public Resident Resident { get; set; }
    }
}