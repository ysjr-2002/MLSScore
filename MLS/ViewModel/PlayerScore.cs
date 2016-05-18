using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLS.ViewModel
{
    public class PlayerScore
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Team { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Mid { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan Score { get; set; }
    }
}
