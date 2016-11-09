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

namespace ChatApp.Activities
{
    [Activity(Label = "MessagesActivity")]
    public class MessagesActivity : BaseActivity<MessageViewModel>
    {
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
    }
    }