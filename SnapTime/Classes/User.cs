using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTime.Classes
{
    public class User
    {
        public string? Id { get; set; }

        public string? Email { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public int? Snaplets { get; set; } = 5; 

        public int? TotalPicturesTaken { get; set; } = 0;

        public TimeOnly? AvailableFrom { get; set; } = new TimeOnly(12, 0);

        public TimeOnly? AvailableTill { get; set; } = new TimeOnly(20, 0);

        public List<Theme>? ChosenThemes { get; set; } = new List<Theme>();

        public List<Picture>? Pictures { get; set; }

        public List<User>? friends { get; set; }

        public List<Race>? Races { get; set; }

        public List<RaceInvite>? RaceInvites { get; set; }

        public List<FriendRequest>? FriendRequests { get; set; }


    }
}
