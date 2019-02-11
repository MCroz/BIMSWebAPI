using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BIMSWebAPI.Controllers
{
    public class BusinessController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Business/GetBusinessList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBusinessList()
        {
            using (var context = new BimsContext())
            {
                //var outVal = context.Businesses.Where(o => o.isRemoved == 0).ToList();
                var outVal = (from bus in context.Businesses
                              join o in context.Owners on bus.OwnerID equals o.ID
                              where bus.isRemoved == 0
                              select new
                              {
                                  bus.KindOfBusiness,
                                  bus.BusinessName,
                                  bus.BusinessAddress,
                                  bus.BusinessContactNo,
                                  bus.FloorArea,
                                  bus.Capitalization,
                                  bus.DTI_SEC_RegNo,
                                  bus.ID,
                                  OwnerName = o.FirstName + " " + o.MiddleName + " " +o.LastName,
                                  OwnerAddress = o.Address
                              }).ToList();

                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Fetched",
                    data = outVal
                });
            }
        }

        [AllowAnonymous]
        [Route("api/Business/GetBusinessInfo/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBusinessInfo(int id)
        {
            string message;
            ResponseStatus resp;
            
            using (var context = new BimsContext())
            {
                var business = (from bus in context.Businesses
                                join owner in context.Owners on bus.OwnerID equals owner.ID
                                where bus.ID == id
                                select new
                                {
                                    bus.KindOfBusiness,
                                    bus.ID,
                                    bus.BusinessAddress,
                                    bus.BusinessName,
                                    bus.BusinessContactNo,
                                    bus.FloorArea,
                                    bus.DTI_SEC_RegNo,
                                    bus.Capitalization,
                                    OwnerName = owner.FirstName + " " + owner.MiddleName + " " + owner.LastName,
                                    OwnerAddress = owner.Address,
                                    OwnerContactNo = owner.ContactNo
                                }).FirstOrDefault();

                if (business == null)
                {
                    message = "No Business Found";
                    resp = ResponseStatus.Fail;
                    return Ok(new ResponseModel()
                    {
                        status = resp,
                        message = message,
                        data = business
                    });
                }
                else
                {
                    message = "Successfully Fetched";
                    resp = ResponseStatus.Success;
                    return Ok(new ResponseModel()
                    {
                        status = resp,
                        message = message,
                        data = business
                    });
                }
            }

        }
    }
}