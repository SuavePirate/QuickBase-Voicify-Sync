using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Models.QuickBase
{
    public class NewTableRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SingleRecordName { get; set; }
        public string PluralRecordName { get; set; }
    }
}
