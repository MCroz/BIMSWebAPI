using BIMSWebAPI.Models;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Linq;

namespace BIMSWebAPI.Controllers
{
    public class LoginController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Login/InitialLogin")]
        [HttpPost]
        public async Task<IHttpActionResult> InitialLogin(LoginModel model)
        {
            if (model.username.Trim() == "" || model.password.Trim() == "")
            {
                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Fail,
                    message = "Please Fill All The Fields."
                });
            }
            User user;
            using (var context = new BimsContext())
            {
                user = context.Users.Where( b => b.Username == model.username && b.Password == model.password).FirstOrDefault();
            }

            if (user != null)
            {
                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Logged-In",
                    data = user
                });
            }

            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Fail,
                message = "Invalid Username/Password Combination"
            });
        }
    }
}