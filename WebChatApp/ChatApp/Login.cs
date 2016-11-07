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

namespace ChatApp
{
    [Activity(Label = "Activity1")]
    public class Login : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Login);

            //?
            StartActivity(typeof(Login));

            var finish = FindViewById<Button>(Resource.Id.finish);
            finish.Click += (sender, e) => Finish();
        }
    }
}