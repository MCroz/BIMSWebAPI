using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class Blotter : BaseEntity
    {
        public int ID { get; set; }

        public int ComplainantID { get; set; }
        public Complainant Complainant { get; set; }

        public int AccusedID { get; set; }
        public Accused Accused { get; set; }

        //For Report
        public string TypeOfIncident { get; set; }
        public DateTime IncidentDateTime { get; set; }

        public string AddressNo { get; set; }
        public string AddressSt { get; set; }
        public string AddressZone { get; set; }

        public string NarrativeReport { get; set; }
        public string Status { get; set; }
    }
}