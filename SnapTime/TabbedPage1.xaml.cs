namespace SnapTime;

public partial class TabbedPage1 : TabbedPage
{
    public TabbedPage1()
    {
        InitializeComponent();
            
        CurrentPage = Children[1]; // Index 1 verwijst naar het tweede tabblad ("Home")
    }
}
