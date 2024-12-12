namespace SnapTime;
using SnapTime.SQLite; // nodig voor telefoon database
using SnapTime.Services;
using SnapTime.Classes;

public partial class SettingsPage : ContentPage
{

    SQLiteDatabase localdatabase;

    public SettingsPage()
	{
		InitializeComponent();
        localdatabase = new SQLiteDatabase();
    }


	private async void OnLogoutButtonClicked(object sender, EventArgs e)
	{
        await localdatabase.DeleteUserAsync(App.CurrentUser.Id);

        Application.Current.MainPage = new LoginPage();
    }
}