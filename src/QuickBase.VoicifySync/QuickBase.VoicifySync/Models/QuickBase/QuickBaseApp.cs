using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBase.VoicifySync.Models.QuickBase
{
    public class QuickBaseApp
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public string DateFormat { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string TimeZone { get; set; }
        public DateTime Updated { get; set; }
        public bool HasEveryoneOnTheInternet { get; set; }
    }
}
