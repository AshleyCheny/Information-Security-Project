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
using Google.ProtocolBuffers;

namespace AndroidChatApp.Models
{
    public class User
    {
        //One user will have its username, password, 
        public string Username { get; set; }
        public string Password { get; set; }
        public uint RegisterationID { get; set; }

        //keys using for encryption and decryption
        //public byte[] IdentityKey { get; set; }
        //public ByteString PreKeys { get; set; }
        //public ByteString SignedPreKeys { get; set; }
        //public ByteString PrivateKey { get; set; }
        public string IdentityKey { get; set; }
        public string PreKeys { get; set; }
        public string SignedPreKey { get; set; }
        public string PrivateKey { get; set; }
        public string LastMessage { get; set; }
        public uint SignedPreKeyID { get; set; }
        public string SignedPreKeySignature { get; set; }
    }
}