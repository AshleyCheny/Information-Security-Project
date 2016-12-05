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
using System.Collections.Specialized;
using Newtonsoft.Json;
using AndroidChatApp.Models;
using System.Threading;
using Android.Preferences;
using libsignal;
using libsignal.state;
using libsignal.state.impl;
using libsignal.ecc;
using libsignal.protocol;

namespace AndroidChatApp.Activities
{
    //**use friend name as Label in this ChatList page
    [Activity(Label = "Messages")]
    public class MessagesActivity : Activity
    {
        public Models.Message[] MyMessages = new Models.Message[] { };
        public Models.Message MyMessage = new Models.Message();
        public Models.Message[] TheirMessages = new Models.Message[] { };
        public Models.Message TheirMessage = new Models.Message();
        //  This displays views vertically in a list with the help of an adapter class that determines the number of child views. It also has support for its children to be selected. 
        ListView listView;
        EditText messageText;
        Button sendButton;
        Adapter adapter;
        uint UserID;

        protected override void OnCreate(Bundle bundle)
        {
            // Getting saved data
            ISharedPreferences sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            User SelectedFriend = JsonConvert.DeserializeObject<User>(sharedPref.GetString("SelectedFriend", string.Empty));
            InMemorySessionStore sessionStore = JsonConvert.DeserializeObject<InMemorySessionStore>(sharedPref.GetString("SessionStore", string.Empty));
            var allSessions = JsonConvert.DeserializeObject<List<Session>>(sharedPref.GetString("AllSessions", string.Empty));
            foreach (Session item in allSessions)
            {
                sessionStore.StoreSession(item.Name, item.DeviceID, item.array);
            }
            PreKeyStore preKeyStore = JsonConvert.DeserializeObject<InMemoryPreKeyStore>(sharedPref.GetString("PreKeyStore", string.Empty));
            SignedPreKeyStore signedPreKeyStore = JsonConvert.DeserializeObject<InMemorySignedPreKeyStore>(sharedPref.GetString("SignedPreKeyStore", string.Empty));
            InMemoryIdentityKeyStore identityStore = JsonConvert.DeserializeObject<InMemoryIdentityKeyStore>(sharedPref.GetString("IdentityStore", string.Empty));
            IdentityKeyPair KeyPair = new IdentityKeyPair(JsonConvert.DeserializeObject<byte[]>(sharedPref.GetString("IdentityKeyPair", string.Empty)));
            uint RegistrationID = Convert.ToUInt32(sharedPref.GetString("RegistrationId", string.Empty));
            identityStore.PutValues(KeyPair, RegistrationID);
            var allTrustedKeys = JsonConvert.DeserializeObject<List<TrustedKey>>(sharedPref.GetString("AllTrustedKeys", string.Empty));
            foreach (TrustedKey item in allTrustedKeys)
            {
                identityStore.SaveIdentity(item.Name, new IdentityKey(item.Identity, 0));
            }
            SignalProtocolAddress SelectedFriendAddress = new SignalProtocolAddress(SelectedFriend.RegisterationID.ToString(), 1);

            // Get the messages from the server
            base.OnCreate(bundle);
            TheirMessages = GetMessages(sharedPref, SelectedFriend, sessionStore, preKeyStore, signedPreKeyStore, identityStore, SelectedFriendAddress);
            // Set our view from the "ChatList" layout resource
            Title = SelectedFriend.Username;
            SetContentView(Resource.Layout.Message);
            listView = FindViewById<ListView>(Resource.Id.messageList);
            //***display FriendsListItem in ListView using Adapter
            listView.Adapter = adapter = new Adapter(this, TheirMessages, UserID);
            messageText = FindViewById<EditText>(Resource.Id.messageText);
            sendButton = FindViewById<Button>(Resource.Id.sendButton);

            // **implement messages sending and receiving here
            //listView.Adapter = adapter = new Adapter(this);

            sendButton.Click += (sender, e) =>
            {
                SessionCipher sessionCipher = new SessionCipher(sessionStore, preKeyStore, signedPreKeyStore, identityStore, SelectedFriendAddress);
                CiphertextMessage cipherMessage = sessionCipher.encrypt(Encoding.UTF8.GetBytes(messageText.Text));

                // Save stores in local Database
                ISharedPreferences sharedprefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = sharedprefs.Edit();
                editor.PutString("PreKeyStore", JsonConvert.SerializeObject(preKeyStore));
                editor.PutString("SignedPreKeyStore", JsonConvert.SerializeObject(signedPreKeyStore));
                List<Session> AllSessions = sessionStore.GetAllSessions();
                editor.PutString("AllSessions", JsonConvert.SerializeObject(AllSessions));
                editor.PutString("SessionStore", JsonConvert.SerializeObject(sessionStore));
                editor.PutString("IdentityStore", JsonConvert.SerializeObject(identityStore));
                List<TrustedKey> AllTrustedKeys = identityStore.GetAllTrustedKeys();
                editor.PutString("AllTrustedKeys", JsonConvert.SerializeObject(AllTrustedKeys));
                editor.Apply();

                Models.Message message = new Models.Message();
                message.MessageID = 4;
                message.MessageReceiverRegisID = SelectedFriend.RegisterationID;
                message.MessageSenderRegisID = Convert.ToUInt32(sharedPref.GetString("RegistrationId", string.Empty));
                message.MessageText = JsonConvert.SerializeObject(cipherMessage.serialize());
                message.MessageTimestamp = DateTime.Now;

                //  call SendMessage() to send the 
                SendMessage(message);
                //  *display the messages in user's own screen using adapter (always display in MyMessageListItem).
                adapter.NotifyDataSetInvalidated();
                listView.SetSelection(adapter.Count);

            };           
        }

