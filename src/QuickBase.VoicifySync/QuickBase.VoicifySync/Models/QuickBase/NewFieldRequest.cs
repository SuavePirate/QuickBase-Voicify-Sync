using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Models.QuickBase
{
    public class NewFieldRequest
    {
        public string Label { get; set; }
        public string FieldType { get; set; } = "text";
        public bool NoWrap { get; set; }
        public bool Bold { get; set; }
        public bool AppearsByDefault { get; set; } = true;
        public bool FindEnabled { get; set; } = true;
        public bool AddToForms { get; set; } = true;
        public string FieldHelp { get; set; } = "";
        public NewFieldProperties Properties { get; set; } = new NewFieldProperties
        {
        };
        public List<QuickBaseFieldPermission> Permissions { get; set; } = new List<QuickBaseFieldPermission>();
    }
}
