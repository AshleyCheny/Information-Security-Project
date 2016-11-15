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
    // We subclassed BasicViewModel to get access to IsBusy and the fields containing common services 
    public class LoginViewModel: BasicViewModel
    {
        // We added the Username and Password properties to be set by the View layer 
        public string Username { get; set; }

        public string Password { get; set; }

        // We implemented a Login method to be called from View, with validation on Username and Password properties.
        public async Task Login()
        {
            if (string.IsNullOrEmpty(Username))
                throw new Exception("Username is blank.");

            if (string.IsNullOrEmpty(Password))
                throw new Exception("Password is blank.");

            // We set IsBusy during the call to the Login method on IWebService 
            IsBusy = true;
            try
            {
                // We set the User property by awaiting the result from Login on the web service 
                settings.User = await service.Login(Username, Password);
                settings.Save();
            }
            finally
            {
                // This will ensure it gets reset properly even when an exception is thrown.
                IsBusy = false;
            }
        }

        //public async Task RegisterPush(string deviceToken)
        //{
        //    if (settings.User == null)
        //        throw new Exception("User is null");

        //    await service.RegisterPush(settings.User.Id, deviceToken);
        //}
    }
}