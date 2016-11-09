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
    public class Conversation
    {
        public int Id { get; set;}
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}