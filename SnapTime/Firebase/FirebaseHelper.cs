﻿using Firebase.Database;
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

            await _firebaseClient
                .Child("users")        // De node waar de gebruikers staan
                .Child(userId)         // Het unieke ID van de gebruiker
                .PutAsync(updatedUser); // Vervang de gebruiker met de nieuwe gegevens
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
    }
}
