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
    public class QuickBaseFieldProvider : BaseQuickBaseProvider
    {
        public QuickBaseFieldProvider(HttpClient client, string realm, string token) : base(client, realm, token)
        {
        }

        public async Task<Result<QuickBaseField>> CreateField(string tableId, NewFieldRequest model)
        {
            return await PostJsonAsync<QuickBaseField>($"https://api.quickbase.com/v1/fields?tableId={tableId}", JsonConvert.SerializeObject(model));
        }
    }
}
