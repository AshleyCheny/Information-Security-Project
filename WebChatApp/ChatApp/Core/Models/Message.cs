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

namespace ChatApp.Core.Models
{
    // A class to store a message's infomation.
    // A message has its Id.
    // the ConversationId it belongs to.
    // Who send the message: UserId and Username.
    // The content of the message: Text.
    // When send the message: Date.
    public class Message
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }
    }
}