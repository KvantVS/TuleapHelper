using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuleapHelper
{
    public class BaseBugTracker
    {
        public List<string> NeededFields { get; set; }
        public int TrackerId { get; set; }
        public TuleapClasses.Project ParentProject { get; set; }

        public void StartWorkOnBug(int artefactId)
        {
            
        }
    }
}
