namespace SnapTime
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }


        private async Task<bool> RequestPermissionsAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
            return status == PermissionStatus.Granted;
        }

        // Zorg ervoor dat de methode een EventHandler is met de juiste handtekening

        private async void TakePhotoAsync(object sender, EventArgs e)
        {
            if (await RequestPermissionsAsync())
            {
                try
                {
                    // Maak een foto met de camera
                    var photo = await MediaPicker.CapturePhotoAsync();

                    if (photo != null)
                    {
                        // Pad voor de foto in de app-specifieke opslag (schijf)
                        var photoPath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                        // Open de foto om te lezen
                        using (var stream = await photo.OpenReadAsync())
                        using (var fileStream = new FileStream(photoPath, FileMode.Create, FileAccess.Write))
                        {
                            await stream.CopyToAsync(fileStream);
                        }

                        // Foto succesvol opgeslagen op de schijf
                        Console.WriteLine($"Foto opgeslagen op: {photoPath}");
                        await UploadPhotoAsync(photoPath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Er is een fout opgetreden: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("De app heeft geen toestemming voor de camera of opslag.");
            }
        }


        private async Task UploadPhotoAsync(string filePath)
        {
            var url = "https://eoiwkpx5left1hk.m.pipedream.net"; // De URL waar je de foto naartoe wilt sturen.

            try
            {
                var photoBytes = await GetPhotoBytesAsync(filePath);

                if (photoBytes != null)
                {
                    // Stel de HTTP client in
                    using (var client = new HttpClient())
                    {
                        var content = new MultipartFormDataContent();

                        // Voeg de foto toe aan de request
                        var byteContent = new ByteArrayContent(photoBytes);
                        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                        content.Add(byteContent, "file", Path.GetFileName(filePath));

                        // Verstuur de foto via POST
                        var response = await client.PostAsync(url, content);

                        // Controleer het resultaat van de upload
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Foto succesvol geüpload!");
                        }
                        else
                        {
                            Console.WriteLine($"Fout bij uploaden van foto: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Er is een fout opgetreden: {ex.Message}");
            }
        }


        private async Task<byte[]> GetPhotoBytesAsync(string filePath)
        {
            byte[] photoBytes;

            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        photoBytes = memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Er is een fout opgetreden bij het lezen van de foto: {ex.Message}");
                return null;
            }

            return photoBytes;
        }






    }

}
