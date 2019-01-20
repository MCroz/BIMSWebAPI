using BIMSWebAPI.Models;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace BIMSWebAPI.Controllers
{
    public class UsersController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Users/GetUserList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserList()
        {
            BIMSDb db = new BIMSDb();
            DataTable dT = db.Select("SELECT * from users");
            Object output = (Object)dT.Rows;


            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(dT);
            dT.Dispose();
            db.CloseConnection();
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Fetched",
                data = JsonConvert.DeserializeObject(JSONString)
            });
        }

        [AllowAnonymous]
        [Route("api/Users/AddUser")]
        [HttpPost]
        public async Task<IHttpActionResult> AddUser(UserModel model)
        {
            BIMSDb db = new BIMSDb();
            db.QuickBind(new string[] { model.fname, model.lname, model.mname, model.username, model.password, model.role, model.updated_by, model.updated_by });
            db.NonQuery("INSERT into users (fname,lname,mname,username,password,role,created_by,updated_by,date_created) VALUES (@1,@2,@3,@4,@5,@6,@7,@8,Current_Timestamp)");

            db.CloseConnection();
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Inserted"
            });
        }

        [AllowAnonymous]
        [Route("api/Users/DeleteUser/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            BIMSDb db = new BIMSDb();
            db.bind("1", id.ToString());
            int isSuccess = db.NonQuery("DELETE from users where id = @1");
            if (isSuccess > 0) {
                db.CloseConnection();
                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Deleted"
                });
            }

            db.CloseConnection();
            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Fail,
                message = "Deletion Failed"
            });
        }
    }
}
