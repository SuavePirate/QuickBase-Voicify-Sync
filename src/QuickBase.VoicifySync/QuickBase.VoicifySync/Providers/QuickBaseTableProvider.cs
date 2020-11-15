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
    public class QuickBaseTableProvider : BaseQuickBaseProvider
    {
        public QuickBaseTableProvider(HttpClient client, string realm, string token) : base(client, realm, token)
        {
        }

        public async Task<Result<QuickBaseTable>> CreateTable(string appId, NewTableRequest model)
        {
            return await PostJsonAsync<QuickBaseTable>($"https://api.quickbase.com/v1/tables?appId={appId}", JsonConvert.SerializeObject(model));
        }
    }
}
