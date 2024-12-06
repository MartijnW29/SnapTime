using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTime.Classes
{
    public class FriendRequest
    {
        public string? Id { get; set; }

        public bool Accepted { get; set; }

        public string? SenderId { get; set; }
        public User? Sender { get; set; }

        public string? ReceiverId { get; set; }
        public User? Receiver { get; set; }
        
    }
}
