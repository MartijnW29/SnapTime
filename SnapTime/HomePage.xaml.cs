using System.IO;

namespace SnapTime;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();

        // Zorg ervoor dat de gebruiker is ingelogd
        if (App.CurrentUser != null)
        {
          
            SnapletsLabel.Text = $"Snaplets: {App.CurrentUser.Snaplets}";

            TotalPicturesTakenLabel.Text = $"Total Pictures Taken: {App.CurrentUser.TotalPicturesTaken}";
        }
        
    }
}
