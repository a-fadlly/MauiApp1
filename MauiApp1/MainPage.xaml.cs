using MauiApp1.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace MauiApp1
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private List<Contact> contacts;
        private DatabaseService databaseService;

        public MainPage()
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            _ = LoadContactsAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void OnDownloadClicked(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status == PermissionStatus.Granted)
            {
                Debug.WriteLine("PermissionStatus.Granted");
            }
            else
            {
                Debug.WriteLine("PermissionStatus.Denied");
            }

            ApiService apiService = new ApiService();
            contacts = await apiService.GetContacts();

            // Save contacts to the local database
            await databaseService.SaveContacts(contacts);

            // Update the collection view
            ContactsCollectionView.ItemsSource = contacts;

            OnPropertyChanged(nameof(contacts));
        }

        private async void OnSearchButtonPressed(object sender, EventArgs e)
        {
            string searchText = SearchBarContacts.Text;

            // Search the contacts in the local database
            List<Contact> searchedContacts = await databaseService.SearchContacts(searchText);

            // Update the collection view with the searched contacts
            ContactsCollectionView.ItemsSource = searchedContacts;

            OnPropertyChanged(nameof(contacts));
        }

        private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected item from the event arguments
            var selectedItems = e.CurrentSelection;
            var tappedItem = (selectedItems.FirstOrDefault() as Contact);

            if (tappedItem != null)
            {
                // Create a new instance of the detail page and pass the parameter
                var detailPage = new DetailPage(tappedItem);

                // Navigate to the detail page
                await Navigation.PushAsync(detailPage);

                // Deselect the tapped item
                ContactsCollectionView.SelectedItem = null;
            }
        }

        private async Task LoadContactsAsync()
        {
            // Load contacts from the local database
            contacts = await databaseService.LoadContacts();

            // Update the collection view
            ContactsCollectionView.ItemsSource = contacts;

            OnPropertyChanged(nameof(contacts));
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            Permissions.RequestAsync<Permissions.StorageWrite>().ContinueWith(HandleStorageWritePermission);
        }

        private void HandleStorageWritePermission(Task<PermissionStatus> task)
        {
            var status = task.Result;

            if (status != PermissionStatus.Granted)
            {
                // Handle denied permission
                Debug.WriteLine("PermissionStatus.Denied");
            }
        }
    }
}
