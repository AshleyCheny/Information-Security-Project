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
using ChatApp.Core.Models;

namespace ChatApp.Core.ViewModels
{
    public class BasicViewModel
    {
        // Create an instance of IWebServices: service
        // 
        protected readonly IWebServices service =
            ServiceContainer.Resolve<IWebServices>();

        // Create an instance of ISettings: settings
        protected readonly ISettings settings =
            ServiceContainer.Resolve<ISettings>();

        // Indicate that if the ViewModel layer is busy.
        public event EventHandler IsBusyChanged = delegate { };

        private bool isBusy = false;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                IsBusyChanged(this, EventArgs.Empty);
            }
        }
    }
}