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
using ChatApp.Core.Models;

namespace ChatApp.Core.ViewModels
{
    public class RegisterViewModel : BasicViewModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public async Task Register()
        {
            if (string.IsNullOrEmpty(Username))
                throw new Exception("Username is blank.");

            if (string.IsNullOrEmpty(Password))
                throw new Exception("Password is blank.");

            if (Password != ConfirmPassword)
                throw new Exception("Passwords don't match.");

            IsBusy = true;
            try
            {
                settings.User = await service.Register(new User
                {
                    Username = Username,
                    Password = Password,
                });
                settings.Save();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}