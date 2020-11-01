using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Models.QuickBase
{
    public class NewQuickBaseAppRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool AssignToken { get; set; } = true;
    }
}
