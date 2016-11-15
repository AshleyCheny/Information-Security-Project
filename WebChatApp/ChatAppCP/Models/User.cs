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

namespace ChatAppCP.Models
{
    //Define the User class 
    public class User
    {
        //The User class has three properties to represent a user.
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}