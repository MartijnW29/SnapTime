namespace SnapTime;
using SnapTime.SQLite; // nodig voor telefoon database
using SnapTime.Services;
using SnapTime.Classes;

public partial class SettingsPage : ContentPage
{
    private readonly FirebaseHelper _firebaseHelper;
    SQLiteDatabase localdatabase;

    public SettingsPage()
    {
        InitializeComponent();
        localdatabase = new SQLiteDatabase();
        _firebaseHelper = new FirebaseHelper();

        if (App.CurrentUser != null)
        {
            // Voeg TimePicker voor 'Beschikbaar van' toe
            TimePicker availableFromPicker = new TimePicker
            {
                Time = App.CurrentUser.AvailableFrom.HasValue ? App.CurrentUser.AvailableFrom.Value.ToTimeSpan() : TimeSpan.Zero,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            availableFromPicker.PropertyChanged += async (sender, e) =>
            {
                if (e.PropertyName == "Time")
                {
                    var selectedTime = availableFromPicker.Time;
                    App.CurrentUser.AvailableFrom = new TimeOnly(selectedTime.Hours, selectedTime.Minutes);
                    await _firebaseHelper.UpdateSpecificUser(App.CurrentUser.Id, App.CurrentUser);
                }
            };

            // Voeg de TimePicker toe aan de container
            AvailableFromTimePickerContainer.Content = availableFromPicker;

            // Voeg TimePicker voor 'Beschikbaar tot' toe
            TimePicker availableTillPicker = new TimePicker
            {
                Time = App.CurrentUser.AvailableFrom.HasValue ? App.CurrentUser.AvailableTill.Value.ToTimeSpan() : TimeSpan.Zero,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            availableTillPicker.PropertyChanged += async (sender, e) =>
            {
                if (e.PropertyName == "Time")
                {
                    var selectedTime = availableTillPicker.Time;
                    App.CurrentUser.AvailableTill = new TimeOnly(selectedTime.Hours, selectedTime.Minutes);
                    await _firebaseHelper.UpdateSpecificUser(App.CurrentUser.Id, App.CurrentUser);
                }
            };

            // Voeg de TimePicker toe aan de container
            AvailableTillTimePickerContainer.Content = availableTillPicker;
        }
    }


    private async void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        await localdatabase.DeleteUserAsync(App.CurrentUser.Id);

        Application.Current.MainPage = new LoginPage();
    }



    private async Task ReloadHomePage()
    {
        // Herlaad de huidige pagina
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var currentPage = Application.Current.MainPage?.Navigation?.NavigationStack.LastOrDefault();

            if (currentPage != null)
            {
                // Verwijder de huidige pagina en navigeer opnieuw
                await Application.Current.MainPage.Navigation.PopAsync(false); // Verwijder de oude pagina
                await Application.Current.MainPage.Navigation.PushAsync(new MainBar(2), false); // Voeg een nieuwe toe
            }
            else
            {
                // Als er geen navigatie-stack is, stel gewoon de MainPage in
                Application.Current.MainPage = new MainBar(0);
            }
        });
    }



}
