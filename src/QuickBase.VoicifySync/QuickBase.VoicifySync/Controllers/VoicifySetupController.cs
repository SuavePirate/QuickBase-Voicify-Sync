using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voicify.Sdk.Core.Models.Integrations.Setup;

namespace QuickBase.VoicifySync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoicifySetupController : ControllerBase
    {
        [HttpPost("Setup")]
        public async Task<IActionResult> Setup([FromBody] IntegrationSetupRequest request)
        {
            return Ok();
        }

        [HttpPost("Config")]
        public async Task<IActionResult> Config([FromBody] IntegrationSetupRequest request)
        {
            return Ok();
        }
    }
}
