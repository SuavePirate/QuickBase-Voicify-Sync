using Newtonsoft.Json;
using QuickBase.VoicifySync.Models.QuickBase;
using ServiceResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Providers
{
    public class QuickBaseRecordProvider : BaseQuickBaseProvider
    {
        public QuickBaseRecordProvider(HttpClient client, string realm, string token) : base(client, realm, token)
        {
        }

        public async Task<Result<object>> AddOrUpdateRecord(UpdateRecordRequest model)
        {
            return await PostJsonAsync<object>($"https://api.quickbase.com/v1/records", JsonConvert.SerializeObject(model));
        }
    }
}
