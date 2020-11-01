using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Models.QuickBase
{
    public class QuickBaseFieldProperties
    {
        public int MaxLength { get; set; }
        public bool AppendOnly { get; set; }
        public bool SortAsGiven { get; set; }
        public bool PrimaryKey { get; set; }
        public bool ForeignKey { get; set; }
    }
}
