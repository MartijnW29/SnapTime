using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTime.Classes
{
    public class Comment
    {
        public string? Id { get; set; }

        public string? CommenterUsername { get; set; }

        public int CommentedPictureId { get; set; }

        public string? Body { get; set; }
        
        
    }
}
