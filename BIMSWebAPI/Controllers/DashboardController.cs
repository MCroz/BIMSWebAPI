using BIMSWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BIMSWebAPI.Controllers
{
    [Authorize]
    public class DashboardController : ApiController
    {
        //[AllowAnonymous]
        [Route("api/Dashboard/GetDashboardInfo")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDashboardInfo()
        {

            DateTime startDate = DateTime.Now.Date;

            DateTime endDate = DateTime.Now.Date.AddDays(1);
            using (var context = new BimsContext())
            {
                var todayBCount = context.BarangayClearanceTransactions
                    .Where(b => b.DateCreated >= startDate && b.DateCreated < endDate).Count();

                var todayICount = context.IndigencyTransactions
                    .Where(b => b.DateCreated >= startDate && b.DateCreated < endDate).Count();

                var todayBCCount = context.BusinessClearanceTransactions
                    .Where(b => b.DateCreated >= startDate && b.DateCreated < endDate).Count();

                var todayDispenseCount = context.DispenseTransactions
                    .Where(b => b.DateCreated >= startDate && b.DateCreated < endDate).Count();

                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Fetched",
                    data = new {
                        BCount = todayBCount,
                        ICount = todayICount,
                        BCCount = todayBCCount,
                        DCount = todayDispenseCount
                    }
                });
            }
        }
    }
}