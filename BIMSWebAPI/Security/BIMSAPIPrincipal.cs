using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace BIMSWebAPI.Security
{
    public class BIMSAPIPrincipal : IPrincipal
    {
        //Constructor
        public BIMSAPIPrincipal(User curUser)
        {
            UserName = curUser.ID.ToString();
            Identity = new GenericIdentity(curUser.ID.ToString());
            User = curUser;
        }

        public string UserName { get; set; }
        public IIdentity Identity { get; set; }
        public User User { get; set; }

        public bool IsInRole(string role)
        {
            if (role.Equals("user"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}