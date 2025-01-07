using SnapTime.Classes;
using SnapTime.SQLite;
using System;
using SnapTime.Services;

namespace SnapTime
{
    public partial class App : Application

    {
        public static User? CurrentUser { get; set; }
        SQLiteDatabase localDatabase;
        public static LoggedInUser? LoggedInUser { get; set; }

        public App()
        {
            InitializeComponent();

            localDatabase = new SQLiteDatabase();
            SetCurrentUserAsync();

            if (CurrentUser == null) // dit werkt nog niet is voor later
            {
                MainPage = new LoginPage();
            }
            else
            {
                MainPage = new MainBar();
            }
        }

        private async void SetCurrentUserAsync()
        {
            LoggedInUser = await localDatabase.GetPreviouslyLoggedInUserAsync();

            if (LoggedInUser != null)
            {
                var firebaseHelper = new FirebaseHelper();
                CurrentUser = await firebaseHelper.GetUserById(LoggedInUser.Id);
                Application.Current.MainPage = new MainBar();
            }
        }
    }
}
