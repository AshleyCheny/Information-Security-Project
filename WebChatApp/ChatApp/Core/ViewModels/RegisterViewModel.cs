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
    // Create the RegisterViewModel for the user's registration. 
    public class RegisterViewModel : BasicViewModel
    {
        // These three properties will handle inputs from the user. 
        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        // Create the Register method.
        public async Task Register()
        {
            if (string.IsNullOrEmpty(Username))
                throw new Exception("Username is blank.");

            if (string.IsNullOrEmpty(Password))
                throw new Exception("Password is blank.");

            if (Password != ConfirmPassword)
                throw new Exception("Passwords don't match.");

            IsBusy = true;

            // Get a user's input and save them in the server's Username and Password properties.
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