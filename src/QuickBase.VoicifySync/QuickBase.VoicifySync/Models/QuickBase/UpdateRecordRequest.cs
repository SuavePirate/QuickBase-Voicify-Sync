using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Models.QuickBase
{
    public class UpdateRecordRequest
    {
        [Newtonsoft.Json.JsonProperty("to")]
        public string To { get; set; } // table id
        [Newtonsoft.Json.JsonProperty("data")]
        public List<Dictionary<string, RecordValue>> Data { get; set; }
    }

    public class RecordValue
    {
        [Newtonsoft.Json.JsonProperty("value")]
        public string Value { get; set; }
        public RecordValue()
        {
        }
        public RecordValue(string value)
        {
            Value = value;
        }
    }
}
