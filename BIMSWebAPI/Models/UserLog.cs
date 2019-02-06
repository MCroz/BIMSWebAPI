using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class UserLog
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string ActionType { get; set; }
        public string Action { get; set; }
        public DateTime LoggedDate { get; set; }
    }
}