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

namespace AndroidChatApp.Models
{
    public class Conversation
    {
        // In the FriendList page, there will be a friend list.
        public int ConversationID { get; set; }
        public int FriendRegisID { get; set; }
        public string FriendName { get; set; }
        public string LastMessage { get; set; }

    }
}