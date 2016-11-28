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

namespace AndroidChatApp.Activities
{
    //**use friend name as Label in this ChatList page
    [Activity(Label = "ChatListActivity")]
    public class MessagesActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "ChatList" layout resource
            SetContentView(Resource.Layout.Message);

            // **implement messages sending and receiving here
           
        }

    }
}