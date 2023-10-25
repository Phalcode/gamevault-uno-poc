using GameVaultUnoPoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameVaultUnoPoc.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        #region Singleton
        private static SettingsViewModel instance = null;
        private static readonly object padlock = new object();

        public static SettingsViewModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SettingsViewModel();
                    }
                    return instance;
                }
            }
        }
        #endregion
        private string username { get; set; }
        private string password { get; set; }
        private string serverurl { get; set; }
        private bool stayLoggedIn { get; set; } = true;
        private User user { get; set; }

        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged(); }
        }
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }
        public string ServerUrl
        {
            get { return serverurl; }
            set { serverurl = value; OnPropertyChanged(); }
        }
        public bool StayLoggedIn
        {
            get { return stayLoggedIn; }
            set { stayLoggedIn = value; OnPropertyChanged(); }
        }
        public User User
        {
            get { return user; }
            set { user = value; OnPropertyChanged(); }
        }
    }
}
