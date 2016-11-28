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
using System.Collections.Specialized;
using Newtonsoft.Json;

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
            //Send the login username and password to the server and get response
            string apiUrl = "https://ycandgap.me/api_server2.php";
            string apiMethod = "registerUser";
            
            // Login_Request has two properties: username and password
            Login_Request myLogin_Request = new Login_Request();
            myLogin_Request.username = username.Text;
            myLogin_Request.password = password.Text;

            // make http post request
            string response = Http.Post(apiUrl, new NameValueCollection()
            {
                { "api_method", apiMethod                                    },
                { "api_data",   JsonConvert.SerializeObject(myLogin_Request) }
            });

            // decode json string to dto object
            API_Response r = JsonConvert.DeserializeObject<API_Response>(response);

            StartActivity(typeof(ConversationActivity));

          
        }
    }
}