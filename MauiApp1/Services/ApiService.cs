using Newtonsoft.Json;
using System.Diagnostics;

namespace MauiApp1.Services
{
    public class ApiService
    {
        private const string ApiUrl = "http://192.168.18.51/api/api.php";

        public async Task<List<Contact>> GetContacts()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(ApiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<Contact> items = JsonConvert.DeserializeObject<List<Contact>>(json);
                return items;
            }
            else
            {
                return null;
            }
        }

        public async Task DownloadImage(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(url);

                    string fileName = Path.GetFileName(url);
                    string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    string filePath = Path.Combine(downloadsPath, fileName);

                    File.WriteAllBytes(filePath, imageBytes);

                    Debug.WriteLine(filePath);
                    // Image saved successfully
                }
                catch (Exception ex)
                {
                    // Error occurred while downloading or saving the image
                }
            }
        }
    }
}
