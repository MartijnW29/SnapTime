namespace SnapTime;

public partial class MainBar : TabbedPage
{
    public MainBar()
    {
        InitializeComponent();

        CurrentPage = Children[1]; // Index 1 verwijst naar het tweede tabblad ("Home")
    }
}
