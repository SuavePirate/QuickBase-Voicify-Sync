using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Models.QuickBase
{
    public class QuickBaseQuestionAnswerItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        [JsonProperty("foreground_image_url")]
        public string ForegroundImageUrl { get; set; }
        [JsonProperty("follow_up_prompt")]
        public string FollowUpPrompt { get; set; }
        [JsonProperty("next_item_record_ids")]
        public string NextItemRecordIds { get; set; }
    }
}
