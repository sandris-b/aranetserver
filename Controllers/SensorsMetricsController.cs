using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AranetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SensorsMetricsController : ControllerBase
    {
        private SqLiteBaseRepository ctx;

        // POST api/<SensorsMetricsController>
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] InputModel value)
        {
            try
            {
                if(!DateTime.TryParse(value.Date, out DateTime resultDate))
                {
                    return Problem("Date is in invalid format, please use yyyy-MM-dd format!");
                }
                var query =
                @"WITH data AS 
                (
                SELECT  
		                sensor_id,
                        MAX(measures.rvalue) max_measurement,
		                MIN(measures.rvalue) min_measurement,
		                metric_id,
                        measures.rtime AS date
                        FROM measures
                        GROUP BY measures.metric_id, sensor_id
                )

                SELECT  *
                FROM    sensors
		                LEFT JOIN data USING (sensor_id)
		                LEFT JOIN metrics USING (metric_id)
		                LEFT JOIN units USING (unit_id) 
                        WHERE date(data.date) = date(@param);";

                string param = resultDate.ToString("yyyy-MM-dd");

                using (ctx = new SqLiteBaseRepository())
                {
                    var result = ctx.db.Query(query, new { param = param } );

                    return Content(await Task.Run(() =>
                    JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore })), "application/json");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
