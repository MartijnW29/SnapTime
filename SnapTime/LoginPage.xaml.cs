namespace SnapTime;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private void OnLoginButtonClicked(object sender, EventArgs e)
	{
		bool isUsernameEmpty = string.IsNullOrEmpty(UsernameEntry.Text);
		bool isPasswordEmpty = string.IsNullOrEmpty(PasswordEntry.Text);

		if (isUsernameEmpty)
		{
			UsernameEntry.Placeholder = "U bent dit invulveld vergeten!";
		}
		else if (isPasswordEmpty)
		{
			PasswordEntry.Placeholder = "U bent dit invulveld vergeten!";
		}
		else
		{
			Navigation.PushAsync(new MainPage());
		}
	}
}