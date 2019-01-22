using BIMSWebAPI.Models;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BIMSWebAPI.Controllers
{
    public class UsersController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Users/GetUserList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserList()
        {
            List<Models.User> users = new List<Models.User>();
            using (var context = new BimsContext())
            {
                users = context.Users.ToList();
            }

            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Success,
                message = "Successfully Fetched",
                data = users
            });
        }

        [AllowAnonymous]
        [Route("api/Users/AddUser")]
        [HttpPost]
        public async Task<IHttpActionResult> AddUser(Models.User user)
        {
            //BIMSDb db = new BIMSDb();
            //db.QuickBind(new string[] { model.fname, model.lname, model.mname, model.username, model.password, model.role, model.updated_by, model.updated_by });
            //db.NonQuery("INSERT into users (fname,lname,mname,username,password,role,created_by,updated_by,date_created) VALUES (@1,@2,@3,@4,@5,@6,@7,@8,Current_Timestamp)");
            List<Models.User> users = new List<Models.User>();
            //db.CloseConnection();
            ResponseStatus thisStatus;
            using (var context = new BimsContext())
            {
                //Check if username already exist
                users = context.Users.Where(b => b.Username == user.Username).ToList();
                if (users.Count > 0)
                {
                    thisStatus = ResponseStatus.Fail;
                }
                else
                {
                    thisStatus = ResponseStatus.Success;
                    context.Users.Add(user);
                }
            }

            if (thisStatus == ResponseStatus.Success)
            {
                return Ok(new ResponseModel()
                {
                    status = thisStatus,
                    message = "Username Already Exist"
                });
            }
            else
            {
                return Ok(new ResponseModel()
                {
                    status = thisStatus,
                    message = "Successfully Added"
                });
            }


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

        [AllowAnonymous]
        [Route("api/Users/Test")]
        [HttpGet]
        public async Task<IHttpActionResult> Test()
        {
            using (var context = new BimsContext())
            {
                User user = new Models.User();
                user.FirstName = "Test 1";
                user.ModifiedBy = "Test 1";
                user.MiddleName = "Test 1";
                var std = context.Users.Add(user);
                context.SaveChanges();
            }


            return Ok(new ResponseModel()
            {
                status = ResponseStatus.Fail,
                message = "Deletion Failed"
            });
        }
    }
}
