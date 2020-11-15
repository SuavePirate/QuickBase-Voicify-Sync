using Newtonsoft.Json;
using QuickBase.VoicifySync.Models.QuickBase;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Voicify.Sdk.Webhooks.Data;

namespace QuickBase.VoicifySync.Providers
{
    public class QuickBaseAppProvider : BaseQuickBaseProvider
    {
        public QuickBaseAppProvider(HttpClient client, string realm, string token) : base(client, realm, token)
        {
        }

        public async Task<Result<QuickBaseApp>> CreateApp(NewQuickBaseAppRequest model)
        {
            return await PostJsonAsync<QuickBaseApp>("https://api.quickbase.com/v1/apps", JsonConvert.SerializeObject(model));
        }
    }
}
