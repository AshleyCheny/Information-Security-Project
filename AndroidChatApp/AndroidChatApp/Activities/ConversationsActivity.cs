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

namespace AndroidChatApp.Activities
{
    [Activity(Label = "Friends")]
    public class ConversationActivity : Activity
    {
        public Conversation[] Conversations = new Conversation[] { };
        public Conversation Conversation = new Conversation();
        //    public Models.Message[] Messages { get; private set; }
        //    public string Text { get; set; }
        ListView listView;
        Adapter adapter;

        protected override void OnCreate(Bundle bundle)
        {
            // Get the conversations in the server
            base.OnCreate(bundle);
            Conversations = GetConversations();
            // Set our view from the "FriendsList" layout resource
            SetContentView(Resource.Layout.Conversation);
            listView = FindViewById<ListView>(Resource.Id.FriendsList);
            //***display FriendsListItem in ListView using Adapter
            listView.Adapter = adapter = new Adapter(this, Conversations);

            // Set the click event
            listView.ItemClick += (sender, e) =>
            {
                Conversation = adapter[e.Position];

                StartActivity(typeof(MessagesActivity));
            };

            // **List out all the registered friends in this FriendsList Page
            // **If two clients have already talked before, display the last message or unread message here.
            // **If two clients haven't talked before, when click on one friend, do initiating session setup

        }

        //    // This code will set up the adapter and reload our list of FriendsList when the activity appears on screen
        //    protected override void OnResume()
        //    {
        //        // Call OnResume method
        //        base.OnResume();
        //        try
        //        {
        //            // Get the conversations in the server
        //            GetConversations();

        //            adapter.NotifyDataSetInvalidated();
        //        }
        //        catch (Exception exc)
        //        {
        //            DisplayError(exc);
        //        }
        //    }

        //    private void DisplayError(Exception exc)
        //    {
        //        string error = exc.Message;
        //        new AlertDialog.Builder(this)
        //            .SetTitle(2130968578)
        //            .SetMessage(error)
        //            .SetPositiveButton(Android.Resource.String.Ok,
        //            (IDialogInterfaceOnClickListener)null)
        //            .Show();
        //    }

        // Retrieve a list of conversations 
        public Conversation[] GetConversations()
        {
            // send the server the user name
            // server side does the selection and return the other users' names/id and store it in a Coversation array.
            //Send the login username and password to the server and get response
            string apiUrl = "https://ycandgap.me/api_server2.php";
            string apiMethod = "getConversations";

            //Login_Request has two properties:username and password
            Login_Request myLogin_Request = new Login_Request();
            //get the login username from previow login page.
            myLogin_Request.userRegisterID = Intent.GetIntExtra("UserRegisterID", 2016);

            // make http post request
            string response = Http.Post(apiUrl, new NameValueCollection()
                {
                    { "api_method", apiMethod                                    },
                    { "api_data",   JsonConvert.SerializeObject(myLogin_Request) }
                });

            // decode json string to dto object
            API_Response1 r = JsonConvert.DeserializeObject<API_Response1>(response);

            // check response
            if (!r.IsError)
            {
                return Conversations = new Conversation[] {new Conversation { ConversationID=r.ConversationID, FriendName=r.SenderName,
                        FriendRegisID =r.ReceiverReigsID, LastMessage = r.LastMessage, SenderRegisID = r.SenderRegisID} };
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
        // Create a subclasse of BaseAdapter<Conversation>: Adapter
        //Connect database and UI
        // adapter holds data from database and send the data to dapter view
        public class Adapter : BaseAdapter<Conversation>
        {
            Conversation[] conversations;
            Activity context;
            //        readonly LayoutInflater inflater;

            // We passed in a Context parameter (our activity) so that we can pull out the LayoutInflater. 
            // This class enables us to load XML layout resources and inflate them into a view object.
            public Adapter(Activity context, Conversation[] conversations) : base()
            {
                this.context = context;
                this.conversations = conversations;
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

                convertView.FindViewById<TextView>(Resource.Id.conversationUsername).Text = conversations[position].FriendName;
                convertView.FindViewById<TextView>(Resource.Id.conversationLastMessage).Text = conversations[position].LastMessage;
                return convertView;
            }

            // We overrode Count to return the number of conversations. 
            public override int Count
            {
                get { return conversations == null ? 0 : conversations.Length; }
            }

            //  We implemented an indexer to return a Conversation object for a position. 
            public override Conversation this[int position]
            {
                get { return conversations[position]; }
            }
        }
}

    public class API_Response1
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public int ConversationID { get; set; }
        public int SenderRegisID { get; set; }
        public int ReceiverReigsID { get; set; }
        public string SenderName { get; set; }
        public string LastMessage { get; set; }
    }
}