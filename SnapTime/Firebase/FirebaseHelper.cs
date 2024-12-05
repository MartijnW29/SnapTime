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

        public async Task<int> CheckUserExistence(string email, string password)
        {
            var users = await _firebaseClient
                .Child("users")
                .OnceAsync<User>();

                foreach (var u in users)
                {
                    if (u.Object.Email == email && u.Object.Password == password)
                    {
                        return u.Object.Id;
                    }
                }
                return -1;

        }

        // Voeg een nieuwe gebruiker toe aan de database
        public async Task MakeAccount(User user)
        {
            await _firebaseClient
                .Child("users")
                .PostAsync(user);
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
    }
}
