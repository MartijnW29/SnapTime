using SnapTime.Classes;
using SnapTime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace SnapTime
{
    public partial class FriendsPage : ContentPage
    {
        private readonly FirebaseHelper _firebaseHelper;
        private List<User> _users;
        private List<User> _searchedUsers;

        public FriendsPage()
        {
            InitializeComponent();
            _firebaseHelper = new FirebaseHelper();
            _users = new List<User>();
            _searchedUsers = new List<User>();

            // Laad de lijst van gebruikers (in dit geval andere gebruikers die mogelijk vrienden kunnen worden)
            LoadUsers();
        }

        private async void LoadUsers()
        {
            // Haal alle gebruikers op uit Firebase
            var users = await _firebaseHelper.GetItems<User>("users");
            _users = users;

            // Stuur de gebruikers naar de CollectionView
            UsersListView.ItemsSource = _searchedUsers;
        }

        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            var searchQuery = SearchBar.Text?.ToLower();

            if (string.IsNullOrEmpty(searchQuery))
            {
                _searchedUsers = _users;
            }
            else
            {
                _searchedUsers = _users
                    .Where(u => u.Username?.ToLower().Contains(searchQuery) ?? false)
                    .ToList();
            }

            // Verander de ItemsSource naar de gefilterde lijst
            UsersListView.ItemsSource = _searchedUsers;
        }

        // De method om een vriend toe te voegen
        private async void AddFriendClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var receiverId = button.CommandParameter?.ToString(); // Haal het userId van de knopparameter op

            if (receiverId == null) return;

            // Haal de huidige gebruiker op uit Firebase (je moet de juiste ID gebruiken)
            var currentUser = await _firebaseHelper.GetUserById("currentUserId"); // Zorg ervoor dat je de juiste ID gebruikt

            var receiver = _searchedUsers.FirstOrDefault(u => u.Id == receiverId);
            if (receiver == null) return;

            // Maak een nieuw vriendverzoek
            var friendRequest = new FriendRequest
            {
                Id = Guid.NewGuid().ToString(),
                Accepted = false,
                SenderId = currentUser?.Id,
                Sender = currentUser,
                ReceiverId = receiver.Id,
                Receiver = receiver
            };

            // Voeg het vriendverzoek toe aan Firebase
            await _firebaseHelper.AddItem(friendRequest, "friendRequests");

            // Optioneel: je kunt de gebruiker hier een melding sturen dat het verzoek is verstuurd
        }
    }
}
