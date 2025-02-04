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
        private List<User> _friends;
        private List<FriendRequest> _incomingRequests;
        private List<User> _searchedUsers;

        public FriendsPage()
        {
            InitializeComponent();
            _firebaseHelper = new FirebaseHelper();
            _friends = new List<User>();
            _incomingRequests = new List<FriendRequest>();
            _searchedUsers = new List<User>();
            BindingContext = this;
            LoadFriendsAndRequests();
        }

        private async void LoadFriendsAndRequests()
        {
            var currentUser = await _firebaseHelper.GetUserById("currentUserId");

            try
            {
                _incomingRequests = await _firebaseHelper.GetFriendRequestsForUser(App.CurrentUser.Id);
                if (_incomingRequests == null || !_incomingRequests.Any())
                {
                    await DisplayAlert("Info", "Geen inkomende vriendverzoeken gevonden.", "OK");
                }
                else
                {
                    foreach (var request in _incomingRequests)
                    {
                        Console.WriteLine($"Verzoek van: {request.Sender.Username} aan {request.Receiver.Username}");
                    }
                }
                IncomingRequestsListView.ItemsSource = _incomingRequests;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Fout bij het ophalen van vriendverzoeken: {ex.Message}", "OK");
            }
        }



        // Methode voor het accepteren van een inkomend vriendverzoek
        private async void AcceptFriendRequest(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var requestId = button.CommandParameter?.ToString(); // Haal het friend request ID op

            if (string.IsNullOrEmpty(requestId))
            {
                await DisplayAlert("Fout", "Verzoek ID is ongeldig.", "OK");
                return;
            }

            // Gebruik try-catch voor foutafhandeling
            try
            {
                // Haal het vriendverzoek op uit de lijst
                var request = _incomingRequests.FirstOrDefault(fr => fr.Id == requestId);
                if (request == null)
                {
                    await DisplayAlert("Fout", "Vriendverzoek niet gevonden.", "OK");
                    return;
                }

                // Markeer het verzoek als geaccepteerd
                request.Accepted = true;
                await _firebaseHelper.UpdateFriendRequest(request.Id, request);

                // Controleer of de gebruikers bestaan voordat ze worden toegevoegd
                var senderUser = request.Sender;
                var receiverUser = request.Receiver;

                if (senderUser == null || receiverUser == null)
                {
                    await DisplayAlert("Fout", "Gebruiker(s) niet gevonden.", "OK");
                    return;
                }

                // Voeg elkaar toe aan de lijst van vrienden
                senderUser.friends = senderUser.friends ?? new List<User>();
                receiverUser.friends = receiverUser.friends ?? new List<User>();
                senderUser.friends.Add(receiverUser);
                receiverUser.friends.Add(senderUser);

                // Update de gebruikers in Firebase
                await _firebaseHelper.UpdateSpecificUser(senderUser.Id, senderUser);
                await _firebaseHelper.UpdateSpecificUser(receiverUser.Id, receiverUser);

                // Verwijder het vriendverzoek (optioneel)
                await _firebaseHelper.DeleteItem("friendRequests", request.Id);

                // Herlaad de vriendenlijst en inkomende verzoeken
                LoadFriendsAndRequests();
            }
            catch (Exception ex)
            {
                // Toon een foutmelding bij een uitzondering
                await DisplayAlert("Fout", $"Er is een fout opgetreden: {ex.Message}", "OK");
            }
        }


        // Zoekfunctie voor vrienden
        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            var searchQuery = SearchBar.Text?.ToLower();

            if (string.IsNullOrEmpty(searchQuery))
            {
                // Haal alle gebruikers op
                var allUsers = await _firebaseHelper.GetItems<User>("users");
                _searchedUsers = allUsers.Where(u => u.Username?.ToLower().Contains(searchQuery) ?? false).ToList();
            }
            else
            {
                // Zoek gebruikers die de query bevatten
                var allUsers = await _firebaseHelper.GetItems<User>("users");
                _searchedUsers = allUsers.Where(u => u.Username?.ToLower().Contains(searchQuery) ?? false).ToList();
            }

            // Update de CollectionView met de zoekresultaten
            UsersListView.ItemsSource = _searchedUsers;
        }

        private async void AddFriendClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var userId = button.CommandParameter?.ToString();

            if (userId == null)
            {
                await DisplayAlert("Fout", "Gebruiker ID is ongeldig.", "OK");
                return;
            }

            // Haal de huidige gebruiker op (zorg ervoor dat je de juiste ID gebruikt)
            var currentUser = await _firebaseHelper.GetUserById(App.CurrentUser.Id);

            if (currentUser == null)
            {
                await DisplayAlert("Fout", "Kan huidige gebruiker niet ophalen.", "OK");
                return;
            }

            // Haal de gebruiker van de knop op
            var userToAdd = _searchedUsers.FirstOrDefault(u => u.Id == userId);
            if (userToAdd == null)
            {
                await DisplayAlert("Fout", "Gebruiker niet gevonden.", "OK");
                return;
            }

            // Controleer of er al een verzoek bestaat
            var existingRequest = await _firebaseHelper.GetFriendRequestsForUser(currentUser.Id);
            var existingRequestToUser = existingRequest.FirstOrDefault(fr => fr.ReceiverId == userToAdd.Id && !fr.Accepted);
            var existingRequestFromUser = existingRequest.FirstOrDefault(fr => fr.SenderId == userToAdd.Id && !fr.Accepted);

            if (existingRequestToUser != null || existingRequestFromUser != null)
            {
                // Er bestaat al een openstaand verzoek, dus voorkom het versturen
                await DisplayAlert("Fout", "Je hebt al een openstaand verzoek met deze gebruiker.", "OK");
                return;
            }

            // Maak een nieuw vriendverzoek
            var friendRequest = new FriendRequest
            {
                Id = Guid.NewGuid().ToString(),
                Accepted = false,
                SenderId = currentUser?.Id,
                Sender = currentUser,
                ReceiverId = userToAdd.Id,
                Receiver = userToAdd
            };

            // Voeg het vriendverzoek toe aan Firebase
            await _firebaseHelper.AddFriendRequest(friendRequest);

            // Optioneel: stuur de gebruiker een melding dat het verzoek is verstuurd
            await DisplayAlert("Verzoek verstuurd", $"Vriendverzoek naar {userToAdd.Username} is verstuurd.", "OK");

            // Herlaad de zoekresultaten (optioneel)
            OnSearchButtonPressed(sender, e);
        }

    }
}
