using BIMSWebAPI.Models;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Linq;
using BIMSWebAPI.App_Code;

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
            
            using (var context = new BimsContext())
            {
                User user;
                user = context.Users.Where( b => b.Username == model.username).FirstOrDefault();

                if (user != null)
                {
                    if (user.Attempt >= 4)
                    {
                        AuditLogHelper.GenerateLog(context,"Login", model.username + " Logged In To The System. Exceeded Maximum Attempt");
                        return Ok(new ResponseModel()
                        {
                            status = ResponseStatus.Fail,
                            message = "This Account Reached the Maximum Number of Attempt to Log-In. This Account is now Blocked."
                        });
                    }

                    if (user.Password != model.password)
                    {
                        AuditLogHelper.GenerateLog(context, "Login", model.username + " Logged In, Invalid Password.");
                        user.Attempt += 1;
                        context.SaveChanges();
                        if (user.Attempt >= 4)
                        {
                            return Ok(new ResponseModel()
                            {
                                status = ResponseStatus.Fail,
                                message = "Incorrect Password. This Account Reached the Maximum Number of Attempt to Log-In. This Account is now Blocked."
                            });
                        }
                        return Ok(new ResponseModel()
                        {
                            status = ResponseStatus.Fail,
                            message = "Incorrect Password"
                        });
                    }

                    user.Attempt = 0;
                    AuditLogHelper.GenerateLog(context, "Login", model.username + " Logged In Successfully.");
                    context.SaveChanges();
                    return Ok(new ResponseModel()
                    {
                        status = ResponseStatus.Success,
                        message = "Successfully Logged-In",
                        data = user
                    });
                }
            }



            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Fail,
                message = "No Account Match for the Entered Username"
            });
        }
    }
}