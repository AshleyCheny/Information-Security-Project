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
    // Create a Message class to represent a message.
    public class Message
    {
        // A message 5 properties.
        // A message will be in a conversation and sent by a user, so it will need to save conversation and user infomation.
        public int Id { get; set; }
        public string Text { get; set; }

        public int ConversationId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}