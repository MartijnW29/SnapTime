using SnapTime.Services;

namespace SnapTime
{
    public partial class LoginPage : ContentPage
    {
        private readonly FirebaseHelper _firebaseHelper;

        public LoginPage()
        {
            InitializeComponent();
            _firebaseHelper = new FirebaseHelper();  // Firebase helper voor database-interactie
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            bool isEmailEmpty = string.IsNullOrEmpty(EmailEntry.Text);
            bool isUsernameEmpty = string.IsNullOrEmpty(UsernameEntry.Text);
            bool isPasswordEmpty = string.IsNullOrEmpty(PasswordEntry.Text);

            // Controleer of de invoervelden leeg zijn
            if (isEmailEmpty && isUsernameEmpty)
            {
                EmailEntry.Placeholder = "U bent dit invulveld vergeten!\nVul je email of Username in!";
            }
            if (isUsernameEmpty && isEmailEmpty)
            {
                UsernameEntry.Placeholder = "U bent dit invulveld vergeten!\nVul je email of Username in!";
            }
            else if (isPasswordEmpty)
            {
                PasswordEntry.Placeholder = "U bent dit invulveld vergeten!";
            }
            else
            {
                // Verkrijg email, gebruikersnaam en wachtwoord uit de invoervelden
                string email = EmailEntry.Text;
                string username = UsernameEntry.Text;
                string password = PasswordEntry.Text;

                // Controleer of de gebruiker bestaat
                var userId = await _firebaseHelper.CheckUserExistence(email, username, password);

                if (userId != null)
                {
                    var user = await _firebaseHelper.GetUserById(userId);

                    if (user != null)
                    {
                        // Sla de ingelogde gebruiker op in de applicatie
                        App.CurrentUser = user;
                    }

                    // Gebruiker gevonden, log in en navigeer naar het hoofdscherm

                    //await DisplayAlert("Inloggen", "Inloggen is gelukt!", "OK");
                    Application.Current.MainPage = new MainBar(); // Navigeer naar het hoofdscherm
                }
                else
                {
                        // Gebruiker niet gevonden, toon een foutmelding
                        await DisplayAlert("Fout", "Ongeldige e-mail of wachtwoord.", "OK");
                }
            }
        }

        private async void OnCreateAccountButtonClicked(object sender, EventArgs e)
        {
            bool isEmailEmpty = string.IsNullOrEmpty(EmailEntry.Text);
            bool isUsernameEmpty = string.IsNullOrEmpty(UsernameEntry.Text);
            bool isPasswordEmpty = string.IsNullOrEmpty(PasswordEntry.Text);

            // Controleer of de invoervelden leeg zijn
            if (isEmailEmpty)
            {
                EmailEntry.Placeholder = "U bent dit invulveld vergeten!";
            }
            else if (isUsernameEmpty)
            {
                UsernameEntry.Placeholder = "U bent dit invulveld vergeten!";
            }
            else if (isPasswordEmpty)
            {
                PasswordEntry.Placeholder = "U bent dit invulveld vergeten!";
            }
            else
            {
                // Verkrijg email, gebruikersnaam en wachtwoord uit de invoervelden
                string email = EmailEntry.Text;
                string username = UsernameEntry.Text;
                string password = PasswordEntry.Text;

                // Maak een nieuw User-object zonder ID
                var newUser = new Classes.User
                {
                    Email = email,
                    Username = username,
                    Password = password
                };

                // Voeg de nieuwe gebruiker toe aan de database
                await _firebaseHelper.MakeAccount(newUser);

                // Bevestiging geven en terug navigeren naar de inlogpagina
                await DisplayAlert("Account aangemaakt", "Je account is succesvol aangemaakt!", "OK");

                // Optioneel: Navigeer terug naar de loginpagina of naar het hoofdscherm
                Application.Current.MainPage = new LoginPage();
            }
        }

    }
}
