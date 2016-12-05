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
using AndroidChatApp.Models;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using Newtonsoft.Json;
using libsignal;
using libsignal.util;
using libsignal.state;
using Android.Preferences;
using libsignal.ecc;
using libsignal.state.impl;

namespace AndroidChatApp.Activities
{
    [Activity(Label = "Register")]
    public class RegisterActivity : Activity
    {
        Button registerButton;
        EditText username, password;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Register);
            username = FindViewById<EditText>(Resource.Id.Username);
            password = FindViewById<EditText>(Resource.Id.Password);

            // Get our button from the layout resource,
            // and attach an event to it
            registerButton = FindViewById<Button>(Resource.Id.RegisterButton);

            // Add event to register. When the user click on the register button, OnRegister will be called.
            registerButton.Click += OnRegister;
        }

        // Clear the texts when return
        protected override void OnResume()
        {
            base.OnResume();

            username.Text =
                password.Text = string.Empty;
        }

        // Implement OnLogin method
        public void OnRegister(object sender, EventArgs e)
        {
            IdentityKeyPair identityKeyPair = KeyHelper.generateIdentityKeyPair();
            uint registrationId = KeyHelper.generateRegistrationId(false);
            List<PreKeyRecord> preKeys = KeyHelper.generatePreKeys(registrationId, 100).ToList();
            Random random = new Random();
            SignedPreKeyRecord signedPreKey = KeyHelper.generateSignedPreKey(identityKeyPair, Convert.ToUInt32(random.Next()));

            InMemorySessionStore SessionStore = new InMemorySessionStore();
            //SessionStore.StoreSession(new SignalProtocolAddress("1234", 1), new SessionRecord());
            PreKeyStore PreKeyStore = new InMemoryPreKeyStore();
            SignedPreKeyStore SignedPreKeyStore = new InMemorySignedPreKeyStore();
            InMemoryIdentityKeyStore IdentityStore = new InMemoryIdentityKeyStore(identityKeyPair, registrationId);

            // Store preKeys in PreKeyStore.
            foreach (PreKeyRecord preKey in preKeys)
            {
                PreKeyStore.StorePreKey(preKey.getId(), preKey);
            }

            // Store signed prekey in SignedPreKeyStore.
            SignedPreKeyStore.StoreSignedPreKey(signedPreKey.getId(), signedPreKey);

            // Save stores in local Database
            ISharedPreferences sharedprefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = sharedprefs.Edit();
            editor.PutString("PreKeyStore", JsonConvert.SerializeObject(PreKeyStore));
            editor.PutString("SignedPreKeyStore", JsonConvert.SerializeObject(SignedPreKeyStore));
            List<Session> AllSessions = SessionStore.GetAllSessions();
            editor.PutString("AllSessions", JsonConvert.SerializeObject(AllSessions));
            editor.PutString("SessionStore", JsonConvert.SerializeObject(SessionStore));
            editor.PutString("IdentityStore", JsonConvert.SerializeObject(IdentityStore));
            List<TrustedKey> AllTrustedKeys = IdentityStore.GetAllTrustedKeys();
            editor.PutString("AllTrustedKeys", JsonConvert.SerializeObject(AllTrustedKeys));
            editor.Apply();

            //var sessionStore = JsonConvert.DeserializeObject<InMemorySessionStore>(sharedprefs.GetString("SessionStore", string.Empty));
            //var allSessions = JsonConvert.DeserializeObject<List<Session>>(sharedprefs.GetString("AllSessions", string.Empty));
            //foreach (Session item in allSessions)
            //{
            //    sessionStore.StoreSession(item.Name, item.DeviceID, item.array);
            //}

            // method to regidter in database
            RequestRegister(registrationId, identityKeyPair, signedPreKey, preKeys);
            StartActivity(typeof(FriendsActivity));
        }

        private void RequestRegister(uint registrationId, IdentityKeyPair identityKeyPair, SignedPreKeyRecord signedPreKey, List<PreKeyRecord> preKeys)
        {
            //Send the login username and password to the server and get response
            string apiUrl = "https://ycandgap.me/api_server2.php";
            string apiMethod = "registerUser";

            // Login_Request has two properties: username and password
            Login_Request myLogin_Request = new Login_Request();
            myLogin_Request.Username = username.Text;
            myLogin_Request.Password = password.Text.GetHashCode();
            myLogin_Request.RegistrationID = registrationId;
            myLogin_Request.PublicIdentityKey = JsonConvert.SerializeObject(identityKeyPair.getPublicKey().serialize());
            myLogin_Request.PublicSignedPreKeyID = signedPreKey.getId();
            myLogin_Request.PublicSignedPreKeySignature = JsonConvert.SerializeObject(signedPreKey.getSignature());
            myLogin_Request.PublicSignedPreKey = JsonConvert.SerializeObject(signedPreKey.getKeyPair().getPublicKey().serialize());

            // Save in local Database
            ISharedPreferences sharedprefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = sharedprefs.Edit();

            // Store identityKeyPair somewhere durable and safe.
            editor.PutString("IdentityKeyPair", JsonConvert.SerializeObject(identityKeyPair.serialize()));
            editor.PutString("SignedPreKey", JsonConvert.SerializeObject(signedPreKey.serialize()));
            editor.PutString("Username", username.Text);
            editor.PutInt("Password", password.Text.GetHashCode());

            // Store registrationId somewhere durable and safe.
            editor.PutString("RegistrationId", registrationId.ToString());
            editor.Apply();

            // make http post request
            string response = Http.Post(apiUrl, new NameValueCollection()
            {
                { "api_method", apiMethod                                    },
                { "api_data",   JsonConvert.SerializeObject(myLogin_Request) }
            });

            // decode json string to dto object
            API_Response r = JsonConvert.DeserializeObject<API_Response>(response);

            if (r!=null)
            {
                if (!r.IsError)
                {
                    foreach (PreKeyRecord preKey in preKeys)
                    {
                        Prekey_Request preKey_Request = new Prekey_Request();
                        preKey_Request.PublicSignedPreKeyID = signedPreKey.getId();
                        preKey_Request.PublicPreKeyID = preKey.getId();
                        preKey_Request.PublicPreKey = JsonConvert.SerializeObject(preKey.getKeyPair().getPublicKey().serialize());
                        apiMethod = "storePreKeys";

                        // make http post request
                        string preKeyResponse = Http.Post(apiUrl, new NameValueCollection()
                            {
                                { "api_method", apiMethod                                    },
                                { "api_data",   JsonConvert.SerializeObject(preKey_Request) }
                            });

                        // decode json string to dto object
                        API_Response preKeyR = JsonConvert.DeserializeObject<API_Response>(preKeyResponse);
                        if (preKeyR==null)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    internal class Prekey_Request
    {
        public uint PublicSignedPreKeyID { get; set; }
        public uint PublicPreKeyID { get; set; }
        public string PublicPreKey { get; set; }
    }
}