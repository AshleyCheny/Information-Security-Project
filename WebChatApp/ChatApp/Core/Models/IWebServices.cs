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
    // Create a fake web service interface containing methods that user will call when in the chatting process.
    public interface IWebServices
    {
        Task<User> Login(string username, string password);

        Task<User> Register(User user);

        Task<Conversation[]> GetConversations(int userId);

        Task<Message[]> GetMessages(int conversationId);

        Task<Message> SendMessage(Message message);

        //Task RegisterPush(string userId, string deviceToken);
    }
}