using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voicify.Sdk.Core.Models.Webhooks.Events;

namespace QuickBase.VoicifySync.Models.Voicify
{
    public class ContentHitEventRequest
    {
        public WebhookContentHitEvent Data { get; set; }
    }
}
