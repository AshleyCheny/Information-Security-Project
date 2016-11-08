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
            login.Click += OnLogin;
        }

        //
        protected override void OnResume()
        {
            base.OnResume();

            username.Text = 
                password.Text = string.Empty;
        }

        //
        async void OnLogin(object sender, EventArgs e)
        {
            viewModel.Username = username.Text;
            viewModel.Password = password.Text;

            try
            {
                await viewModel.Login();     
                //TODO: navigate to a new activity    
            }
            catch (Exception exc)
            {
                DisplayError(exc);
            }
        }
    }
}