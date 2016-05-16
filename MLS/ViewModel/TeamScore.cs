using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLS.ViewModel
{
    public class TeamScore
    {
        public string Code { get; set; }
        public string Team { get; set; }
        public string A { get; set; }
        public TimeSpan AStart { get; set; }
        public TimeSpan AEnd { get; set; }
        public TimeSpan AScore { get; set; }


        public string B { get; set; }
        public TimeSpan BStart { get; set; }
        public TimeSpan BEnd { get; set; }
        public TimeSpan BScore { get; set; }


        public string C { get; set; }
        public TimeSpan CStart { get; set; }
        public TimeSpan CEnd { get; set; }
        public TimeSpan CScore { get; set; }

        public string D { get; set; }
        public TimeSpan DStart { get; set; }
        public TimeSpan DEnd { get; set; }
        public TimeSpan DScore { get; set; }

        public TimeSpan GroupScore { get; set; }
    }
}
