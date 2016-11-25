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
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;

namespace AndroidChatApp.Activities
{
    [Activity(Label = "Login", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        EditText username, password;
        Button loginButton;
        TextView registerLink;

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
            string apiUrl = "https://ycandgap.me/api_server2.php";
            string apiMethod = "loginUser";

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

            // check response
            if (!r.IsError && r.ResponseData == "SUCCESS")
            {
                //if login successfully, go to FriendsList page
                StartActivity(typeof(FriendsListActivity));
            }
            else
            {
                //if login fails, pop up an alert message. Wrong username or password or a new user
                AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
                dialogBuilder.SetMessage(r.ErrorMessage);
                //dialogBuilder.SetPositiveButton("Ok", null);
                dialogBuilder.Show();
            }
        }

        //when user click on the register link, it will go to the Register Page.
        public void OnRegisterLink(object sender, EventArgs e)
        {

            StartActivity(typeof(RegisterActivity));
        }

    } //END OF Class

    public class Login_Request
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public static class Http
    {
        public static string Post(string uri, NameValueCollection pairs)
        {
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, pairs);
            }
            return Encoding.Default.GetString(response);
        }
    }

    public class API_Response
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string ResponseData { get; set; }
    }
} //End of Namespace