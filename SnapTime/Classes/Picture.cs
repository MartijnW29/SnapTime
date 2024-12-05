﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTime.Classes
{
    internal class Picture
    {
        public int Id { get; set; }

        public byte[]? Image { get; set; }

        public int ownerId { get; set; }
        public User? owner { get; set; }

        public List<Comment>? comments { get; set; }
    }
}
