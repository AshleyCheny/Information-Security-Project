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

        // When a user calls the Login method (input username and password), the server will create a new User instance with Id = 1 and Username.
        public async Task<User> Login(string username, string password)
        {
            await Sleep();

            return new User { Id = 1, Username = username };
        }

        // When a user calls Register method, the user will pass its information(Username and Password).
        public async Task<User> Register(User user)
        {
            await Sleep();

            return user;
        }

        // When the user calls GetConversations method, the server will return a new conversation with conversation id = 1. 
        public async Task<Conversation[]> GetConversations(int userId)
        {
            await Sleep();

            return new[]
            {
                // In this Conversation, the user will talk to user whose UserId = 2.
                new Conversation { Id = 1, UserId = 2, Username = "bobama", LastMessage = "Hey!" },
                new Conversation { Id = 2, UserId = 3, Username = "bobloblaw", LastMessage = "Have you seen that new movie?" },
                new Conversation { Id = 3, UserId = 4, Username = "gmichael", LastMessage = "What?" },
            };
        }

        // When the user enter the conversation, it will pass a conversationId to the server.
        public async Task<Message[]> GetMessages(int conversationId)
        {
            await Sleep();

            // The server then create and return several new messages.
            return new[]
            {
                new Message
                {
                    Id = 1,
                    ConversationId = conversationId,
                    UserId = 2,
                    Text = "Hey",
                    Date = DateTime.Now.AddMinutes(-15),
                },
                new Message
                {
                    Id = 2,
                    ConversationId = conversationId,
                    UserId = 1,
                    Text = "What's Up?",
                    Date = DateTime.Now.AddMinutes(-10),
                },
                new Message
                {
                    Id = 3,
                    ConversationId = conversationId,
                    UserId = 2,
                    Text = "Have you seen that new movie?",
                    Date = DateTime.Now.AddMinutes(-5),
                },
                new Message
                {
                    Id = 4,
                    ConversationId = conversationId,
                    UserId = 1,
                    Text = "It's great!",
                    Date = DateTime.Now,
                },
            };
        }

        // When the user calls the SendMessage method with the message he or she just types in.
        public async Task<Message> SendMessage(Message message)
        {
            await Sleep();

            // The server will return the message passing to it.
            return message;
        }

        //public async Task RegisterPush(string userId, string deviceToken)
        //{
        //    await Sleep();
        //}
    }
}