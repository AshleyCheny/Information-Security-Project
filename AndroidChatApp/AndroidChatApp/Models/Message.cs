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
    public class Message
    {
        public int MessageID { get; set; }
        public int MessageSenderRegisID { get; set; }
        public int MessageReceiverRegisID { get; set; }
        public string MessageText { get; set; }
        public DateTime MessageTimestamp { get; set; }
    }
}