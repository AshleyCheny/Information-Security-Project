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
    [Activity(Label = "Friends")]
    public class FriendsListActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "FriendsList" layout resource
            SetContentView(Resource.Layout.FriendsList);

            // **List out all the registered friends in this FriendsList Page
            // **If two clients have already talked before, display the last message or unread message here.
            // **If two clients haven't talked before, when click on one friend, do initiating session setup

        }
    }
}