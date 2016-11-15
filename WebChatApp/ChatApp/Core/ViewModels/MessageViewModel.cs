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
using System.Threading.Tasks;
using ChatApp.Core.Models;

namespace ChatApp.Core.ViewModels
{
    public class MessageViewModel : BasicViewModel
    {
        public Conversation[] Conversations { get; private set; }
        public Conversation Conversation { get; set; }
        public Models.Message[] Messages { get; private set; }
        public string Text { get; set; }


        // Retrieve a list of conversations 
        public async Task GetConversations()
        {
            if (settings.User == null)
                throw new Exception("Not logged in.");

            IsBusy = true;
            try
            {
                Conversations = await service.GetConversations(settings.User.Id);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Retrieve a list of messages within a conversation. 
        public async Task GetMessages()
        {
            if (Conversation == null)
                throw new Exception("No conversation.");

            IsBusy = true;
            try
            {
                Messages = await service.GetMessages(Conversation.Id);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Send a message and update the local list of messages 
        public async Task SendMessage()
        {
            if (settings.User == null)
                throw new Exception("Not logged in.");

            if (Conversation == null)
                throw new Exception("No conversation.");

            if (string.IsNullOrEmpty(Text))
                throw new Exception("Message is blank.");

            IsBusy = true;
            try
            {
                var message = await service.SendMessage(new Models.Message
                {
                    UserId = settings.User.Id,
                    ConversationId = Conversation.Id,
                    Text = Text
                });

                //Update our local list of messages
                var messages = new List<Models.Message>();
                if (Messages != null)
                    messages.AddRange(Messages);
                messages.Add(message);

                Messages = messages.ToArray();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}