using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIMSWebAPI.Models
{
    public class LoginModel
    {
        public String username { get; set; }
        public String password { get; set; }
    }

    public class ResponseModel
    {
        public ResponseStatus status { get; set; }
        public String message { get; set; }
        public Object data { get; set; }
    }

    public enum ResponseStatus
    {
        Fail,
        Success
    }

    public class UserModel
    {
        public String fname { get; set; }
        public String mname { get; set; }
        public String lname { get; set; }
        public String username { get; set; }
        public String password { get; set; }
        public String role { get; set; }
        public String updated_by { get; set; }
    }
}