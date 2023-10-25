using GameVaultUnoPoc.ViewModels;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace GameVaultUnoPoc.Helper
{
    internal class WebHelper
    {
        internal async static Task<string> GetRequest(string uri)
        {
            using (var client = new HttpClient())
            {
                var authenticationString = $"{SettingsViewModel.Instance.Username}:{SettingsViewModel.Instance.Password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        public static async Task<BitmapImage> DownloadImageFromUrlAsync(string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                var authenticationString = $"{SettingsViewModel.Instance.Username}:{SettingsViewModel.Instance.Password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
                long offset = 0;
                using (HttpResponseMessage response = await client.GetAsync(imageUrl))
                {
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        using (MemoryStream fs = new MemoryStream())
                        {
                            while (streamToReadFrom.Position < streamToReadFrom.Length)
                            {
                                var nextChunkSize = Math.Min(1024, (int)(streamToReadFrom.Length - offset));
                                var data = new byte[nextChunkSize];

                                streamToReadFrom.Read(data, 0, nextChunkSize);
                                fs.Write(data, 0, nextChunkSize);
                                offset += nextChunkSize;
                            }
                            return await GetBitmapAsync(fs.ToArray());
                        }
                    }
                }
            }
        }
        public static async Task<BitmapImage> GetBitmapAsync(byte[] data)
        {
            var bitmapImage = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(stream))
                {
                    writer.WriteBytes(data);
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                    writer.DetachStream();
                }

                stream.Seek(0);
                await bitmapImage.SetSourceAsync(stream);
            }

            return bitmapImage;
        }
    }
}
