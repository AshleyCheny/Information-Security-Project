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
using ChatApp.Core.ViewModels;
using System.Threading.Tasks;

namespace ChatApp.Activities
{
    // Set MainLauncher to true to make this activity the first activity for the application.
    [Activity(Label = "@string/ApplicationName", MainLauncher = true)]
    public class LoginActivity : BaseActivity<LoginViewModel>
    {
        EditText username, password;
        Button login;

        //
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.Login);
            username = FindViewById<EditText>(Resource.Id.username);
            password = FindViewById<EditText>(Resource.Id.password);
            login = FindViewById<Button>(Resource.Id.login);
            // Add event to login. When the user click on the Login button, Onlogin will be called.
            login.Click += OnLogin;
        }

        //
        protected override void OnResume()
        {
            base.OnResume();

            username.Text = 
                password.Text = string.Empty;
        }

        // Implement OnLogin method
        async void OnLogin(object sender, EventArgs e)
        {
            viewModel.Username = username.Text;
            viewModel.Password = password.Text;

            try
            {
                 await viewModel.Login();
                    // **TODO: navigate to a new activity  
                    StartActivity(typeof(ConversationsActivity));
                
            }
            catch (Exception exc)
            {
                DisplayError(exc);
            }
        }
    }
}