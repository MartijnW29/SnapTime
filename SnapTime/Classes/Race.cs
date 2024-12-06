using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTime.Classes
{
    public class Race
    {
        public string? Id { get; set; }

        public string? Type { get; set; }

        public int Bet { get; set; }

        public bool RandomThemes { get; set; }
        


        public string? HostId { get; set; }
        public User? Host { get; set; }


        public List<User>? Players { get; set; }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        

    }
}
