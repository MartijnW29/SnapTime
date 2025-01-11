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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Laad de thema's wanneer de pagina verschijnt
        await LoadThemes();
    }

    private async void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        await localdatabase.DeleteUserAsync(App.CurrentUser.Id);
        Application.Current.MainPage = new LoginPage();
    }

    private async Task LoadThemes()
    {
        var availableThemes = await _firebaseHelper.GetThemes();
        ThemesStackLayout.Children.Clear();

        foreach (var theme in availableThemes)
        {
            var switchControl = new Switch
            {
                // Zet de juiste status van de switch op basis van de ChosenThemes van de gebruiker
                IsToggled = App.CurrentUser.ChosenThemes?.Any(t => t.Id == theme.Id) ?? false
            };

            // Event handler voor wanneer de switch wordt veranderd
            switchControl.Toggled += async (sender, e) =>
            {
                if (e.Value) // Als de switch aan gaat
                {
                    if (!App.CurrentUser.ChosenThemes.Any(t => t.Id == theme.Id))
                    {
                        App.CurrentUser.ChosenThemes.Add(theme);
                    }
                }
                else // Als de switch uit gaat
                {
                    App.CurrentUser.ChosenThemes.RemoveAll(t => t.Id == theme.Id);
                }

                // Zorg ervoor dat de hele lijst wordt opgeslagen
                await _firebaseHelper.UpdateSpecificUser(App.CurrentUser.Id, App.CurrentUser);
            };


            var themeLabel = new Label
            {
                Text = theme.Name,
                VerticalOptions = LayoutOptions.Center
            };

            var themeLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { themeLabel, switchControl }
            };

            ThemesStackLayout.Children.Add(themeLayout);
        }
    }


    private async void OnCreateThemeButtonClicked(object sender, EventArgs e)
    {
        string themeName = await DisplayPromptAsync("Nieuw Thema", "Voer een naam in voor het nieuwe thema:");

        if (!string.IsNullOrWhiteSpace(themeName))
        {
            // Haal de lijst van thema's op
            var existingThemes = await _firebaseHelper.GetThemes();

            // Controleer of het thema al bestaat
            if (existingThemes.Any(t => t.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase)))
            {
                await DisplayAlert("Fout", "Er bestaat al een thema met deze naam.", "OK");
            }
            else
            {
                // Maak het nieuwe thema aan
                var newTheme = new Theme { Name = themeName };
                await _firebaseHelper.AddItem(newTheme, "themes");

                // Voeg het nieuwe thema toe aan de huidige gebruiker
                App.CurrentUser.ChosenThemes.Add(newTheme);
                await _firebaseHelper.UpdateSpecificUser(App.CurrentUser.Id, App.CurrentUser);

                // Voeg de knop voor het nieuwe thema toe
                AddThemeToggleButton(newTheme);
            }
        }
    }

    private void AddThemeToggleButton(Theme theme)
    {
        var themeToggleButton = new Switch
        {
            IsToggled = App.CurrentUser.ChosenThemes.Contains(theme),
            HorizontalOptions = LayoutOptions.Fill
        };

        themeToggleButton.Toggled += async (sender, e) =>
        {
            if (themeToggleButton.IsToggled)
            {
                App.CurrentUser.ChosenThemes.Add(theme);
            }
            else
            {
                App.CurrentUser.ChosenThemes.Remove(theme);
            }
            await _firebaseHelper.UpdateSpecificUser(App.CurrentUser.Id, App.CurrentUser);
        };

        ThemesStackLayout.Children.Add(new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children = { new Label { Text = theme.Name }, themeToggleButton }
        });
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
