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
using ChatApp.Core.ViewModels;
using ChatApp.Core.Models;

namespace ChatApp.Activities
{
    [Activity(Label = "Conversations")]
    public class ConversationsActivity : BaseActivity<MessageViewModel>
    {
        ListView listView;
        Adapter adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.Conversations);
            listView = FindViewById<ListView>(Resource.Id.conversationsList);
            listView.Adapter = adapter = new Adapter(this);

            // Set the click event
            listView.ItemClick += (sender, e) =>
            {
                viewModel.Conversation = adapter[e.Position];

                StartActivity(typeof(MessagesActivity));
            };
        }

        // This code will set up the adapter and reload our list of conversations when the activity appears on screen
        protected async override void OnResume()
        {
            // Call BaseActivity's OnResume method
            base.OnResume();

            try
            {
                // Get the three Conversations created in FakeWebService
                await viewModel.GetConversations();

                adapter.NotifyDataSetInvalidated(); 
            }
            catch (Exception exc)
            {
                DisplayError(exc);
            }
        }

        // Create a subclasse of BaseAdapter<Conversation>: Adapter
        //Connect database and UI
        // adapter holds data from database and send the data to dapter view
        class Adapter : BaseAdapter<Conversation>
        {
            readonly MessageViewModel messageViewModel = ServiceContainer.Resolve<MessageViewModel>();

            readonly LayoutInflater inflater;

            // We passed in a Context parameter (our activity) so that we can pull out the LayoutInflater. 
            // This class enables us to load XML layout resources and inflate them into a view object.
            public Adapter(Context context)
            {
                inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            }

            //  Implement GetItemId method. 
            // This is a general method used to identify rows, so try to return a unique number. 
            public override long GetItemId(int position)
            {
                return messageViewModel.Conversations[position].Id;
            }

            // We set up GetView, which recycles the convertView variable by only creating a new view if it is null. 
            // We also pulled out the text views in our layout to set their text. 
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                if (convertView == null)
                {
                    // convertView shows one conversation the the conversation list
                    convertView = inflater.Inflate(Resource.Layout.ConversationListItem, null);
                }

                var conversation = this [position];
                var username = convertView.FindViewById<TextView>(Resource.Id.conversationUsername);
                var lastMessage = convertView.FindViewById<TextView>(Resource.Id.conversationLastMessage);

                username.Text = conversation.Username;
                lastMessage.Text = conversation.LastMessage;
                return convertView;
            }

            // We overrode Count to return the number of conversations. 
            public override int Count
            {
                get { return messageViewModel.Conversations == null ? 0 : messageViewModel.Conversations.Length; }
            }

            //  We implemented an indexer to return a Conversation object for a position. 
            public override Conversation this[int index]
            {
                get { return messageViewModel.Conversations[index]; }
            }
        }
    }

}