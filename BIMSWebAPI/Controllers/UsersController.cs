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
            Models.User exist;
            ResponseStatus thisStatus;
            using (var context = new BimsContext())
            {
                //Check if username already exist
                exist = context.Users.Where(b => b.Username == user.Username).FirstOrDefault();
                if (exist != null)
                {
                    thisStatus = ResponseStatus.Fail;
                }
                else
                {
                    thisStatus = ResponseStatus.Success;
                    user.Attempt = 0;
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }

            if (thisStatus == ResponseStatus.Fail)
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
            string message;
            ResponseStatus resp;
            using (var context = new BimsContext())
            {
                var user = context.Users.Find(id);
                if (user == null)
                {
                    message = "No User Found";
                    resp = ResponseStatus.Fail;
                }
                else
                {
                    context.Users.Remove(user);
                    context.SaveChanges();
                    message = "Successfully Removed";
                    resp = ResponseStatus.Success;
                }
            }
            return Ok(new ResponseModel()
            {
                status = resp,
                message = message
            });
        }

        [AllowAnonymous]
        [Route("api/Users/GetUserInfo/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserInfo(int id)
        {
            string message;
            ResponseStatus resp;
            Models.User user;
            using (var context = new BimsContext())
            {
                user = context.Users.Find(id);
                if (user == null)
                {
                    message = "No User Found";
                    resp = ResponseStatus.Fail;
                }
                else
                {
                    message = "Successfully Removed";
                    resp = ResponseStatus.Success;
                }
            }
            return Ok(new ResponseModel()
            {
                status = resp,
                message = message,
                data = user
            });
        }

        [AllowAnonymous]
        [Route("api/Users/UpdateUser")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateUser(Models.User user)
        {
            Models.User exist;
            ResponseStatus thisStatus;
            using (var context = new BimsContext())
            {
                //Check if username already exist
                exist = context.Users.Where(b => b.Username == user.Username && b.ID != user.ID).FirstOrDefault();
                if (exist != null)
                {
                    thisStatus = ResponseStatus.Fail;
                }
                else
                {
                    var selectedUser = context.Users.Find(user.ID);
                    selectedUser.FirstName = user.FirstName;
                    selectedUser.MiddleName = user.MiddleName;
                    selectedUser.LastName = user.LastName;
                    selectedUser.Role = user.Role;
                    selectedUser.Username = user.Username;
                    selectedUser.Password = user.Password;
                    selectedUser.ModifiedBy = user.ModifiedBy;

                    thisStatus = ResponseStatus.Success;
                    context.SaveChanges();
                }
            }

            if (thisStatus == ResponseStatus.Fail)
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
                    message = "Successfully Updated"
                });
            }
        }
    }
}
