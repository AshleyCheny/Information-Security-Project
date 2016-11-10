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
using Message = ChatApp.Core.Models.Message;


namespace ChatApp.Activities
{
    [Activity(Label = "Username here**")]
    public class MessagesActivity : BaseActivity<MessageViewModel>
    {
        //  This displays views vertically in a list with the help of an adapter class that determines the number of child views. It also has support for its children to be selected. 
        ListView listView;
        EditText messageText;
        Button sendButton;
        Adapter adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            Title = viewModel.Conversation.Username;
            SetContentView(Resource.Layout.Messages);
            listView = FindViewById<ListView>(Resource.Id.messageList);
            messageText = FindViewById<EditText>(Resource.Id.messageText);
            sendButton = FindViewById<Button>(Resource.Id.sendButton);

            listView.Adapter =
                adapter = new Adapter(this);

            sendButton.Click += async (sender, e) =>
            {
                viewModel.Text = messageText.Text;
                try
                {
                    await viewModel.SendMessage();
                    messageText.Text = string.Empty;
                    adapter.NotifyDataSetInvalidated();
                    listView.SetSelection(adapter.Count);
                }
                catch (Exception exc)
                {
                    DisplayError(exc);
                }
            };
        }

        // Next, we'll need to implement OnResume to load the messages, invalidate the adapter, and then scroll the list view to the end,
        protected async override void OnResume()
        {
            base.OnResume();

            try
            {
                await viewModel.GetMessages();
                adapter.NotifyDataSetInvalidated();
                listView.SetSelection(adapter.Count);
            }
            catch (Exception exc)
            {
                DisplayError(exc);
            }
        }

        class Adapter : BaseAdapter<Core.Models.Message>
        {
            readonly MessageViewModel messageViewModel = ServiceContainer.Resolve<MessageViewModel>();
            readonly ISettings settings = ServiceContainer.Resolve<ISettings>();
            readonly LayoutInflater inflater;
            const int MyMessageType = 0, TheirMessageType = 1;

            public Adapter(Context context)
            {
                inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override int Count
            {
                get { return messageViewModel.Messages == null ? 0 : messageViewModel.Messages.Length; }
            }

            public override Message this[int index]
            {
                get { return messageViewModel.Messages[index]; }
            }

            public override int ViewTypeCount
            {
                get { return 2; }
            }

            public override int GetItemViewType(int position)
            {
                var message = this[position];
                return message.UserId == settings.User.Id ? MyMessageType : TheirMessageType;
            }

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
                        convertView = inflater.Inflate(Resource.Layout.MyMessageListItem, null);
                    }
                    else
                    {
                        convertView = inflater.Inflate(Resource.Layout.TheirMessageListItem, null);
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
                messageText.Text = message.Text;
                dateText.Text = message.Date.ToString("MM/dd/yy HH:mm");

                // We return convertView. 
                return convertView;
            }
        }
    }
}