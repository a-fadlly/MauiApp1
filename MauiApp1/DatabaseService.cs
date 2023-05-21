using SQLite;
using MauiApp1.Services;
using System.Diagnostics;

namespace MauiApp1
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection database;
        private ApiService apiService;

        public DatabaseService()
        {
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "mydatabase.db");
            database = new SQLiteAsyncConnection(databasePath);
            database.CreateTableAsync<Contact>().Wait();
        }

        public async Task SaveContacts(List<Contact> contacts)
        {
            await database.DeleteAllAsync<Contact>();

            apiService = new ApiService();

            foreach (var contact in contacts)
            {
                await SaveContact(contact);
                await apiService.DownloadImage(contact.Pic);

                Debug.WriteLine(contact.Pic);
            }
        }

        public async Task<List<Contact>> SearchContacts(string searchText)
        {
            return await database.Table<Contact>()
                .Where(c => c.Name.Contains(searchText) || c.Phone.Contains(searchText))
                .ToListAsync();
        }

        public async Task<List<Contact>> LoadContacts()
        {
            return await database.Table<Contact>().ToListAsync();
        }

        public async Task<Contact> LoadContact(int id)
        {
            return await database.Table<Contact>().FirstOrDefaultAsync(i => i.Id == id);
        }
        private async Task SaveContact(Contact contact)
        {
            await database.InsertAsync(contact);
        }
    }
}