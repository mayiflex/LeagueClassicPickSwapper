using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueClassicPickSwapper.DTO_s {
    public class SwapRequest {
        public long id { get; set; }
        public bool initiatedByLocalPlayer { get; set; } = true;
        public int otherSummonerIndex { get; set; }
        public int requestorIndex { get; set; }
        public int responderIndex { get; set; }
        public string state { get; set; } = "SENT";
        public string type { get; set; } = "";
    }
}
