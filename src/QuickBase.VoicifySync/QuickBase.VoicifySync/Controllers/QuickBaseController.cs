using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickBase.VoicifySync.Models;
using QuickBase.VoicifySync.Models.QuickBase;
using Voicify.Sdk.Cms.Api;
using Voicify.Sdk.Cms.Client;
using Voicify.Sdk.Core.Models.Model;

namespace QuickBase.VoicifySync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuickBaseController : ControllerBase
    {
        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem([FromBody]QuickBaseQuestionAnswerItem item)
        {
            try
            {
                var authApi = new AuthenticationApi("https://cms.voicify.com");

                // sign in with api user creds to get token
                var tokenResponse = await authApi.AuthenticateAsync(TempApiRecord.OrgId, TempApiRecord.OrgSecret, "password", TempApiRecord.Username, TempApiRecord.Token);
                var featureApi = new FeatureApi(new Configuration
                {
                    BasePath = "https://cms.voicify.com",
                    DefaultHeader = new Dictionary<string, string>
                    {
                        { "Authorization", $"Bearer {tokenResponse.AccessToken}" }
                    }
                });
                var questionAnswerApi = new QuestionAnswerApi(new Configuration
                {
                    BasePath = "https://cms.voicify.com",
                    DefaultHeader =new Dictionary<string, string>
                    {
                        { "Authorization", $"Bearer {tokenResponse.AccessToken}" }
                    }
                });

                // get root feature of app
                var features = await featureApi.GetFeaturesForApplicationAsync("d4b2f71a-e528-43d4-b4e5-9cf5a5862e3a");
                var rootFeature = features.FirstOrDefault(af => string.IsNullOrEmpty(af.ParentId));

                // add question answer to root feature
                var faq = await questionAnswerApi.CreateFullContentItemAsync(new QuestionAnswerModel(
                    applicationFeatureId: rootFeature.Id,
                    applicationId: rootFeature.ApplicationId,
                    isLive: true,
                    title: item.Question,
                    questionSet: new List<QuestionModel>
                {
                new QuestionModel(content: item.Question)
                },
                responses: new List<AnswerModel>
                {
                new AnswerModel(content: item.Answer)
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            
            return Ok();
        }
        
        [HttpGet("HealthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }
    }
}
