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
    public class Message
    {
        public string Id { get; set; }

        public string ToId { get; set; }

        public string ConversationId { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }
    }
}