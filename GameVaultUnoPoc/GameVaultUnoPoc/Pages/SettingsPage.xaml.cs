using GameVaultUnoPoc.Helper;
using GameVaultUnoPoc.Models;
using GameVaultUnoPoc.ViewModels;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;


#if __WASM__
using Uno.Web.Http;
#endif

namespace GameVaultUnoPoc.Pages
{
    public sealed partial class SettingsPage : UserControl
    {

        public SettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = SettingsViewModel.Instance;
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (SettingsViewModel.Instance.User != null)
            {
                return;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string savefile = $"{AppDomain.CurrentDomain.BaseDirectory}save.txt";
                if (File.Exists(savefile))
                {
                    string[] result = File.ReadAllText(savefile).Split(";");
                    SettingsViewModel.Instance.Username = result[0];
                    SettingsViewModel.Instance.Password = result[1];
                    SettingsViewModel.Instance.ServerUrl = result[2];
                    string userResult = await WebHelper.GetRequest(SettingsViewModel.Instance.ServerUrl + "api/users/me");
                    SettingsViewModel.Instance.User = JsonSerializer.Deserialize<User>(userResult);
                    uiImgUser.Source = await WebHelper.DownloadImageFromUrlAsync($"{SettingsViewModel.Instance.ServerUrl}api/images/{SettingsViewModel.Instance.User.ProfilePicture.ID}");
                }
            }
            else
            {
#if __WASM__

var cookies = Uno.Web.Http.CookieManager.GetDefault().GetCookies();
foreach (var cookie in cookies)
{
if(cookie.Name == "Auth")
{
string cookieValue = cookie.Value;
string[] result = File.ReadAllText(cookieValue).Split(";");
                    SettingsViewModel.Instance.Username = result[0];
                    SettingsViewModel.Instance.Password = result[1];
                    SettingsViewModel.Instance.ServerUrl = result[2];

}
}

#endif
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsViewModel.Instance.StayLoggedIn)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string savefile = $"{AppDomain.CurrentDomain.BaseDirectory}save.txt";
                    if (!File.Exists(savefile))
                    {
                        File.Create(savefile).Close();
                    }
                    File.WriteAllText(savefile, $"{SettingsViewModel.Instance.Username};{SettingsViewModel.Instance.Password};{SettingsViewModel.Instance.ServerUrl}");
                }
                else
                {
                    Console.WriteLine("#################ISNOTWINDOWS");
#if __WASM__
                 Console.WriteLine("#################ISWASM");  
var cookie = new Uno.Web.Http.Cookie("Auth", $"{SettingsViewModel.Instance.Username};{SettingsViewModel.Instance.Password};{SettingsViewModel.Instance.ServerUrl}");
var request = new Uno.Web.Http.SetCookieRequest(cookie)
{
	Path = "/",
	Expires = DateTimeOffset.UtcNow.AddDays(2),
	Secure = true,
};

Uno.Web.Http.CookieManager.GetDefault().SetCookie(request);

#endif

                }
            }
            string result = await WebHelper.GetRequest(SettingsViewModel.Instance.ServerUrl + "api/users/me");
            SettingsViewModel.Instance.User = JsonSerializer.Deserialize<User>(result);
            uiImgUser.Source = await WebHelper.DownloadImageFromUrlAsync($"{SettingsViewModel.Instance.ServerUrl}api/v1/images/{SettingsViewModel.Instance.User.ProfilePicture.ID}");
        }

        private void ClearSave_Click(object sender, RoutedEventArgs e)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string savefile = $"{AppDomain.CurrentDomain.BaseDirectory}save.txt";
                if (File.Exists(savefile))
                {
                    File.Delete(savefile);
                }
            }
            else
            {
#if __WASM__

                CookieManager.GetDefault().DeleteCookie("Auth", path: "/");

#endif
            }
        }
    }
}
