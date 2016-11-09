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

namespace ChatApp.Activities
{
    // Implement a simple base class for all the activities throughout our app. 
    [Activity(Label = "BaseActivity")]
    // Make this class generic
    public class BaseActivity<TViewModel> : Activity
        where TViewModel : BasicViewModel
    {
        // Define a protected variable named viewModel to store a view model of specific type.
        protected readonly TViewModel viewModel;
        protected ProgressDialog progress;

        //
        public BaseActivity()
        {
            viewModel = ServiceContainer.Resolve(typeof(TViewModel)) as TViewModel;
        }

        //
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            progress = new ProgressDialog(this);
            progress.SetCancelable(false);
            progress.SetTitle(Resource.String.Loading);
        }

        //
        protected override void OnResume()
        {
            base.OnResume();

            viewModel.IsBusyChanged += OnIsBusyChanged;
        }

        //
        protected override void OnPause()
        {
            base.OnPause();

            viewModel.IsBusyChanged -= OnIsBusyChanged;
        }

        // Displayed a simple ProgressBar function with the Loading string from the Strings.xml file to indicate network activity. 
        void OnIsBusyChanged(object sender, EventArgs e)
        {
            if (viewModel.IsBusy)
                progress.Show();
            else progress.Hide();
        }

        // Display errors to the user by a pop-up dialog indicating something went wrong.
        protected void DisplayError(Exception exc)
        {
            string error = exc.Message;
            new AlertDialog.Builder(this)
                .SetTitle(Resource.String.ErrorTitle)
                .SetMessage(error)
                .SetPositiveButton(Android.Resource.String.Ok,
                (IDialogInterfaceOnClickListener)null)
                .Show();
        }


    }
}