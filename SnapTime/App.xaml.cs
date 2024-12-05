using SnapTime.Classes;

namespace SnapTime
{
    public partial class App : Application
    {
        
        public static User? CurrentUser { get; set; }

        public App()
        {
            InitializeComponent();
            if (CurrentUser == null) // dit werkt nog niet is voor later
            {
                MainPage = new LoginPage();
            }
            else
            {
                MainPage = new MainBar();
            }
        }
    }
}
