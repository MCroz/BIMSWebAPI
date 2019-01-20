using BIMSWebAPI.Models;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

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
            BIMSDb db = new BIMSDb();
            db.QuickBind(new string[] { model.username, model.password });
            DataTable dT = db.Select("SELECT * from users where username = @1 AND password = @2");
            if (dT.Rows.Count == 0)
            {
                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Fail,
                    message = "Invalid Username/Password Combination"
                });
            }
            string output = JsonConvert.SerializeObject(dT);
            dT.Dispose();
            db.CloseConnection();
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Logged-In",
                data = JsonConvert.DeserializeObject(output)
            });
        }
    }
}