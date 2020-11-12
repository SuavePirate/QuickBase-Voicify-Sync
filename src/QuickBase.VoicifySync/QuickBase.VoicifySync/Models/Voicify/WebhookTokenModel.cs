using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Models.Voicify
{
    public class WebhookTokenModel
    {
        public string VoicifyApiUserName { get; set; }
        public string VoicifyApiUserSecret { get; set; }
        public string VoicifyApplicationId { get; set; }
        public string QuickBaseToken { get; set; }
        public string QuickBaseRealm { get; set; }
        public string QuickBaseAppId { get; set; }
        public string QuickBaseRequestTableId { get; set; }
        public string QuickBaseFaqTableId { get; set; }
        public Dictionary<string,string> QuickBaseRequestTableMatrix { get; set; }
        public Dictionary<string,string> QuickBaseFaqTableMatrix { get; set; }
    }
}
