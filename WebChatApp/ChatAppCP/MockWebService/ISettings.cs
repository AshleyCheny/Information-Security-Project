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
    // Implement a simple interface for persisting application settings.
    public interface ISettings
    {
        // Define a User property.
        User User { get; set; }

        // Define a method Save.
        void Save();
    }
}