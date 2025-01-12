using SnapTime.Classes;
using SnapTime.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnapTime
{
    public partial class RacesPage : ContentPage
    {
        private readonly FirebaseHelper _firebaseHelper;
        private List<Race> _races;

        public RacesPage()
        {
            InitializeComponent();
            _firebaseHelper = new FirebaseHelper();
            LoadRaces();
        }

        private async void LoadRaces()
        {
            _races = await _firebaseHelper.GetRaces();
            RacesListView.ItemsSource = _races;
        }

        private async void CreateRaceClicked(object sender, EventArgs e)
        {
            // Combineer datum en tijd voor start en einde
            DateTime startDateTime = StartDatePicker.Date.Add(StartTimePicker.Time);
            DateTime endDateTime = EndDatePicker.Date.Add(EndTimePicker.Time);

            var newRace = new Race
            {
                Id = Guid.NewGuid().ToString(),
                Type = RaceTypeEntry.Text,
                Bet = int.TryParse(BetEntry.Text, out var bet) ? bet : 0,
                RandomThemes = RandomThemesSwitch.IsToggled,
                Start = startDateTime,
                End = endDateTime
            };

            await _firebaseHelper.AddRace(newRace);

            // Refresh the race list
            LoadRaces();

            // Clear input fields
            RaceTypeEntry.Text = string.Empty;
            BetEntry.Text = string.Empty;
            RandomThemesSwitch.IsToggled = false;
            StartDatePicker.Date = DateTime.Today;
            StartTimePicker.Time = TimeSpan.Zero;
            EndDatePicker.Date = DateTime.Today;
            EndTimePicker.Time = TimeSpan.Zero;
        }
    }
}
