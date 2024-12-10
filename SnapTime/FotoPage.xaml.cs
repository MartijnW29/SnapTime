using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SnapTime.Services;
using System.IO;
using System.Net;

namespace SnapTime;

public partial class FotoPage : ContentPage
{
    private object SnapletsEarned;

    public FotoPage()
	{
		InitializeComponent();
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

    private async void TakePictureAsync(object sender, EventArgs e)
    {
        if (await RequestPermissionsAsync())
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    LoadingOverlay.IsVisible = true;

                    var photoPath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                    using (var stream = await photo.OpenReadAsync())
                    using (var fileStream = new FileStream(photoPath, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    Console.WriteLine($"Foto opgeslagen op: {photoPath}");

                    // Foto comprimeren en verkleinen
                    var optimizedPhotoBytes = await CompressAndResizePictureAsync(photoPath);

                    if (optimizedPhotoBytes != null)
                    {
                        await UploadPictureAsync(optimizedPhotoBytes, photo.FileName);
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

    private async Task<byte[]> CompressAndResizePictureAsync(string filePath)
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


    private async Task UploadPictureAsync(byte[] photoBytes, string fileName)
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

                    if (App.CurrentUser != null && (int)response.StatusCode >= 200 && (int)response.StatusCode <= 300)
                    {
                        

                        // score voor je foto
                        if ((int)response.StatusCode == 200)
                        {
                            App.CurrentUser.Snaplets += 0;
                            SnapletsEarned = 0;
                        }
                        else if ((int)response.StatusCode == 205)
                        {
                            App.CurrentUser.Snaplets -= 5;
                            SnapletsEarned = -5;
                        }
                        else if((int)response.StatusCode == 210)
                        {
                            App.CurrentUser.Snaplets += 1;
                            SnapletsEarned = 1;
                        }
                        else if ((int)response.StatusCode == 220)
                        {
                            App.CurrentUser.Snaplets += 2;
                            SnapletsEarned = 2;
                        }
                        else if ((int)response.StatusCode == 230)
                        {
                            App.CurrentUser.Snaplets += 3;
                            SnapletsEarned = 3;
                        }
                        else if ((int)response.StatusCode == 299)
                        {
                            Console.WriteLine("Fout in Pipedream");
                        }

                        // Verhoog het aantal foto's lokaal
                        App.CurrentUser.TotalPicturesTaken += 1;

                        // Synchroniseer met Firebase
                        var firebaseHelper = new FirebaseHelper(); // Gebruik jouw Firebase-helper

                        await firebaseHelper.UpdateSpecificUser(App.CurrentUser.Id.ToString(), App.CurrentUser);

                        Console.WriteLine($"Totaal aantal foto's bijgewerkt: {App.CurrentUser.TotalPicturesTaken}");
                        Console.WriteLine($"Totaal aantal Snaplets bijgewerkt: {App.CurrentUser.Snaplets}");


                        await ReloadHomePage();
                        LoadingOverlay.IsVisible = false;
                        await DisplayAlert("Foto is verwerkt", $"Je hebt {SnapletsEarned} Snaplets verdient!", "OK");

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

    private async Task ReloadHomePage()
    {
        // Herlaad de huidige pagina
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var currentPage = Application.Current.MainPage?.Navigation?.NavigationStack.LastOrDefault();

            if (currentPage != null)
            {
                // Verwijder de huidige pagina en navigeer opnieuw
                await Application.Current.MainPage.Navigation.PopAsync(false); // Verwijder de oude pagina
                await Application.Current.MainPage.Navigation.PushAsync(new MainBar(2), false); // Voeg een nieuwe toe
            }
            else
            {
                // Als er geen navigatie-stack is, stel gewoon de MainPage in
                Application.Current.MainPage = new MainBar(2);
            }
        });
    }


}