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
using AndroidChatApp.Models;
using System.Net;
using System.IO;

namespace AndroidChatApp.Activities
{
    [Activity(Label = "Register")]
    public class RegisterActivity : Activity
    {
        Button registerButton;
        EditText username, password;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Register);
            username = FindViewById<EditText>(Resource.Id.Username);
            password = FindViewById<EditText>(Resource.Id.Password);

            // Get our button from the layout resource,
            // and attach an event to it
            registerButton = FindViewById<Button>(Resource.Id.RegisterButton);

            // Add event to register. When the user click on the register button, OnRegister will be called.
            registerButton.Click += OnRegister;
        }

        // Clear the texts when return
        protected override void OnResume()
        {
            base.OnResume();

            username.Text =
                password.Text = string.Empty;
        }

        // Implement OnLogin method
        public void OnRegister(object sender, EventArgs e)
        {
            //**get register user's username and password from the textboxes.
            User NewRegisteredUser = new User();
            NewRegisteredUser.Username = username.Text.ToString();
            NewRegisteredUser.Password = password.Text.ToString();
            
            StartActivity(typeof(FriendsListActivity));

          
        }
    }
}