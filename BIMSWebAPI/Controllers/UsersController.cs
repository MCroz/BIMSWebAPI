﻿using BIMSWebAPI.Models;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using BIMSWebAPI.App_Code;
using System.Diagnostics;
using System.Reflection;

namespace BIMSWebAPI.Controllers
{
    [AllowAnonymous]
    public class UsersController : ApiController
    {
        //[AllowAnonymous]
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

        //[AllowAnonymous]
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

                    var currentUser = context.Users.Find(user.ModifiedBy);
                    string logChanges = "";
                    foreach (PropertyInfo pi in user.GetType().GetProperties())
                    {
                        //pi.GetValue(myClass, null)?.ToString();
                        if (pi.Name != "ModifiedBy" && pi.Name != "CreatedBy" && pi.Name != "DateModified" && pi.Name != "DateCreated" && pi.Name != "ID")
                        {
                            if (logChanges != "")
                            {
                                logChanges += ", ";
                            }
                            logChanges += pi.GetValue(user, null)?.ToString() != "" ? pi.Name + " = " + pi.GetValue(user, null)?.ToString() : "";
                        }
                    }
                    string changes = currentUser.Username + " Added A User and Set: " + logChanges;
                    AuditLogHelper.GenerateLog(context, "Create", changes);


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

        //[AllowAnonymous]
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

        //[AllowAnonymous]
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

        //[AllowAnonymous]
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
                    selectedUser.ModifiedBy = user.ModifiedBy;

                    thisStatus = ResponseStatus.Success;
                    var currentUser = context.Users.Find(user.ModifiedBy);
                    string changes = currentUser.Username + " Updated User: " + AuditLogHelper.FetchChanges(context);
                    AuditLogHelper.GenerateLog(context, "Update", changes);

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


        //[AllowAnonymous]
        [Route("api/Users/ResetPassword/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> ResetPassword(int id)
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
                    //context.Users.Remove(user);
                    user.Password = "1234";
                    AuditLogHelper.GenerateLog(context, "Update", "Reset " + user.Username + " Password");
                    context.SaveChanges();
                    message = "Password Reset is Successfully. New Password is '1234'";
                    resp = ResponseStatus.Success;
                }
            }
            return Ok(new ResponseModel()
            {
                status = resp,
                message = message
            });
        }

        //[AllowAnonymous]
        [Route("api/Users/UnblockUser/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> UnblockUser(int id)
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
                    user.Attempt = 0;
                    AuditLogHelper.GenerateLog(context, "Update", "Unblock " + user.Username);
                    context.SaveChanges();
                    message = "Successfully Unblocked the User.";
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
        [Route("api/Users/ForgotPassword")]
        [HttpPost]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            string message;
            ResponseStatus resp;
            using (var context = new BimsContext())
            {
                var user = context.Users.Where(u => u.Username == model.Username).FirstOrDefault();
                if (user == null)
                {
                    message = "No Matching Username Found";
                    resp = ResponseStatus.Fail;
                }
                else
                {
                    if (user.SecretQuestion1ID == model.SecretQuestion1 && user.SecretQuestion2ID == model.SecretQuestion2 && user.SecretAnswer1 == model.SecretAnswer1 && user.SecretAnswer2 == model.SecretAnswer2)
                    {
                        user.Password = "1234";
                        context.SaveChanges();
                        message = "Password Reset Successful. Your New Password is '1234'";
                        resp = ResponseStatus.Success;
                    }
                    else
                    {
                        message = "Invalid Secret Answer/Secret Question Combination.";
                        resp = ResponseStatus.Fail;
                    }
                }
            }
            return Ok(new ResponseModel()
            {
                status = resp,
                message = message
            });
        }

        //[AllowAnonymous]
        [Route("api/Users/FirstTimeLogin")]
        [HttpPost]
        public async Task<IHttpActionResult> FirstTimeLogin(FirstTimeLoginModel model)
        {
            string message;
            ResponseStatus resp;
            using (var context = new BimsContext())
            {
                var user = await context.Users.FindAsync(model.UserID);
                if (user == null)
                {
                    message = "No User Found";
                    resp = ResponseStatus.Fail;
                }
                else
                {
                    user.ModifiedBy = model.UserID;
                    user.SecretAnswer1 = model.SecretAnswer1;
                    user.SecretAnswer2 = model.SecretAnswer2;
                    user.SecretQuestion1ID = model.SecretQuestion1;
                    user.SecretQuestion2ID = model.SecretQuestion2;
                    user.Password = model.Password;
                    context.SaveChanges();

                    message = "Successfully Updated.";
                    resp = ResponseStatus.Success;
                }
                return Ok(new ResponseModel()
                {
                    status = resp,
                    message = message,
                    data = user
                });
            }

        }


        //[AllowAnonymous]
        [Route("api/Users/UpdateUserProfile")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateUserProfile(UpdateUserProfileModel model)
        {
            string message;
            ResponseStatus resp;
            using (var context = new BimsContext())
            {
                var user = await context.Users.FindAsync(model.UserID);
                if (user == null)
                {
                    message = "No User Found";
                    resp = ResponseStatus.Fail;
                }
                else
                {
                    user.ModifiedBy = model.UserID;
                    user.SecretAnswer1 = model.SecretAnswer1;
                    user.SecretAnswer2 = model.SecretAnswer2;
                    user.SecretQuestion1ID = model.SecretQuestion1;
                    user.SecretQuestion2ID = model.SecretQuestion2;
                    user.FirstName = model.FirstName;
                    user.MiddleName = model.MiddleName;
                    user.LastName = model.LastName;

                    if (model.UpdatePassword)
                    {
                        if (model.CurrentPassword != user.Password)
                        {
                            return Ok(new ResponseModel()
                            {
                                status = ResponseStatus.Fail,
                                message = "Current Password Doesn't Match."
                            });
                        }
                        if (model.CurrentPassword == model.Password)
                        {
                            return Ok(new ResponseModel()
                            {
                                status = ResponseStatus.Fail,
                                message = "Current Password and New Password Shouldn't Be Equal."
                            });
                        }

                        user.Password = model.Password;
                    }
                    context.SaveChanges();

                    message = "Successfully Updated.";
                    resp = ResponseStatus.Success;
                    return Ok(new ResponseModel()
                    {
                        status = resp,
                        message = message,
                        data = user
                    });
                }
            }
            return Ok(new ResponseModel()
            {
                status = resp,
                message = message
            });
        }
    }

    public class UpdateUserProfileModel
    {
        public int UserID { get; set; }
        public int SecretQuestion1 { get; set; }
        public string SecretAnswer1 { get; set; }
        public int SecretQuestion2 { get; set; }
        public string SecretAnswer2 { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool UpdatePassword { get; set; }
    }

    public class FirstTimeLoginModel
    {
        public int UserID { get; set; }
        public int SecretQuestion1 { get; set; }
        public string SecretAnswer1 { get; set; }
        public int SecretQuestion2 { get; set; }
        public string SecretAnswer2 { get; set; }
        public string Password { get; set; }
    }


    public class ForgotPasswordModel
    {
        public string Username { get; set; }
        public int SecretQuestion1 { get; set; }
        public string SecretAnswer1 { get; set; }
        public int SecretQuestion2 { get; set; }
        public string SecretAnswer2 { get; set; }
    }
}
