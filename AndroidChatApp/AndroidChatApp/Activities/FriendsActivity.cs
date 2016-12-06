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
using System.Threading.Tasks;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Android.Preferences;

namespace AndroidChatApp.Activities
{
    [Activity(Label = "Friends")]
    public class FriendsActivity : Activity
    {
        public User[] Friends = new User[] { };
        public User Friend = new User();
        //    public Models.Message[] Messages { get; private set; }
        //    public string Text { get; set; }
        ListView listView;
        Adapter adapter;

        protected override void OnCreate(Bundle bundle)
        {
            // Get the conversations from the server
            base.OnCreate(bundle);

            Friends = GetFriends();
            // Set our view from the "FriendsList" layout resource
            SetContentView(Resource.Layout.Conversation);
            listView = FindViewById<ListView>(Resource.Id.FriendsList);
            //***display FriendsListItem in ListView using Adapter
            listView.Adapter = adapter = new Adapter(this, Friends);

            // Set the click event
            listView.ItemClick += (sender, e) =>
            {
                Friend = adapter[e.Position];

                ISharedPreferences sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = sharedPref.Edit();
                editor.PutString("SelectedFriend", JsonConvert.SerializeObject(Friend));
                List<Models.Message> messages = new List<Models.Message>();
                editor.PutString("SelectedFriendMessageList", JsonConvert.SerializeObject(messages));
                editor.Apply();

                StartActivity(typeof(MessagesActivity));
            };

            // **List out all the registered friends in this FriendsList Page
            // **If two clients have already talked before, display the last message or unread message here.
            // **If two clients haven't talked before, when click on one friend, do initiating session setup

        }

        // Retrieve a list of conversations 
        private User[] GetFriends()
        {
            // send the server the user name
            // server side does the selection and return the other users' names/id and store it in a user array.
            //Send the login username and password to the server and get response
            string apiUrl = "https://ycandgap.me/api_server2.php";
            string apiMethod = "getFriends";

            //Login_Request has two properties:username and password
            Login_Request myLogin_Request = new Login_Request();
            //get the login username from previow login page.
            ISharedPreferences sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            myLogin_Request.RegistrationID = Convert.ToUInt32(sharedPref.GetString("RegistrationId", string.Empty));


            // make http post request
            string response = Http.Post(apiUrl, new NameValueCollection()
                {
                    { "api_method", apiMethod                                    },
                    { "api_data",   JsonConvert.SerializeObject(myLogin_Request) }
                });

            // decode json string to dto object
            API_Response1 r = JsonConvert.DeserializeObject<API_Response1>(response);

            // check response
            if (r != null)
            {
                if (!r.IsError)
                {
                    if (r.Array != null)
                    {
                        MessagesActivity m = new MessagesActivity();
                        List<User> friends = new List<User>();
                        string lastmessage = string.Empty;
                        //(m.GetMessages(sharedPref) != null) ? m.GetMessages(sharedPref).FirstOrDefault().MessageText : string.Empty;
                        foreach (Friend friend in r.Array)
                        {
                            friends.Add(new User()
                            {
                                IdentityKey = friend.IdentityKey,
                                LastMessage = lastmessage,
                                RegisterationID = Convert.ToUInt32(friend.RegistrationID),
                                SignedPreKeyID = Convert.ToUInt32(friend.SignedPreKeyID),
                                SignedPreKeySignature = friend.SignedPreKeySignature,
                                SignedPreKey = friend.SignedPreKey,
                                Username = friend.Username
                            });
                        }
                        return friends.ToArray();
                    }
                    else
                    {
                        return null;
                    }
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
        // Create a subclasse of BaseAdapter<Conversation>: Adapter
        //Connect database and UI
        // adapter holds data from database and send the data to dapter view
        public class Adapter : BaseAdapter<User>
        {
            User[] users;
            Activity context;
            //        readonly LayoutInflater inflater;

            // We passed in a Context parameter (our activity) so that we can pull out the LayoutInflater. 
            // This class enables us to load XML layout resources and inflate them into a view object.
            public Adapter(Activity context, User[] users) : base()
            {
                this.context = context;
                this.users = users;
                //inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            }
            //  Implement GetItemId method. 
            // This is a general method used to identify rows, so try to return a unique number. 
            public override long GetItemId(int position)
            {
                return position;
            }

            // We set up GetView, which recycles the convertView variable by only creating a new view if it is null. 
            // We also pulled out the text views in our layout to set their text. 
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                if (convertView == null)
                {
                    // convertView shows one conversation the the conversation list
                    convertView = context.LayoutInflater.Inflate(Resource.Layout.ConversationListItem, null);
                }

                convertView.FindViewById<TextView>(Resource.Id.conversationUsername).Text = users[position].Username;
                convertView.FindViewById<TextView>(Resource.Id.conversationLastMessage).Text = users[position].LastMessage;
                return convertView;
            }

            // We overrode Count to return the number of conversations. 
            public override int Count
            {
                get { return users == null ? 0 : users.Length; }
            }

            //  We implemented an indexer to return a Conversation object for a position. 
            public override User this[int position]
            {
                get { return users[position]; }
            }
        }
}

    public class API_Response1
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public Friend[] Array { get; set; }
    }

    public class Friend
    {
        public string Username { get; set; }
        public string RegistrationID { get; set; }
        public string IdentityKey { get; set; }
        public string SignedPreKey { get; set; }
        public string SignedPreKeyID { get; set; }
        public string SignedPreKeySignature { get; set; }
    }
}