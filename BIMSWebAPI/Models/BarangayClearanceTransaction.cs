using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class BarangayClearanceTransaction : BaseEntity
    {
        public int ID { get; set; }

        public int ResidentID { get; set; }
        public Resident Resident { get; set; }

        public string Purpose { get; set; }

        public string ControlNo { get; set; }
    }
}