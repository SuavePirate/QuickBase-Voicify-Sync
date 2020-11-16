using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickBase.VoicifySync.Models.QuickBase;
using QuickBase.VoicifySync.Models.Voicify;
using QuickBase.VoicifySync.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Voicify.Sdk.Core.Models.Webhooks.Events;
using Voicify.Sdk.Core.Models.Webhooks.Requests;
using Voicify.Sdk.Webhooks.Services;

namespace QuickBase.VoicifySync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoicifyController : ControllerBase
    {
        private readonly IConfiguration _config;

        public VoicifyController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost("ContentHit")]
        public async Task<IActionResult> ContentHit([FromHeader] string authorization, [FromBody] JObject request)
        {
            var tokenModel = TokenConvert.DeserializeEncryptedToken<WebhookTokenModel>(authorization, _config.GetValue<string>("EncodingKey") ?? "whoops");
            var recordProvider = new QuickBaseRecordProvider(new HttpClient(), tokenModel.QuickBaseRealm, tokenModel.QuickBaseToken);
            var requestModel = new UpdateRecordRequest
            {
                To = tokenModel.QuickBaseRequestTableId,
                Data = new List<Dictionary<string, RecordValue>>
                {
                    new Dictionary<string, RecordValue>
                    {
                        { tokenModel.QuickBaseRequestTableMatrix["applicationId"], new RecordValue(request["data"]["originalRequest"]["applicationId"].Value<string>()) },
                        { tokenModel.QuickBaseRequestTableMatrix["requestDate"], new RecordValue(request["data"]["eventDate"].Value<string>()) },
                        { tokenModel.QuickBaseRequestTableMatrix["platform"], new RecordValue(request["data"]["originalRequest"]["assistant"].Value<string>()) },
                        { tokenModel.QuickBaseRequestTableMatrix["requestId"], new RecordValue(request["data"]["originalRequest"]["requestId"]?.Value<string>() ?? Guid.NewGuid().ToString()) },
                        { tokenModel.QuickBaseRequestTableMatrix["userId"], new RecordValue(request["data"]["originalRequest"]["userId"].Value<string>()) },
                        { tokenModel.QuickBaseRequestTableMatrix["sessionId"], new RecordValue(request["data"]["originalRequest"]["sessionId"].Value<string>()) },
                        { tokenModel.QuickBaseRequestTableMatrix["slots"], new RecordValue(request["data"]["originalRequest"]["slots"].ToString()) },
                        { tokenModel.QuickBaseRequestTableMatrix["contentItemId"], new RecordValue(request["data"]["content"]["id"].Value<string>()) },
                        { tokenModel.QuickBaseRequestTableMatrix["featureTypeId"], new RecordValue(request["data"]["featureTypeId"].Value<string>()) }
                    }
                }
            };

            var result = await recordProvider.AddOrUpdateRecord(requestModel);

            if (result?.ResultType == ServiceResult.ResultType.Ok)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Errors);
        }
    }
}
