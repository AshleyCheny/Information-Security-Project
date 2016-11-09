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

namespace ChatApp.Core.Models
{
    public class FakeWebService: IWebServices
    {
        public int SleepDuration { get; set; }

        public FakeWebService()
        {
            SleepDuration = 1;
        }

        private Task Sleep()
        {
            return Task.Delay(SleepDuration);
        }

        public async Task<User> Login(string username, string password)
        {
            await Sleep();

            return new User { Id = "1", Username = username };
        }

        public async Task<User> Register(User user)
        {
            await Sleep();

            return user;
        }

        public async Task<Message[]> GetMessages(string conversationId)
        {
            await Sleep();

            return new[]
            {
                new Message
                {
                    Id = "1",
                    ConversationId = conversationId,
                    UserId = "2",
                    Text = "Hey",
                    Date = DateTime.Now.AddMinutes(-15),
                },
                new Message
                {
                    Id = "2",
                    ConversationId = conversationId,
                    UserId = "1",
                    Text = "What's Up?",
                    Date = DateTime.Now.AddMinutes(-10),
                },
                new Message
                {
                    Id = "3",
                    ConversationId = conversationId,
                    UserId = "2",
                    Text = "Have you seen that new movie?",
                    Date = DateTime.Now.AddMinutes(-5),
                },
                new Message
                {
                    Id = "4",
                    ConversationId = conversationId,
                    UserId = "1",
                    Text = "It's great!",
                    Date = DateTime.Now,
                },
            };
        }

        public async Task<Message> SendMessage(Message message)
        {
            await Sleep();

            return message;
        }

        public async Task RegisterPush(string userId, string deviceToken)
        {
            await Sleep();
        }
    }
}