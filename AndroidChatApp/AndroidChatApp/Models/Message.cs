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
        public uint MessageSenderRegisID { get; set; }
        public uint MessageReceiverRegisID { get; set; }
        public string MessageText { get; set; }
        public DateTime MessageTimestamp { get; set; }
    }
}