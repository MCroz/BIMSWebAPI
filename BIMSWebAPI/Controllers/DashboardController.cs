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
    public class DashboardController : ApiController
    {
        [AllowAnonymous]
        [Route("api/Dashboard/GetDashboardInfo")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDashboardInfo()
        {
            using (var context = new BimsContext())
            {
                var model = context.BarangayClearanceTransactions
                    .Where(b => b.DateCreated.Value.Year == DateTime.Now.Year)
                    .GroupBy(o => new
                    {
                        Month = o.DateCreated.Value.Month
                    })
                    .Select(g => new
                    {
                        Month = g.Key.Month,
                        Total = g.Count()
                    })
                    .OrderByDescending(a => a.Month)
                    .ToList();

                return Ok(new ResponseModel()
                {
                    status = ResponseStatus.Success,
                    message = "Successfully Fetched",
                    data = model
                });
            }
        }
    }
}