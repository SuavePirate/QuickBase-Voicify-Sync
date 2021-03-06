﻿using Newtonsoft.Json;
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

        public async Task<Result<(QuickBaseField Field, string TableId)>> CreateField(string tableId, NewFieldRequest model)
        {
            var result = await PostJsonAsync<QuickBaseField>($"https://api.quickbase.com/v1/fields?tableId={tableId}", JsonConvert.SerializeObject(model));
            if(result.ResultType == ResultType.Ok)
            {
                return new SuccessResult<(QuickBaseField Field, string TableId)>((result.Data, tableId));
            }

            return new InvalidResult<(QuickBaseField Field, string TableId)>(result.Errors.FirstOrDefault());
        }
    }
}
