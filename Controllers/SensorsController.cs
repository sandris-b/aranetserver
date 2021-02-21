using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Dapper;

namespace AranetServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private SqLiteBaseRepository ctx;

        // GET: api/<SensorsController>
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Get()
        {
            try
            {
                var query = 
                @"WITH Dates AS
                ( 
                SELECT
                        sensor_id, 
                        MAX(rtime) last_measurement, 
                        rvalue, 
                        metric_id 
                        FROM    measures 
                        GROUP BY sensor_id) 
                SELECT * 
                FROM sensors 
                LEFT JOIN Dates USING (sensor_id) 
                LEFT JOIN metrics USING (metric_id) 
                LEFT JOIN units USING (unit_id)";
                using (ctx = new SqLiteBaseRepository())
                {
                    var result = ctx.db.Query(query);

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
