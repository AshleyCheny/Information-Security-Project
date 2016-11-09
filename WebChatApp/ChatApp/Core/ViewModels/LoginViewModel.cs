using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;

namespace ChatApp.Core.ViewModels
{
    public class LoginViewModel: BasicViewModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public async Task Login()
        {
            if (string.IsNullOrEmpty(Username))
                throw new Exception("Username is blank.");

            if (string.IsNullOrEmpty(Password))
                throw new Exception("Password is blank.");

            IsBusy = true;
            try
            {
                settings.User = await service.Login(Username, Password);
                settings.Save();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task RegisterPush(string deviceToken)
        {
            if (settings.User == null)
                throw new Exception("User is null");

            await service.RegisterPush(settings.User.Id, deviceToken);
        }
    }
}