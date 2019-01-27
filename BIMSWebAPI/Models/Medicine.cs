using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class Medicine : BaseEntity
    {
        public int ID { get; set; }
        public string MedicineName { get; set; }
        public string Description { get; set; }

        public IList<Stock> Stocks { get; set; }
    }
}