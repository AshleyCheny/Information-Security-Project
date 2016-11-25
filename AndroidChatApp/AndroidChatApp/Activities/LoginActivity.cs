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

namespace AndroidChatApp.Activities
{
    [Activity(Label = "Login", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        EditText username, password;
        Button loginButton;
        TextView registerLink;

        //**when user tries to login, it should have access to local store
        UserLocalStore userLocalStore;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "Login" layout resource
            SetContentView(Resource.Layout.Login);
            username = FindViewById<EditText>(Resource.Id.Username);
            password = FindViewById<EditText>(Resource.Id.Password);
            registerLink = FindViewById<TextView>(Resource.Id.RegisterLink);

            // Get our button from the layout resource,
            // and attach an event to it
            loginButton = FindViewById<Button>(Resource.Id.LoginButton);
       
            // Add event to login. When the user click on the Login button, Onlogin will be called.
            loginButton.Click += OnLogin;

            // add event to registerLink
            registerLink.Click += OnRegisterLink;

            //**create an instance of UserLocalStore class
            userLocalStore = new UserLocalStore(this);
        }

        
        // Clear the texts when return
        protected override void OnResume()
        {
            base.OnResume();

            username.Text =
                password.Text = string.Empty;
        }

        // Implement OnLogin method
        // username and password VERIFICATION here! (send data to login.php in server to verify)
        // if verify sucessfully, go to the Friends page
        // else pop up an error message in this Login page
        public void OnLogin(object sender, EventArgs e)
        {
            //Send the login username and password to the server and get response

            //**save loggedin user's information to local database
            User LoggedInUser = new User();
            userLocalStore.StoreUserData(LoggedInUser);
            userLocalStore.SetUserLoggedIn(true);
            //StartActivity(typeof(FriendsListActivity));

           
        }

        //when user click on the register link, it will go to the Register Page.
        public void OnRegisterLink(object sender, EventArgs e)
        {
            StartActivity(typeof(RegisterActivity));
        }

    } //END OF Class
} //End of Namespace