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
using ChatAppCP.Models;

namespace ChatAppCP.MockWebService
{
    public class FakeSettings : ISettings
    {
        public User User { get; set; }

        public void Save() { }
    }
}