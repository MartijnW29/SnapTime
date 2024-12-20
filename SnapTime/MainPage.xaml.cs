﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;

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

        private async void TakePhotoAsync(object sender, EventArgs e)
        {
            if (await RequestPermissionsAsync())
            {
                try
                {
                    var photo = await MediaPicker.CapturePhotoAsync();

                    if (photo != null)
                    {
                        var photoPath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                        using (var stream = await photo.OpenReadAsync())
                        using (var fileStream = new FileStream(photoPath, FileMode.Create, FileAccess.Write))
                        {
                            await stream.CopyToAsync(fileStream);
                        }

                        Console.WriteLine($"Foto opgeslagen op: {photoPath}");

                        // Foto comprimeren en verkleinen
                        var optimizedPhotoBytes = await CompressAndResizePhotoAsync(photoPath);

                        if (optimizedPhotoBytes != null)
                        {
                            await UploadPhotoAsync(optimizedPhotoBytes, photo.FileName);
                        }
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

        private async Task<byte[]> CompressAndResizePhotoAsync(string filePath)
        {
            try
            {
                // Gebruik de volledige naam voor ImageSharp.Image
                using (var image = await SixLabors.ImageSharp.Image.LoadAsync(filePath))
                {
                    // Schaal de afbeelding naar een kleinere resolutie
                    image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Processing.ResizeOptions
                    {
                        Size = new SixLabors.ImageSharp.Size(500, 500), // Verklein naar maximaal 500x500
                        Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max
                    }));

                    using (var memoryStream = new MemoryStream())
                    {
                        var options = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = 100 };
                        await image.SaveAsJpegAsync(memoryStream, options);
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij comprimeren/verkleinen van afbeelding: {ex.Message}");
                return null;
            }
        }


        private async Task UploadPhotoAsync(byte[] photoBytes, string fileName)
        {
            var url = "https://eoiwkpx5left1hk.m.pipedream.net";

            try
            {
                if (photoBytes != null)
                {
                    using (var client = new HttpClient())
                    {
                        var content = new MultipartFormDataContent();

                        var byteContent = new ByteArrayContent(photoBytes);
                        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                        content.Add(byteContent, "file", fileName);

                        var response = await client.PostAsync(url, content);

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
    }
}
