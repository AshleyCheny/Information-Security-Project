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
using ChatAppCP.Models;
using System.Threading.Tasks;

namespace ChatAppCP.MockWebService
{
    // In our App, there are 7 operations the App will perform aganist a web server.
    // Define an interface that offers methods for each operation.
    public interface IWebService
    {
        Task<User> Login(string username, string password);
        Task<User> Register(User user);
        Task<User[]> GetFriends(int userId);
        Task<User> AddFriend(int userId, string username);
        Task<Conversation[]> GetConversations(int userId);
        Task<Models.Message[]> GetMessages(int conversationId);
        Task<Models.Message> SendMessage(Models.Message message);

    }
}