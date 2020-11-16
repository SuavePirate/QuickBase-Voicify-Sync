using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using QuickBase.VoicifySync.Models.Voicify;
using QuickBase.VoicifySync.Providers;
using Voicify.Sdk.Cms.Api;
using Voicify.Sdk.Cms.Client;
using Voicify.Sdk.Core.Models.Integrations.Setup;
using Voicify.Sdk.Core.Models.Model;
using Voicify.Sdk.Webhooks.Services;
using IntegrationSetupSection = Voicify.Sdk.Core.Models.Integrations.Setup.IntegrationSetupSection;
using IntegrationSetupProperty = Voicify.Sdk.Core.Models.Integrations.Setup.IntegrationSetupProperty;
using ServiceResult;
using System.Net.Http;
using QuickBase.VoicifySync.Models.QuickBase;
using Microsoft.Extensions.Configuration;

namespace QuickBase.VoicifySync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoicifySetupController : ControllerBase
    {
        private readonly IConfiguration _config;

        public VoicifySetupController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("Setup")]
        public async Task<IActionResult> Setup([FromBody] IntegrationSetupRequest request)
        {
            var appApi = new ApplicationApi(new Configuration
            {
                BasePath = "https://cms.voicify.com",
                DefaultHeader = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {request.AccessToken}" }
                }
            });

            var apps = await appApi.GetApplicationsForOrganizationAsync(request.OrganizationId);

            // app selector
            // text field for realm
            // text field for token
            var response = new IntegrationSetupResponse()
            {
                SetupSections = new List<IntegrationSetupSection>
                {
                    new IntegrationSetupSection
                    {
                        InstructionsMarkdown = @"Link your Quick Base account to a Voicify app by selecting the app, 
                            and providing a Quick Base user token and realm. Once linked, this integration will create a Quick Base app for you and supply all the tables and integrations you need to get started.",
                        Properties = new List<IntegrationSetupProperty>
                        {
                            new IntegrationSetupProperty
                            {
                                Id = "voicify-app",
                                Label = "Voicify Application",
                                Required = true,
                                Field = new SelectField
                                {
                                    Options = apps.Select(a => new SelectFieldOption
                                    {
                                        Label = a.Name,
                                        Value = a.Id
                                    }).ToList()
                                }
                            },
                            new IntegrationSetupProperty
                            {
                                Id = "quick-base-token",
                                Label = "Quick Base User Token",
                                Required = true,
                                Field = new ShortTextField
                                {
                                    Placeholder = "User token. You can get this from your Quick Base Account"
                                }
                            },
                            new IntegrationSetupProperty
                            {
                                Id = "quick-base-realm",
                                Label = "Quick Base Realm",
                                Required = true,
                                Field = new ShortTextField
                                {
                                    Placeholder = "realm"
                                }
                            }
                        }
                    }
                }
            };

            return Ok(response);

        }

        [HttpPost("Config")]
        public async Task<IActionResult> Config([FromBody] IntegrationSetupRequest request)
        {
            request.AdditionalProperties.TryGetValue("voicify-app", out var voicifyAppId);
            request.AdditionalProperties.TryGetValue("quick-base-token", out var quickbaseToken);
            request.AdditionalProperties.TryGetValue("quick-base-realm", out var quickbaseRealm);

            // validate inputs
            if (string.IsNullOrEmpty(voicifyAppId))
                return BadRequest(new[] { "You must select a Voicify app." });
            if (string.IsNullOrEmpty(quickbaseToken))
                return BadRequest(new[] { "You must provide a user token for Quick Base." });
            if (string.IsNullOrEmpty(quickbaseRealm))
                return BadRequest(new[] { "You must provide a Quick Base realm." });
            var voicifyConfig = new Configuration
            {
                BasePath = "https://cms.voicify.com",
                DefaultHeader = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {request.AccessToken}" }
                }
            };

            using var client = new HttpClient();
            var appProvider = new QuickBaseAppProvider(client, quickbaseRealm, quickbaseToken);
            var tableProvider = new QuickBaseTableProvider(client, quickbaseRealm, quickbaseToken);
            var fieldProvider = new QuickBaseFieldProvider(client, quickbaseRealm, quickbaseToken);
            var appApi = new ApplicationApi(voicifyConfig);
            var userApi = new UserApi(voicifyConfig);
            var webhookApi = new WebhookApi(voicifyConfig);
            var orgApi = new OrganizationApi(voicifyConfig);


            // get voicify org
            var orgs = await orgApi.GetForUserAsync();
            var org = orgs.FirstOrDefault(o => o.Id == request.OrganizationId);

            // get voicify app
            var app = await appApi.FindApplicationAsync(voicifyAppId);

            // create quick base app
            var appResult = await appProvider.CreateApp(new NewQuickBaseAppRequest
            {
                AssignToken = true,
                Description = app.Description,
                Name = app.Name
            });
            if (appResult.ResultType != ResultType.Ok)
                return BadRequest(appResult.Errors);

            // create tables
            var faqTableResult = await tableProvider.CreateTable(appResult.Data.Id, new NewTableRequest
            {
                Name = "Questions and Answers",
                Description = "Free form question and answer options",
                PluralRecordName = "Questions and Answers",
                SingleRecordName = "Question and Answer"
            });
            if (faqTableResult.ResultType != ResultType.Ok)
                return BadRequest(faqTableResult.Errors);
            var requestTableResult = await tableProvider.CreateTable(appResult.Data.Id, new NewTableRequest
            {
                Name = "Conversational Request Data",
                Description = "Data from the requests made by users to the voice/bot application",
                PluralRecordName = "Requests",
                SingleRecordName = "Request"
            });
            if (requestTableResult.ResultType != ResultType.Ok)
                return BadRequest(requestTableResult.Errors);

            // create fields for tables
            var results = await Task.WhenAll(new List<Task<Result<(QuickBaseField Field, string TableId)>>>
            {
                fieldProvider.CreateField(faqTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "question"
                }),
                fieldProvider.CreateField(faqTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "answer"
                }),
                fieldProvider.CreateField(faqTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "foregroundImageUrl"
                }),
                fieldProvider.CreateField(faqTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "followUpPrompt"
                }),
                fieldProvider.CreateField(faqTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "nextItemRecordIds"
                }),


                fieldProvider.CreateField(requestTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "applicationId"
                }),
                fieldProvider.CreateField(requestTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "requestDate"
                }),
                fieldProvider.CreateField(requestTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "platform"
                }),
                fieldProvider.CreateField(requestTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "requestId"
                }),
                fieldProvider.CreateField(requestTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "userId"
                }),
                fieldProvider.CreateField(requestTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "sessionId"
                }),
                fieldProvider.CreateField(requestTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "slots"
                }),
                fieldProvider.CreateField(requestTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "contentItemId"
                }),
                fieldProvider.CreateField(requestTableResult.Data.Id, new NewFieldRequest
                {
                    Label = "featureTypeId"
                }),
            });


            // create API user in voicify
            var apiUser = await userApi.CreateApiUserAsync(new NewApiUserRequest(request.OrganizationId));

            // create token with api user credentials
            var token = TokenConvert.SerializeEncryptedToken(new WebhookTokenModel
            {
                VoicifyOrganizationId = org.Id,
                VoicifyOrganizationSecret = org.Secret,
                VoicifyApiUserName = apiUser.Username,
                VoicifyApiUserSecret = apiUser.Password,
                QuickBaseToken = quickbaseToken,
                QuickBaseRealm = quickbaseRealm,
                VoicifyApplicationId = voicifyAppId,
                QuickBaseAppId = appResult.Data.Id,
                QuickBaseFaqTableId = faqTableResult.Data.Id,
                QuickBaseRequestTableId = requestTableResult.Data.Id,
                QuickBaseRequestTableMatrix = results.Where(r => r.Data.TableId == requestTableResult.Data.Id)
                .ToDictionary(
                    d => d.Data.Field.Label,
                    d => d.Data.Field.Id.ToString()),
                QuickBaseFaqTableMatrix = results.Where(r => r.Data.TableId == faqTableResult.Data.Id)
                .ToDictionary(
                    d => d.Data.Field.Label,
                    d => d.Data.Field.Id.ToString())
            }, _config.GetValue<string>("EncodingKey") ?? "whoops");

            // create webhook in voicify
            var webhook = await webhookApi.CreateWebhookAsync(request.OrganizationId,
                new NewWebhookRequest(
                    title: $"{app?.Name ?? "App"} - Quick Base Request Event",
                    description: "Creates event records in Quick Base when a request is received through Voicify. Use this token for Quick Base pipelines as well",
                    url: "https://quick-base-voicify-sync.azurewebsites.net/api/voicify/contentHit",
                    webhookTypeId: "53b40ef2-769c-46e6-bc99-c709e7600c03", // Content Hit Event webhook type
                    accessToken: token
                ));

            // add webhook to app
            var appWebhook = await appApi.AddWebhookAsync(voicifyAppId, webhook.Id, new WebhookParametersRequest(values: new Dictionary<string, string>(), userDefinedParameters: new Dictionary<string, string>()));


            return Ok();
        }
    }
}
