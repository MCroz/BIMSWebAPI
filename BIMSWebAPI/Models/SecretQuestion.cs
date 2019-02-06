using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class SecretQuestion
    {
        public int ID { get; set; }
        public string Question { get; set; }
    }
}