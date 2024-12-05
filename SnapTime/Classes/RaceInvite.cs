using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTime.Classes
{
    public class RaceInvite
    {
        public int Id { get; set; }

        public bool Accepted { get; set; }

        public int InvitedId { get; set; }
        public User? Invited { get; set; }

        public int raceId { get; set; }
    }
}