        public Models.Message[] GetMessages(ISharedPreferences sharedPref, User SelectedFriend, SessionStore sessionStore, 
            PreKeyStore preKeyStore, SignedPreKeyStore signedPreKeyStore, IdentityKeyStore identityStore, SignalProtocolAddress SelectedFriendAddress)
        {
            // send the server the user name
            // server side does the selection and return the messages and store it in a message array.
            //Send the login username and password to the server and get response
            string apiUrl = "https://ycandgap.me/api_server2.php";
            string apiMethod = "getMessage";

            //Login_Request has two properties:username and password
            Login_Request myLogin_Request = new Login_Request();
            Models.Message recieveMessage = new Models.Message();

            //get the login username from previow login page.
            recieveMessage.MessageReceiverRegisID = Convert.ToUInt32(sharedPref.GetString("RegistrationId", string.Empty));
            recieveMessage.MessageSenderRegisID = SelectedFriend.RegisterationID;
            myLogin_Request.message = recieveMessage;
            UserID = myLogin_Request.RegistrationID;


            // make http post request
            string response = Http.Post(apiUrl, new NameValueCollection()
                {
                    { "api_method", apiMethod                                    },
                    { "api_data",   JsonConvert.SerializeObject(myLogin_Request) }
                });

            // decode json string to dto object
            API_Response2 r = JsonConvert.DeserializeObject<API_Response2>(response);

            // check response
            if (r != null)
            {
                if (!r.IsError)
                {
                    SessionCipher sessionCipher = new SessionCipher(sessionStore, preKeyStore, signedPreKeyStore, identityStore, SelectedFriendAddress);
                    byte[] decipherMessage = sessionCipher.decrypt(new PreKeySignalMessage((JsonConvert.DeserializeObject<byte[]>(r.MessageText))));
                    string checkMessage = Encoding.UTF8.GetString(decipherMessage);

                    return TheirMessages = new Models.Message[] {new Models.Message { MessageID = r.MessageID, MessageSenderRegisID = r.MessageSenderRegisID,
                        MessageReceiverRegisID = r.MessageReceiverRegisID, MessageText = checkMessage, MessageTimestamp = r.MessageTimestamp} };
                }
                else
                {
                    //if login fails, pop up an alert message. Wrong username or password or a new user
                    AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
                    dialogBuilder.SetMessage(r.ErrorMessage);
                    //dialogBuilder.SetPositiveButton("Ok", null);
                    dialogBuilder.Show();
                    return null;

                }
            }
            else
            {
                if (!sessionStore.ContainsSession(SelectedFriendAddress))
                {
                    // Instantiate a SessionBuilder for a remote recipientId + deviceId tuple.
                    SessionBuilder sessionBuilder = new SessionBuilder(sessionStore, preKeyStore, signedPreKeyStore,
                                                                       identityStore, SelectedFriendAddress);
                    RetrievedPreKey preKeyPublic = RetrieveSelectedFriendPublicPreKey(SelectedFriend);
                    IdentityKey SelectedFriendSignedPreKey = new IdentityKey(JsonConvert.DeserializeObject<byte[]>(SelectedFriend.SignedPreKey), 0);
                    PreKeyBundle retrievedPreKey = new PreKeyBundle(SelectedFriend.RegisterationID, 1, preKeyPublic.PrekeyID, preKeyPublic.PublicPreKey.getPublicKey()
                        , SelectedFriend.SignedPreKeyID, SelectedFriendSignedPreKey.getPublicKey(), JsonConvert.DeserializeObject<byte[]>(SelectedFriend.SignedPreKeySignature)
                        ,new IdentityKey(JsonConvert.DeserializeObject<byte[]>(SelectedFriend.IdentityKey), 0));
                    // Build a session with a PreKey retrieved from the server.
                    sessionBuilder.process(retrievedPreKey);
                }
                return null;
            }
        }

