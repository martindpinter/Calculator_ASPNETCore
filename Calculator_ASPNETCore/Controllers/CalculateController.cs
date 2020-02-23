using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Calculator_ASPNETCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculateController : ControllerBase
    {
        // POST: api/Calculate
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return ExpressionEvaluator.Evaluate(value);
        }
    }
}
