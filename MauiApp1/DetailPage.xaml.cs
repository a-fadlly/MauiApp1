using System.Diagnostics;

namespace MauiApp1;

public partial class DetailPage : ContentPage
{
    public Contact LoadedContact { get; set; }

    private DatabaseService databaseService;

    public DetailPage(Contact contact)
    {
        InitializeComponent();

        databaseService = new DatabaseService();
    }
}