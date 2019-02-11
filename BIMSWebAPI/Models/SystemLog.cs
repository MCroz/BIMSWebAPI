using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class SystemLog
    {
        public int ID { get; set; }
        public string LogAction { get; set; }
        public string LogType { get; set; }
        public DateTime LogTime { get; set; }

        //LogType
        //Login
        //Logout

    }
}