        private RetrievedPreKey RetrieveSelectedFriendPublicPreKey(User SelectedFriend)
        {
            //Send the SignedPreKey to the server and get the corresponding PreKey response
            string apiUrl = "https://ycandgap.me/api_server2.php";
            string apiMethod = "getPreKey";

            Prekey_Request retrievePreKey_Request = new Prekey_Request();
            //get the user SignedPreKey.
            ISharedPreferences sharedprefs = PreferenceManager.GetDefaultSharedPreferences(this);
            retrievePreKey_Request.PublicSignedPreKeyID = SelectedFriend.SignedPreKeyID;

            // make http post request
            string response = Http.Post(apiUrl, new NameValueCollection()
                {
                    { "api_method", apiMethod                                    },
                    { "api_data",   JsonConvert.SerializeObject(retrievePreKey_Request) }
                });

            // decode json string to dto object
            API_Response3 r = JsonConvert.DeserializeObject<API_Response3>(response);

            // check response
            if (r != null)
            {
                if (!r.IsError)
                {
                    return new RetrievedPreKey() { PublicPreKey = new IdentityKey(JsonConvert.DeserializeObject<byte[]>(r.PreKey),0), PrekeyID = Convert.ToUInt32(r.PreKeyID) };
                }
                else
                {
                    //if login fails, pop up an alert message. Wrong username or password or a new user
                    AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
                    dialogBuilder.SetMessage(r.ErrorMessage);
                    //dialogBuilder.SetPositiveButton("Ok", null);
                    dialogBuilder.Show();
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void SendMessage(Models.Message message)
        {
            //sent messages to the server
            string apiUrl = "https://ycandgap.me/api_server2.php";
            string apiMethod = "sendMessages";

            //Login_Request has two properties:username and password
            Login_Request mySendMessage_Request = new Login_Request();
            //get the login username from previow login page.
            mySendMessage_Request.message = message;

            // make http post request
            string response = Http.Post(apiUrl, new NameValueCollection()
                {
                    { "api_method", apiMethod                                    },
                    { "api_data",   JsonConvert.SerializeObject(mySendMessage_Request) }
                });
        }

        //// Next, we'll need to implement OnResume to load the messages, invalidate the adapter, and then scroll the list view to the end,
        //protected override void OnResume()
        //{
        //    base.OnResume();

        //    GetMessages();
        //    adapter.NotifyDataSetInvalidated();
        //    listView.SetSelection(adapter.Count);
        //}

        //private void GetMessages()
        //{
        //    throw new NotImplementedException();
        //}

        // Create a subclasse of BaseAdapter<Conversation>: Adapter
        //Connect database and UI
        // adapter holds data from database and send the data to dapter view
        public class Adapter : BaseAdapter<Models.Message>
        {
            Models.Message[] theirMessages;
            Activity context;
            const int MyMessageType = 0, TheirMessageType = 1;
            uint UserID;
            //readonly LayoutInflater inflater;

            // We passed in a Context parameter (our activity) so that we can pull out the LayoutInflater. 
            // This class enables us to load XML layout resources and inflate them into a view object.
            public Adapter(Activity context, Models.Message[] theirMessages, uint userID) : base()
            {
                this.context = context;
                this.theirMessages = theirMessages;
                this.UserID = userID;
                //inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            }
            //  Implement GetItemId method. 
            // This is a general method used to identify rows, so try to return a unique number. 
            public override long GetItemId(int position)
            {
                return position;
            }

            public override int GetItemViewType(int position)
            {
                var message = this[position];
                return message.MessageSenderRegisID == UserID ? MyMessageType : TheirMessageType;
            }

            // We set up GetView, which recycles the convertView variable by only creating a new view if it is null. 
            // We also pulled out the text views in our layout to set their text. 
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                //  First pull out the message object for the position of the row. 
                var message = this[position];
                int type = GetItemViewType(position);

                // Next, we grab the view type that determines whether it is the current user's message or the other user in the conversation.
                // If convertView is null, we inflate the appropriate layout based on the type. 
                if (convertView == null)
                {
                    if (type == MyMessageType)
                    {
                        convertView = context.LayoutInflater.Inflate(Resource.Layout.MyMessageListItem, null);
                    }
                    else
                    {
                        convertView = context.LayoutInflater.Inflate(Resource.Layout.TheirMessageListItem, null);
                    }
                }

                // Next, we pull the two text views, messageText and dateText, out of convertView. 
                // We have to use the type value to make sure we use the correct resource IDs. 
                TextView messageText, dateText;
                if (type == MyMessageType)
                {
                    messageText = convertView.FindViewById<TextView>(Resource.Id.myMessageText);
                    dateText = convertView.FindViewById<TextView>(Resource.Id.myMessageDate);
                }
                else
                {
                    messageText = convertView.FindViewById<TextView>(Resource.Id.theirMessageText);
                    dateText = convertView.FindViewById<TextView>(Resource.Id.theirMessageDate);
                }

                // We set the appropriate text on both text views using the message object. 
                messageText.Text = message.MessageText;
                dateText.Text = message.MessageTimestamp.ToString("MM/dd/yy HH:mm");

                // We return convertView. 
                return convertView;
                //if (convertView == null)
                //{
                //    // convertView shows one conversation the the conversation list
                //    convertView = context.LayoutInflater.Inflate(Resource.Layout.TheirMessageListItem, null);
                //}

                //convertView.FindViewById<TextView>(Resource.Id.theirMessageText).Text = theirMessages[position].MessageText;
                //convertView.FindViewById<TextView>(Resource.Id.theirMessageDate).Text = theirMessages[position].MessageTimestamp.ToShortTimeString();
                //return convertView;
            }

            // We overrode Count to return the number of conversations. 
            public override int Count
            {
                get { return theirMessages == null ? 0 : theirMessages.Length; }
            }

            //  We implemented an indexer to return a Conversation object for a position. 
            public override Models.Message this[int position]
            {
                get { return theirMessages[position]; }
            }
        }
    }

    internal class RetrievedPreKey
    {
        public uint PrekeyID { get; set; }
        public IdentityKey PublicPreKey { get; set; }
    }

    internal class API_Response3
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string PreKeyID { get; set; }
        public string PreKey { get; set; }
    }

    internal class API_Response2
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public int MessageID { get; set; }
        public uint MessageSenderRegisID { get; set; }
        public uint MessageReceiverRegisID { get; set; }
        public string MessageText { get; set; }
        public DateTime MessageTimestamp { get; set; }
        public bool Sent { get; set; }
    }
}