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
using ChatAppCP.Models;

namespace ChatAppCP.MockWebService
{
    public class FakeWebService: IWebService
    {
        // Create the SleepDuration property to store a number in milliseconds.
        // This is to simulate an interaction with a web server. 
        public int SleepDuration { get; set; }

        // This is a constructor method for FakeWebService Class, setting the initial value of SleepDuration.
        public FakeWebService()
        {
            SleepDuration = 1;
        }

        // Create the Sleep method to return a task that introduce delays of a number of milliseconds.
        // This methods will be used throughout this Fake service to cause a delay on each operation.
        private Task Sleep()
        {
            return Task.Delay(SleepDuration);
        }

        // Implement the Login method with an await call on Sleep method and return a new User object with the appropriate Username.
        // ***Implement JWT to check specific credentials.
        public async Task<User> Login(string username, string password)
        {
            await Sleep();

            return new User { Id = 1, Username = username };
        }

        // Implement Register method with an await call on Sleep method and return the user object.
        public async Task<User> Register(User user)
        {
            await Sleep();

            return user;
        }

        // Implement GetFriends method
        public async Task<User[]> GetFriends(int userId)
        {
            await Sleep();

            return new[]
            {
                new User { Id = 2, Username = "Alice" },
                new User { Id = 3, Username = "Bob"},
                new User { Id = 4, Username = "Chris"},
            };
        }

        // Implement AddFriend method
        public async Task<User> AddFriend(int userId, string username)
        {
            await Sleep();

            return new User { Id = 5, Username = username };
            
        }

        // Implement GetConversation method.
        public async Task<Conversation[]> GetConversations(int userId)
        {
            await Sleep();

            return new[]
            {
                new Conversation { Id = 1, UserId = 2 },
                new Conversation { Id = 1, UserId =3 },
                new Conversation { Id = 1, UserId =4 }

            };
          
        }

        // Implement GetMessages to retrieve a list of messages.
        public async Task<Models.Message[]> GetMessages(int conversationId)
        {
            await Sleep();

            return new[]
            {
                new Models.Message { Id = 1, ConversationId = conversationId, UserId = 2, Text = "Hey" },
                new Models.Message { Id = 2, ConversationId = conversationId, UserId = 1, Text ="What's up?"},
                new Models.Message { Id = 3, ConversationId = conversationId, UserId = 2, Text = "Did you finish the project?"},
                new Models.Message { Id = 4, ConversationId = conversationId, UserId = 1, Text = "Oh, not yet. How about you?"},
            };
        }

        // Implement SendMessage method
        public async Task<Models.Message> SendMessage(Models.Message message)
        {
            await Sleep();

            return message;
            
        }
    }
}