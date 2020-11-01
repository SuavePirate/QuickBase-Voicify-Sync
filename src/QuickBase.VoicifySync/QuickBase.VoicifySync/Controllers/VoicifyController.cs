using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuickBase.VoicifySync.Models.QuickBase;
using QuickBase.VoicifySync.Models.Voicify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Voicify.Sdk.Core.Models.Webhooks.Events;
using Voicify.Sdk.Core.Models.Webhooks.Requests;

namespace QuickBase.VoicifySync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoicifyController : ControllerBase
    {
        [HttpPost("ContentHit")]
        public async Task<IActionResult> ContentHit([FromBody]JsonElement request)
        {
            using var client = new HttpClient();

            var result = await client.PostAsync("https://api.quickbase.com/v1/records", new StringContent(JsonConvert.SerializeObject(new UpdateRecordRequest
            {
                To = "bqx3eq53s",
                Data = new List<Dictionary<string, RecordValue>>
                {
                    new Dictionary<string, RecordValue>
                    {
                        { "6", new RecordValue(request.GetProperty("data").GetProperty("originalRequest").GetProperty("assistant").GetString()) },
                        { "7", new RecordValue(request.GetProperty("data").GetProperty("originalRequest").GetProperty("userId").GetString()) },
                        { "8", new RecordValue(request.GetProperty("data").GetProperty("originalRequest").GetProperty("sessionId").GetString()) },
                        { "9", new RecordValue(request.GetProperty("data").GetProperty("originalRequest").GetProperty("requestId").GetString()) },
                        { "10", new RecordValue(request.GetProperty("data").GetProperty("content").GetProperty("id").GetString()) },
                        { "11", new RecordValue(request.GetProperty("data").GetProperty("featureTypeId").GetString()) }
                    }
                }
            }), Encoding.UTF8, "application/json"));

            return Ok();
        }
    }
}
