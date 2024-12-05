using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTime.Classes
{
    public class Theme
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User>? Users { get; set; }

    }
}
