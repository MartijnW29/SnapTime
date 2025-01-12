using Firebase.Database;
using Firebase.Database.Query;
using SnapTime.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnapTime.Services
{
    public class FirebaseHelper
    {
        private readonly FirebaseClient _firebaseClient;

        public FirebaseHelper()
        {
            // Vervang door jouw Firebase Database URL
            _firebaseClient = new FirebaseClient("https://snaptime-23f71-default-rtdb.europe-west1.firebasedatabase.app/");
        }

        // Voeg een nieuwe race toe
        public async Task AddRace(Race race)
        {
            await _firebaseClient
                .Child("races")
                .PostAsync(race);
        }

        // Haal alle races op
        public async Task<List<Race>> GetRaces()
        {
            var races = await _firebaseClient
                .Child("races")
                .OnceAsync<Race>();

            return races.Select(r => r.Object).ToList();
        }

        // Update een race
        public async Task UpdateRace(string raceId, Race race)
        {
            await _firebaseClient
                .Child("races")
                .Child(raceId)
                .PutAsync(race);
        }

        // Verwijder een race
        public async Task DeleteRace(string raceId)
        {
            await _firebaseClient
                .Child("races")
                .Child(raceId)
                .DeleteAsync();
        }

        public async Task<List<Theme>> GetThemes()
        {
            var themes = await _firebaseClient
                .Child("themes") // Zorg ervoor dat dit de juiste node is waar je thema's staan opgeslagen
                .OnceAsync<Theme>();

            return themes.Select(t => t.Object).ToList();
        }

        public async Task UpdateSpecificUser(string userId, User updatedUser)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID mag niet leeg zijn.");
            }

            if (updatedUser == null)
            {
                throw new ArgumentNullException(nameof(updatedUser), "Updated user mag niet null zijn.");
            }

            // Controleer of ChosenThemes niet null is en zorg ervoor dat het correct wordt opgeslagen
            updatedUser.ChosenThemes = updatedUser.ChosenThemes ?? new List<Theme>();

            await _firebaseClient
                .Child("users")
                .Child(userId)
                .PutAsync(updatedUser);
        }

        public async Task<User> GetUserById(string userId)
        {
            var users = await _firebaseClient
                .Child("users")
                .OnceAsync<User>();

            var user = users.FirstOrDefault(u => u.Object.Id == userId)?.Object;
            return user;
        }

        public async Task<string> CheckUserExistence(string email, string Username, string password)
        {
            var users = await _firebaseClient
                .Child("users")
                .OnceAsync<User>();

            foreach (var u in users)
            {
                if (u.Object.Email == email && u.Object.Password == password || u.Object.Username == Username && u.Object.Password == password)
                {
                    return u.Object.Id;
                }
            }
            return null;
        }

        // Voeg een nieuwe gebruiker toe aan de database
        public async Task MakeAccount(User user)
        {
            // Voeg de gebruiker toe aan de Firebase-database
            var result = await _firebaseClient
                .Child("users")
                .PostAsync(user);

            // Haal de unieke Firebase-sleutel op en sla die op als Id
            string userId = result.Key;
            user.Id = userId; // Wijs de gegenereerde sleutel toe aan het user-object

            // Update de gebruiker in Firebase met de nieuwe Id
            await _firebaseClient
                .Child("users")
                .Child(userId) // Gebruik de gegenereerde sleutel hier
                .PutAsync(user);
        }

        // Voeg data toe
        public async Task AddItem<T>(T item, string node)
        {
            await _firebaseClient
                .Child(node)
                .PostAsync(item);
        }

        // Haal data op
        public async Task<List<T>> GetItems<T>(string node)
        {
            var items = await _firebaseClient
                .Child(node)
                .OnceAsync<T>();

            return items.Select(x => x.Object).ToList();
        }

        // Update data
        public async Task UpdateItem<T>(string node, string key, T item)
        {
            await _firebaseClient
                .Child(node)
                .Child(key)
                .PutAsync(item);
        }

        // Verwijder data
        public async Task DeleteItem(string node, string key)
        {
            await _firebaseClient
                .Child(node)
                .Child(key)
                .DeleteAsync();
        }

        public async Task<FriendRequest> GetExistingFriendRequest(string senderId, string receiverId)
        {
            var friendRequests = await _firebaseClient
                .Child("friendRequests")
                .OnceAsync<FriendRequest>();

            // Zoek naar een vriendverzoek waar de zender en ontvanger overeenkomen en het verzoek niet geaccepteerd is
            return friendRequests
                .Select(fr => fr.Object)
                .FirstOrDefault(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId && !fr.Accepted);
        }



        // Voeg een vriendverzoek toe
        public async Task AddFriendRequest(FriendRequest request)
        {
            await _firebaseClient
                .Child("friendRequests")
                .PostAsync(request);
        }

        // Haal alle vriendverzoeken op voor een gebruiker
        public async Task<List<FriendRequest>> GetFriendRequestsForUser(string userId)
        {
            var friendRequests = await _firebaseClient
                .Child("friendRequests")
                .OnceAsync<FriendRequest>();

            return friendRequests
                .Select(fr => fr.Object)
                .Where(fr => fr.ReceiverId == userId && !fr.Accepted)
                .ToList();
        }

        // Update een vriendverzoek (bijvoorbeeld bij acceptatie)
        public async Task UpdateFriendRequest(string requestId, FriendRequest updatedRequest)
        {
            await _firebaseClient
                .Child("friendRequests")
                .Child(requestId)
                .PutAsync(updatedRequest);
        }

        // Haal een gebruiker op met behulp van de gebruikersnaam
        public async Task<User> GetUserByUsername(string username)
        {
            var users = await _firebaseClient
                .Child("users")
                .OnceAsync<User>();

            return users.Select(u => u.Object)
                        .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
