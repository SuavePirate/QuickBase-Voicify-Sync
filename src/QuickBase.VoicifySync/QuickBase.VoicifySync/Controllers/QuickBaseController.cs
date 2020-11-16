using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuickBase.VoicifySync.Models;
using QuickBase.VoicifySync.Models.QuickBase;
using QuickBase.VoicifySync.Models.Voicify;
using Voicify.Sdk.Cms.Api;
using Voicify.Sdk.Cms.Client;
using Voicify.Sdk.Core.Models.Model;
using Voicify.Sdk.Webhooks.Services;

namespace QuickBase.VoicifySync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuickBaseController : ControllerBase
    {
        private readonly IConfiguration _config;

        public QuickBaseController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem([FromHeader] string authorization, [FromBody]QuickBaseQuestionAnswerItem item)
        {
            try
            {
                var tokenModel = TokenConvert.DeserializeEncryptedToken<WebhookTokenModel>(authorization, _config.GetValue<string>("EncodingKey") ?? "whoops");

                var authApi = new AuthenticationApi("https://cms.voicify.com");

                // sign in with api user creds to get token
                var tokenResponse = await authApi.AuthenticateAsync(tokenModel.VoicifyOrganizationId, tokenModel.VoicifyOrganizationSecret, "password", tokenModel.VoicifyApiUserName, tokenModel.VoicifyApiUserSecret);
                var voicifyConfig = new Configuration
                {
                    BasePath = "https://cms.voicify.com",
                    DefaultHeader = new Dictionary<string, string>
                    {
                        { "Authorization", $"Bearer {tokenResponse.AccessToken}" }
                    }
                };
                var featureApi = new FeatureApi(voicifyConfig);
                var questionAnswerApi = new QuestionAnswerApi(voicifyConfig);
                var mediaItemApi = new MediaItemApi(voicifyConfig);
                MediaItemModel mediaItem = null;
                if(!string.IsNullOrEmpty(item?.ForegroundImageUrl) && Uri.TryCreate(item.ForegroundImageUrl, UriKind.Absolute, out var imageUri))
                {
                    // TODO: once voicify allows external urls through this api, send it up
                    //mediaItem = await mediaItemApi.CreateMediaItem_0Async(tokenModel.VoicifyApplicationId, new NewMediaItemRequest(item.ForegroundImageUrl, item.ForegroundImageUrl, item.ForegroundImageUrl, ""));
                }

                // get root feature of app
                var features = await featureApi.GetFeaturesForApplicationAsync(tokenModel.VoicifyApplicationId);
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
                        new AnswerModel(
                            content: item.Answer, 
                            largeImage: mediaItem, 
                            followUp: string.IsNullOrEmpty(item.FollowUpPrompt) 
                                ? null 
                                : new FollowUpModel(applicationId: tokenModel.VoicifyApplicationId, content: item.FollowUpPrompt))
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
