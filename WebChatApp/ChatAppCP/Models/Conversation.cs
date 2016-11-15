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
    // Create a class to represent a conversation.
    public class Conversation
    {
        // A conversation has three properties including Id, UserId and Username.
        // A user will be included in a conversation, so Conversation class needs to save User's id and name.
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}