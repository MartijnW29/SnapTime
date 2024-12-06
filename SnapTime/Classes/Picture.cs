using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTime.Classes
{
    public class Picture
    {
        public string? Id { get; set; }

        public byte[]? Image { get; set; }

        public string? ownerId { get; set; }
        public User? owner { get; set; }

        public List<Comment>? comments { get; set; }
    }
}
