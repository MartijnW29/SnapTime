namespace SnapTime;

public partial class MainBar : TabbedPage
{
    public MainBar(int page = 1)
    {
        InitializeComponent();

        CurrentPage = Children[page]; // Index 1 verwijst naar het tweede tabblad ("Home")
    }
}
