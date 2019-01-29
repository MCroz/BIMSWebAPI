using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class Resident : BaseEntity
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string CivilStatus { get; set; }

        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }
        public string AddressNo { get; set; }
        public string AddressSt { get; set; }
        public string AddressZone { get; set; }
        public string BirthPlace { get; set; }
        public string Citizenship { get; set; }

        public string Image { get; set; }
    }
}