using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Voicify.Sdk.Webhooks.Data;

namespace QuickBase.VoicifySync.Providers
{
    public abstract class BaseQuickBaseProvider : HttpDataProvider
    {
        protected string _realm;
        protected string _token;
        public BaseQuickBaseProvider(HttpClient client, string realm, string token) : base(client)
        {
            _realm = realm;
            _token = token;
        }

        protected override Task SetTokenAsync()
        {
            if (_client.DefaultRequestHeaders.Contains("Authorization"))
                _client.DefaultRequestHeaders.Remove("Authorization");

            _client.DefaultRequestHeaders.Add("Authorization", $"QB-USER-TOKEN b5q8sy_pcu2_c8cwfi6bt5smy96rxbfkbs23bte");

            if (_client.DefaultRequestHeaders.Contains("QB-Realm-Hostname"))
                _client.DefaultRequestHeaders.Remove("QB-Realm-Hostname");

            _client.DefaultRequestHeaders.Add("QB-Realm-Hostname", "hackathon20-adunn");

            return Task.CompletedTask;
        }

    }
}
