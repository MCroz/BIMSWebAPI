using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class DispenseTransaction : BaseEntity
    {
        public int ID { get; set; }

        public int ResidentID { get; set; }
        public Resident Resident { get; set; }
        
        public string PrescriptionDescription { get; set; }

        public IList<InventoryMovement> InventoryMovement { get; set; }
    }
}