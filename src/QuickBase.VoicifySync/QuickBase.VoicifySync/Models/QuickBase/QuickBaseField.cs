using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Models.QuickBase
{
    public class QuickBaseField
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string FieldType { get; set; }
        public string Mode { get; set; }
        public bool NoWrap { get; set; }
        public bool Bold { get; set; }
        public bool Required { get; set; }
        public bool AppearsByDefault { get; set; }
        public bool FindEnabled { get; set; }
        public bool Unique { get; set; }
        public bool DoesDataCopy { get; set; }
        public string FieldHelp { get; set; }
        public bool Audited { get; set; }
        public QuickBaseFieldProperties Properties { get; set; }
        public List<QuickBaseFieldPermission> Permissions { get; set; }
    }
}
